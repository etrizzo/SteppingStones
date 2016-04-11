using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RigidShape : MonoBehaviour {
	// Starting with _
	// 0 = _
	// 1 = l
	int shapeType = -1;
	Color color1;
	Color color2;
	int c1, c2;
	Square[] squares = new Square[5];
	Square anchor;
	Square[,] board;
	Color[] acolors;

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

		updateModel ();

		//Debug.Log ("Rigid Shape inited!");
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

	}


	private int generateTypeProbability() {
		// Should be adjusted depending on level????????????????????????????????????????????????
		return (int) Random.Range(0,0);
	}

	public void grow() {
		// you know, grow and stuff .....

		switch (shapeType) {
		case 0:
			grow__();
			break;

		default:
			print ("Whoopsies! you hit the default shape case, line 42");
			break;
		}
	}

	private void grow__() {
		Vector2 pos = anchor.getPosition ();
		Vector2 new_pos;
		Mesh mesh = anchor.model.GetComponent<MeshFilter> ().mesh;
//		Color[] colors = new Color[mesh.uv.Length];
//		for (var i = 0; i < mesh.uv.Length; i++) {
//			colors [i] = color1;
//		}
		mesh.colors = acolors;
		anchor.setColor (c1);
		anchor.model.mat.color = color1;
		for (int i = 1; i <= 4; i++) {
			new_pos = new Vector2 (pos.x + i, pos.y);
			if (i % 2 == 0) {
				squares [i] = addSquare (new_pos, c1);
			} else {
				squares [i] = addSquare (new_pos, c2);
			}
		}
	}


	private void grow_l() {
		//stuff
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

		return square;

	}

	public IEnumerator settleShape(){
		Debug.Log("Settling shape!");
		Vector2 pos;
		yield return new WaitForSeconds (.25f);
		bool settle = checkSettle ();

		if (settle) {
			foreach(Square s in squares){
				pos = s.getPosition ();
				board [(int)pos.x, (int)pos.y] = null;
				board [(int)pos.x, (int)pos.y - 1] = s;
				s.setPosition (new Vector2 (pos.x, pos.y - 1));
			}
			yield return new WaitForSeconds (.25f);
			StartCoroutine (settleShape ());
		}
		else {
			//Debug.Log("Checking conflicts!");
			checkConflicts ();
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
			if (below != null && below.rigid != s.rigid) {
				return false;
			}
		}
		return true;
	}

	void checkConflicts(){
		foreach(Square s in squares){
			StartCoroutine (sm.checkConflicts (s));
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




	
}