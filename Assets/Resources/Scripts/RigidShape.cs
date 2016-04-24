using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RigidShape : MonoBehaviour {
	// Starting with _
	// 0 = _
	// 1 = l
	public int shapeType = -1; // 0-_, 1-l
	Color color1;
	Color color2;
	int c1, c2;
	Square[] squares = new Square[5];
	Square anchor;
	Square[,] board;
	Color[] acolors;
	public AudioSource rsAudio;
	public AudioClip rsClip;

	bool growing = false;
	int speed = 3;
	float growCounter = 0;

	SquareManager sm;

	public void init(Square a, Square[,] b, SquareManager sm) {
		anchor = a;
		anchor.rigid = this;
		anchor.setAnchor ();
		board = b;
		this.sm = sm;
		shapeType = generateTypeProbability();
		c1 = getColorInt ();
		c2 = getColorInt ();
		while (c1 == c2) {
			c2 = getColorInt ();
		}
		color1 = getColor (c1);
		color2 = getColor(c2);

		squares [0] = anchor;

		initSound ();
		updateModel ();

		//Debug.Log ("Rigid Shape inited!");
	}

	void initSound(){
		rsAudio = this.gameObject.AddComponent<AudioSource> ();
		rsAudio.loop = false;
		rsAudio.playOnAwake = false;
		rsClip = Resources.Load<AudioClip> ("Audio/Special Blocks/Grow 5");
		rsAudio.clip = rsClip;
	}

	private void updateModel() {
		// Do stuff to update model MANUALLY UGH
		Mesh mesh = anchor.model.GetComponent<MeshFilter>().mesh;
		Vector2[] uv = mesh.uv;
		Color[] colors = new Color[uv.Length];
		for (var i = 0; i < uv.Length; i++) {
			colors [i] = Color.Lerp (color1, color2, uv [i].x);
		}
		acolors = mesh.colors;
		mesh.colors = colors;

		switch (shapeType) {	//0-_l, 1-l

		case 0: 
			anchor.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tile__");
			break;
		case 1:
			anchor.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tile_l");
			break;
		default:
			anchor.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileBlank");	// Set the texture.  Must be in Resources folder.
			break;
		}

	}


	private int generateTypeProbability() {
		// Should be adjusted depending on level????????????????????????????????????????????????
		int r = Random.Range(0,10);
		return sm.rsq [r];
	}

	public void grow() {
		anchor.setFalling (true);
		growing = true;
		// you know, grow and stuff .....
		anchor.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileBlank");
		rsAudio.Play ();

	}

	void Update(){
		if (growing) {
			growCounter += Time.deltaTime * speed;
		}
		if (growCounter >= 1) {
			Vector2 pos = anchor.getPosition ();
			Vector2 new_pos;
			Mesh mesh = anchor.model.GetComponent<MeshFilter> ().mesh;
			mesh.colors = acolors;
			anchor.setColor (c1);
			anchor.model.mat.color = color1;

			switch (shapeType) {
			case 0: // _
				for (int i = 1; i <= 4; i++) {
					new_pos = new Vector2 (pos.x + i, pos.y);
					growCounter = 0;
					if (i % 2 == 0) {
						squares [i] = addSquare (new_pos, c1);
					} else {
						squares [i] = addSquare (new_pos, c2);
					}
				}
				growCounter = 0;
				//StartCoroutine (settleShape ());
				break;
			case 1: // l
				for (int i = 1; i <= 4; i++) {
					new_pos = new Vector2 (pos.x, pos.y + i);
					growCounter = 0;
					if (i % 2 == 0) {
						squares [i] = addSquare (new_pos, c1);
					} else {
						squares [i] = addSquare (new_pos, c2);
					}
				}
				growCounter = 0;
				break;
			default:
				print ("Whoopsies! you hit the default shape case, line 42");
				break;
			}
		} else {
			foreach (Square s in squares) {
				if (s != null) {
					s.setFalling (true);
				}
			}
		}

	}

	// ---
	public Square addSquare(Vector2 pos, int c){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = sm.squareFolder.transform;
		square.transform.position = new Vector3 (pos.x, pos.y, 0);
		square.init (pos, c, false, 0);
		square.rigid = this;

		sm.squares.Add (square);
		square.name = "Square " + sm.squares.Count;

		board [(int)pos.x, (int)pos.y] = square;
//		square.setFalling (true);
		square.addSqman(sm);
		return square;

	}

	public IEnumerator settleShape(){
		Vector2 pos;
		bool settle = checkSettle ();
//		Debug.Log("Settling shape! " +settle);
		if (settle) {
			foreach(Square s in squares){
				pos = s.getPosition ();
				board [(int)pos.x, (int)pos.y] = null;
				board [(int)pos.x, (int)pos.y - 1] = s;
				s.setPosition (new Vector2 (pos.x, pos.y - 1));
			}
			yield return new WaitForSeconds (.5f);
			if (this != null) {
				StartCoroutine (settleShape ());
			}
		}
		else {
			setShapeFalling (false);
			//Debug.Log("Checking conflicts!");
			sm.settleAudio.Play();
			checkConflicts ();
		}
	}

	public void setShapeFalling(bool b){
		foreach (Square s in squares) {
			Vector2 pos = s.getPosition ();
			Square below = board [(int)(pos.x), (int)(pos.y - 1)];
			Debug.Log(s + "at position"+s.getPosition().x+", "+s.getPosition().y+" has landed on " + below+" at position"+below.getPosition().x+", "+below.getPosition().y);
			s.setFalling (b);
		}
	}

	bool checkSettle(){
		Square s;
		Vector2 pos;
		Square below;
		for (int i = 0; i < 5; i++) {
			s = squares [i];
			pos = s.getPosition ();
			below = board [(int)pos.x, (int)(pos.y - 1)];
			if (below != null && below.rigid != s.rigid && !below.isFalling()) {
				return false;
			}
		}
		return true;
	}

	void checkConflicts(){
		foreach(Square s in squares){
			if (s != null) {
				StartCoroutine (sm.checkConflicts (s));
			}
		}
	}

	public Color getColor(int c){
		switch (c) {
		case 0:
			return Color.cyan;
			break;
		case 1:
			return Color.magenta;
			break;
		case 2:
			return Color.yellow;
			break;
		default:
			return Color.white;
			break;
		}
	}

	public int getColorInt(){
		return Random.Range (0, 3);
	}


	public Square[] getSquares(){
		return squares;
	}

	public bool checkValidGrow(Vector2 pos, int height, int width){
		if (pos.x + width < sm.BOARDSIZEX && pos.y < sm.BOARDSIZEY) {		//anchor should always be bottom left
			for (int i = 0; i < height; i++) {
				if (board [(int)pos.x, (int)(pos.y + i)] != null) {
//				if (!checkSpot((int)(pos.x), (int) (pos.y + i))){
					print ("invalid place");
					return false;
				}
			}
			for (int i = 0; i < width; i++) {
				if (board [(int)(pos.x + i), (int)pos.y] != null) {
//				if (!checkSpot((int)(pos.x + i), (int) (pos.y))){
					print ("invalid place");
					return false;
				}
			}
			return true;
		}
		print ("off edge of board");
		return false;
	}


	public bool checkSpot(int x, int y){
		if (board [x, y] != null) {
			return false;
		} else if (y + 1 < sm.BOARDSIZEY) {
			if (board [x, y] != null && board [x, y].falling) {
				return false;
			}
		}
		return true;

	}
}