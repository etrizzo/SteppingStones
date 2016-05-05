using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System;

using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public SquareManager sqman;
	public AudioSource menuAudio;
	public AudioClip menuClip;
	public AudioSource gameAudio1;
	public AudioClip gameClip1;
	public AudioSource gameAudio2;
	public AudioClip gameClip2;
	public AudioSource gameAudio3;
	public AudioClip gameClip3;
	public AudioSource gameAudio4;
	public AudioClip gameClip4;
	public AudioSource gameAudio5;
	public AudioClip gameClip5;
	public AudioSource gameAudio6;
	public AudioClip gameClip6;
	public AudioSource gameAudio7;
	public AudioClip gameClip7;
	public int w;
	public int h;
	Square[,] board;
	bool useQueue;
	int[] q;
	int[] rsq;
	string levelName;
	int levelNum;


	public bool bambiQwop;

	public int NUMLEVELS = 9;
	public static int numBlockTypes = 3;
	public static int numRigidTypes = 2;

	public GameObject destinationSquareFolder;
	List<Square>destinationSquares;
	public GameObject beginningSquareFolder;
	List<Square>beginningSquares;

	public GameObject groundSquareFolder;
	public List<Square> groundSquares;
	public List<Square> inBoardSquares;

	public Camera cam;
	float dist;
	public Background bg;
	public static float x_coord, y_coord;

	public Square destination;
	public Square beginning;

	public Hero hero;
	public bool success = false;

	float wave = 0;
	int waveSpeed = 4;

	GUIStyle buttonStyle;
	GUIStyle guiStyle;
	GUIStyle guiStyle2;
	GUIStyle guiStyle3;
	GUIContent lvlbutton;
	GUIStyleState lvlButtonHover;

	public Highlight hi;

	public List<Square> squarePath;

	public Vector2 scrollPosition = Vector2.zero;

	int[] levelUnlockStatus = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };/*{1, 0, 0, 0, 0, 0, 0, 0, 0, 0};*/


	void Start () {
		levelUnlockStatus [0] = 1;
		for (int i = 1; i < NUMLEVELS; i++) {
			levelUnlockStatus[i] = PlayerPrefs.GetInt ("levelUnlockStatus"+i);
		}

		bambiQwop = false;

		groundSquareFolder = new GameObject();
		groundSquareFolder.name = "Ground";
		groundSquares = new List<Square> ();
		destinationSquareFolder = new GameObject();
		destinationSquareFolder.name = "Destination";
		destinationSquares = new List<Square> ();
		beginningSquareFolder = new GameObject();
		beginningSquareFolder.name = "Beginning";
		beginningSquares = new List<Square> ();

		initSound ();
		initStyles ();
	}

	private void initStyles(){
		//Cursor.SetCursor ((Texture2D)Resources.Load ("Textures/cursor"), new Vector2 (4, 4), CursorMode.Auto);

		buttonStyle = new GUIStyle ();
		buttonStyle.font = (Font) Resources.Load("Fonts/blockyo");
		buttonStyle.normal.textColor = new Color (0, 0, 0, .8f);

		guiStyle = new GUIStyle ();
		guiStyle.fontSize = 30;
		guiStyle.alignment = TextAnchor.MiddleCenter;
		guiStyle.font = (Font)Resources.Load ("Fonts/blockyo");
		guiStyle.richText = true;
		guiStyle.normal.textColor = new Color (1f, 1f, 1f, .9f);

		//HOME MENU
		guiStyle2 = new GUIStyle ();
		guiStyle2.fontSize = 80;
		guiStyle2.alignment = TextAnchor.MiddleCenter;
		guiStyle2.font = (Font)Resources.Load ("Fonts/blockyo");
		guiStyle2.richText = true;
		guiStyle2.normal.textColor = new Color (1f, 1f, 1f, .9f);

		guiStyle3 = new GUIStyle ();
		guiStyle3.fontSize = 45;
		guiStyle3.alignment = TextAnchor.MiddleCenter;
		guiStyle3.font = (Font)Resources.Load ("Fonts/blockyo");
		guiStyle3.richText = true;
		guiStyle3.normal.textColor = new Color (1f, 1f, 1f, .9f);

		GUI.depth = 10;
		lvlbutton = new GUIContent ();
		lvlButtonHover = new GUIStyleState ();
		lvlButtonHover.background = Resources.Load<Texture2D> ("Textures/glow");
		buttonStyle.hover = lvlButtonHover;
	}

	void initSound(){
		menuAudio = this.gameObject.AddComponent<AudioSource> ();
		menuAudio.loop = true;
		menuAudio.playOnAwake = false;
		menuClip = Resources.Load<AudioClip> ("Audio/MenuSoundtrack");
		menuAudio.clip = menuClip;
		menuAudio.Play ();

		gameAudio1 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio1.loop = true;
		gameAudio1.playOnAwake = false;
		gameClip1 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 1");
		gameAudio1.clip = gameClip1;
		gameAudio1.Play ();
		gameAudio1.mute = true;

		gameAudio2 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio2.loop = true;
		gameAudio2.playOnAwake = false;
		gameClip2 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 2");
		gameAudio2.clip = gameClip2;
		gameAudio2.Play ();
		gameAudio2.mute = true;

		gameAudio3 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio3.loop = true;
		gameAudio3.playOnAwake = false;
		gameClip3 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 3");
		gameAudio3.clip = gameClip3;
		gameAudio3.Play ();
		gameAudio3.mute = true;

		gameAudio4 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio4.loop = true;
		gameAudio4.playOnAwake = false;
		gameClip4 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 4");
		gameAudio4.clip = gameClip4;
		gameAudio4.Play ();
		gameAudio4.mute = true;

		gameAudio5 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio5.loop = true;
		gameAudio5.playOnAwake = false;
		gameClip5 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 5");
		gameAudio5.clip = gameClip5;
		gameAudio5.Play ();
		gameAudio5.mute = true;

		gameAudio6 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio6.loop = true;
		gameAudio6.playOnAwake = false;
		gameClip6 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 6");
		gameAudio6.clip = gameClip6;
		gameAudio6.Play ();
		gameAudio6.mute = true;

		gameAudio7 = this.gameObject.AddComponent<AudioSource> ();
		gameAudio7.loop = true;
		gameAudio7.playOnAwake = false;
		gameClip7 = Resources.Load<AudioClip> ("Audio/Soundtrack Layers/Layer 7");
		gameAudio7.clip = gameClip7;
		gameAudio7.Play ();
		gameAudio7.mute = true;
	}

	private void clearBoard(){
		sqman.clear ();
		hi.clear ();

		foreach (Square s in destinationSquares) {
			if (s != null) {
				print (s);
				s.destroy ();
			}
		}
		foreach (Square s in beginningSquares) {
			if (s != null) {
				s.destroy ();
			}
		}

		destinationSquares = new List<Square> ();
		beginningSquares = new List<Square> ();
		DestroyImmediate (sqman);
		DestroyImmediate (hi);
		DestroyImmediate (beginning);
		DestroyImmediate (destination);
		DestroyImmediate (hero);
	}

	void Update(){
		Vector3 worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		int mousex = (int)Mathf.Floor (worldPos.x);
		int mousey = (int)Mathf.Ceil (worldPos.y);
		if (state.mode == 1 && sqman != null) {		//if the game is playing
			if (Input.GetMouseButtonUp (0)) {
				if (!success) {
					sqman.placeSquare (new Vector2 ((float)mousex, (float)mousey));
				}
			}
			if (bambiQwop) {
				waveSpeed = 10;
			}
			if (success) {
				gameAudio1.mute = true;
				gameAudio2.mute = true;
				gameAudio3.mute = true;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
				if (levelNum < NUMLEVELS) {
					Debug.Log ("LEVEL " + levelNum + "UNLOCKD");
					levelUnlockStatus[levelNum] = 1;
					PlayerPrefs.SetInt("levelUnlockStatus"+levelNum, 1);
					/*if (!sqman.successAudio.isPlaying) {
						clearBoard ();
						setLevelName ("Level"+(levelNum+1), (levelNum+1));
						levelNum++;
						success = false;
						go = false;
						state.mode = 1;
					}*/
				}
			}
			if (sqman.height < 4 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = true;
				gameAudio3.mute = true;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 4 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = true;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 6 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 8 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 10 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = false;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 12 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = false;
				gameAudio6.mute = false;
				gameAudio7.mute = true;
			}
			if (sqman.height > 14 && !success) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = false;
				gameAudio6.mute = false;
				gameAudio7.mute = false;
			}
		} 

		if (destination != null) {
			wave += Time.deltaTime * waveSpeed;
			if (bambiQwop) {
				if (wave > 3) {
					destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/bambi2");
					wave = -1;
				} else if (wave > 2) {
					destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/bambi3");
				} else if (wave > 1) {
					destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/bambi2");
				} else if (wave > 0) {
					destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/bambi1");
				}
			} else {
				if (!sqman.destinationClose () && sqman.upset) {
					if (wave > 3) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window?2");
						wave = -1;
					} else if (wave > 2) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window?3");
					} else if (wave > 1) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window?2");
					} else if (wave > 0) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window?1");
					}
				} else {
					if (wave > 3) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window2");
						wave = -1;
					} else if (wave > 2) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window3");
					} else if (wave > 1) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window2");
					} else if (wave > 0) {
						destination.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window1");
					}
				}
			}
		}

	}


	public void initBoard(){
		Debug.Log ("MAEK BORD");
		TextAsset temp = Resources.Load<TextAsset>("Levels/"+getLevelName ()) as TextAsset;
		byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(temp.text);
		MemoryStream stream = new MemoryStream(byteArray);
		StreamReader sr = new StreamReader (stream);
		string line = "";
		line = readLine(sr);
		w = int.Parse (line);
		line = readLine(sr);
		h = int.Parse (line);
		board = new Square[w, h];
		fixCamera ();
		for (int i = 0; i < w; i++) {	//read ground height of each column in the board
			line = readLine(sr);
			addColumn (int.Parse (line), i);
		}
		line = readLine(sr);
		int beginHeight = int.Parse (line);		//get start height
		beginning = makeBeginning (beginHeight);
		hero = makeHero (beginHeight);
		line = readLine(sr);;					//get destination height
		int destHeight = int.Parse (line);
		destination = makeDestination (destHeight);
		q = new int[10];
		int prob = 0;
		int start = 0;
		useQueue = (int.Parse (readLine (sr)) == 1);
		if (useQueue) {
			for (int i = 0; i < numBlockTypes; i++) {
				print ("Adding block type : " + i);
				prob = int.Parse (readLine(sr));
				for (int j = 0; j < prob; j++) {
					q [j + start] = i;
				}
				start = start + prob;
			}

		}
		rsq = new int[10];
		int rsprob;
		if (prob != 0) {			//using rigid shapes
			start = 0;
			for (int i = 0; i < numRigidTypes; i++) {		//number of rigid shapes
				rsprob = int.Parse (readLine(sr));
				for (int j = 0; j < rsprob; j++) {
					rsq [j + start] = i;
				}
			}
		}
		Square sq;
		while (sr.Peek () != -1) {
			line = readLine(sr);
			string[] s = line.Split (' ');
			sq = addSquare (new Vector2 (float.Parse (s [0]), float.Parse (s [1])), int.Parse (s [2]), false, int.Parse (s [3]));
			sq.setColor (int.Parse (s [2]));
			sq.setType (int.Parse (s [3]));
//			Square sq = new Square ();
//			sq.init (new Vector2 (float.Parse (s [0]), float.Parse (s [1])), int.Parse (s [2]), false,  int.Parse (s [3]));
		}


	}


	public string readLine(StreamReader sr){
		String line = sr.ReadLine ();
		while (line.StartsWith("//") && sr.Peek () != -1) {
			line = sr.ReadLine ();
		}
		return line;



	}

	public void fixCamera(){

		this.cam = Camera.main;
		int height = (int)(h / 2);
		int width = (int)(w / 2);
		cam.orthographicSize = height + 2;
		cam.transform.position = new Vector3(width-1, height-1, -10);

		dist = (transform.position - cam.transform.position).z;
		x_coord = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, dist)).x;
		y_coord = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, dist)).y;



	}

	//adds a column of ground from 0 to height
	public void addColumn(int height, int x){
		for (int i = 0; i <= height; i++) {
			Square s = addGround(new Vector2 (x, i), true); 
//			print (s == null);
			s.init (new Vector2 (x, i), -1, true);
			board [x, i] = s;
		}
	}


//		board = new Square[BOARDSIZEX, BOARDSIZEY];
//		//initialize level w/ ground squares (read from text file?)
//		for (int i = 0; i < BOARDSIZEX; i++) {
//			Square s = addSquare (new Vector2(i,0), true);
//			//			s.init (new Vector2 (i, 0), -1, true);
//			board [i, 0] = s;
//
//		}
//
//
//		destination = makeExtremeSquare ("destination");
//		beginning = makeExtremeSquare ("beginning");
//	}

	public Square makeBeginning(int height){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = beginningSquareFolder.transform;
		square.transform.position = new Vector3 (-1, height, 0);
		square.init(new Vector2((float) -1, (float) height), 4, false);

		square.name = "Beginning";

		square.model.mat.color = Color.gray;

		for(int i=height-1; i >=0; i--){
			GameObject squareObject2 = new GameObject ();
			Square square2 = squareObject2.AddComponent<Square> ();

			square2.transform.parent = beginningSquareFolder.transform;
			square2.transform.position = new Vector3 (-1, i, 0);
			square2.init(new Vector2((float) -1, (float) i), 4, false);

			square2.name = "Beginning "+i;

			square2.model.mat.color = Color.gray;
		}

		return square;
	}

	public Hero makeHero(int height){
		print ("A NEW HERO IS BORN");
		GameObject heroObject = new GameObject ();
		Hero h = heroObject.AddComponent<Hero> ();

		//		square.transform.parent = squareFolder.transform;
		h.transform.position = new Vector3 (-.5f, height+.5f, 0);

		h.init(new Vector2((float) -1, (float) height+1), this);

		h.name = "Hero";

		return h;
	}

	public Square makeDestination(int height){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = destinationSquareFolder.transform;
		square.transform.position = new Vector3 (w, height, 0);
		square.init(new Vector2((float) w, (float) height), 4, false);

		square.name = "Destination";
//		sqman.destination = square;
		square.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/window1");

		GameObject towerTopObject1 = new GameObject ();
		Square towerTopSquare1 = towerTopObject1.AddComponent<Square> ();
		GameObject towerTopObject2 = new GameObject ();
		Square towerTopSquare2 = towerTopObject2.AddComponent<Square> ();

		towerTopSquare1.transform.position = new Vector3(w, height+1, 0);
		towerTopSquare1.init(new Vector2((float) w, (float) height+1), 5, false);
		towerTopSquare2.transform.position = new Vector3 (w + 1, height+1, 0);
		towerTopSquare2.init(new Vector2((float) w+1, (float) height+1), 5, false);

		towerTopSquare1.name = "Tower Top Square 1 ";
		towerTopSquare2.name = "Tower Top Square 2 ";
		towerTopSquare1.transform.parent = destinationSquareFolder.transform;
		towerTopSquare2.transform.parent = destinationSquareFolder.transform;


		towerTopSquare1.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/towerCornerL");
		towerTopSquare2.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/towerCornerR");

		destinationSquares.Add (towerTopSquare1);
		destinationSquares.Add (towerTopSquare2);

		for (int i = height; i >= 0; i--) {
			GameObject towerObject1 = new GameObject ();
			Square towerSquare1 = towerObject1.AddComponent<Square> ();
			GameObject towerObject2 = new GameObject ();
			Square towerSquare2 = towerObject2.AddComponent<Square> ();

			towerSquare1.transform.position = new Vector3(w, i, 0);
			towerSquare1.init(new Vector2((float) w, (float) i), 5, false);
			towerSquare2.transform.position = new Vector3 (w + 1, i, 0);
			towerSquare2.init(new Vector2((float) w+1, (float) i), 5, false);

			towerSquare1.name = "Tower Square 1 " + i;
			towerSquare2.name = "Tower Square 2 " + i;

			towerSquare1.transform.parent = destinationSquareFolder.transform;
			towerSquare2.transform.parent = destinationSquareFolder.transform;

			destinationSquares.Add (towerSquare1);
			destinationSquares.Add (towerSquare2);

		}

		/*GameObject towerObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		Tower tower = towerObject.AddComponent<Tower> ();

		tower.transform.position = new Vector3 (w, height, 0);
		tower.init(new Vector2((float) w, (float) height), square, this);

		tower.name = "Tower";*/

		return square;
	}

	public Square addGround(Vector2 pos, bool isGround){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = groundSquareFolder.transform;
		square.transform.position = new Vector3 (pos.x, pos.y, 0);
		square.init (pos, -1, isGround);

		groundSquares.Add (square);
		square.name = "Ground " + groundSquares.Count;

		return square;

	}

	public Square addSquare(Vector2 pos, int color, bool isGround, int type){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

//		square.transform.parent = groundSquareFolder.transform;
		square.transform.position = new Vector3 (pos.x, pos.y, 0);
		square.init (pos, color, isGround, type);
//		print ("Adding to inBoardSquares " + square);
		inBoardSquares.Add (square);
		square.name = "Square " + groundSquares.Count;
		board [(int)pos.x, (int)pos.y] = square;

		return square;
	}

	public string getLevelName(){		//TODO: make it good
		return levelName;

	}

	void setLevelName(string name, int num){
		levelName = name;
		levelNum = num;
	}

	public void pathAnimation(){
		squarePath.Reverse ();
		printPath ();
//		Time.timeScale = .01f;
//		Time.fixedDeltaTime = .01f;
		Debug.Log ("pathAnimation");
		//heroAnimation(squarePath);
		//Time.timeScale = 1f;
		hero.model.canMove = true;
	//	hero.model.moveAlong (squarePath);
	}

	void printPath(){
		String path = "PATH: ";
		foreach (Square s in squarePath){
			path = path + s.getPosition() + " ";
		}
		print(path);
	}
			

//	void heroAnimation(List<Square> squarePath){
//		int counter = 0;
//		foreach (Square sq in squarePath) {
//			Vector2 nextPos = sq.getPosition ();
//			hero.model.nextMove (nextPos);
//		}
//	}

	/************************ Start Gui Stuff ****************************/
	bool go = false;
	bool done = false;
	public struct GuiState
	{
		public int mode;
	}

	public GuiState state;
	// Start button that disappears once clicked (and triggers the start of the game)
	void OnGUI ()
	{

		switch (state.mode) {
		case 0:
			menuScreen ();
			break;
		case 1:
			if (!go) {			//only initialize the board once
				startGame ();
			}
			if (GUI.Button(new Rect(30, 30, 100, 40), "Test your path.")) {
				if (sqman.boardSolved ()) {
					success = true;

				}
				pathAnimation ();

			}
			if (GUI.Button (new Rect (Screen.width-160, 30, 100, 40), "Menu")) {
				Application.LoadLevel (Application.loadedLevel);

			}
			break;
		}
	}

	private void menuScreen(){

		menuAudio.mute = false;
		int xpos;
		int ypos;

		gameAudio1.Stop();
		gameAudio2.Stop();
		gameAudio3.Stop();
		gameAudio4.Stop();
		gameAudio5.Stop();
		gameAudio6.Stop();
		gameAudio7.Stop();

		if (!go && !done) {
			xpos = ((Screen.width) - (100)) / 2;
			ypos = ((Screen.height) - (80)) / 2 - ((Screen.height / 3)-(Screen.height/10));
			GUI.Label (new Rect (xpos, ypos, 90, 40), "<color=black>S t e p p i n g\n\nS t o n e s</color>", guiStyle2);
			GUI.Label (new Rect (xpos, ypos, 110, 60), "<color=black>S t e p p i n g\n\nS t o n e s</color>", guiStyle2);
			GUI.Label (new Rect (xpos, ypos, 100, 50), "<color=cyan>S</color> <color=magenta>t</color> <color=yellow>e</color> <color=cyan>p</color> <color=magenta>p</color> <color=yellow>i</color> <color=cyan>n</color> <color=yellow>g</color>\n\n<color=yellow>S</color> <color=cyan>t</color> <color=magenta>o</color> <color=yellow>n</color> <color=cyan>e</color> <color=magenta>s</color>", guiStyle2);

			xpos = ((Screen.width) - (100)) / 6;
			ypos = ((Screen.height) - (100)) / 4 * 3;
			GUI.Label (new Rect (xpos, ypos, 100, 50), "<color=black>C l i c k   t o\n\np l a c e\n\nb l o c k s</color>", guiStyle);
			xpos *= 5;
			GUI.Label (new Rect (xpos, ypos, 100, 50), "<color=black>C r e a t e\n\na\n\np a t h</color>", guiStyle);
			xpos = ((Screen.width) - (100)) / 2;
			ypos = ((Screen.height) - (80));
			GUI.Label (new Rect (xpos, ypos, 100, 50), "<color=cyan>S a v e</color>   <color=magenta>y o u r</color>   <color=yellow>f r i e n d</color>", guiStyle3);
			GUI.Label (new Rect (xpos, ypos, 90, 40), "<color=cyan>S a v e</color>   <color=magenta>y o u r</color>   <color=yellow>f r i e n d</color>", guiStyle3);
			GUI.Label (new Rect (xpos, ypos, 95, 45), "<color=black>S a v e   y o u r   f r i e n d</color>", guiStyle3);


		}
		if (!go && !done) {
			xpos = ((Screen.width)-256) / 2;
			ypos = ((Screen.height / 2));

			Texture bambiTexture = Resources.Load<Texture2D> ("Textures/bambi");
			bambiQwop = GUILayout.Toggle(bambiQwop,bambiTexture);

			scrollPosition = GUI.BeginScrollView (new Rect (xpos, ypos, 270, 200), scrollPosition, new Rect (0, 0, 220, 50 *NUMLEVELS)); 

			for (int i = 0; i < NUMLEVELS; i++) {
				if (levelUnlockStatus [i] == 0) {
					
					lvlbutton.image = Resources.Load<Texture2D> ("Textures/lockd");
				} else {
					print ("level: " + (i + 1));
					lvlbutton.image = Resources.Load<Texture2D> ("Textures/lv" + (i + 1));
				}
				if (GUI.Button (new Rect (0, 0+50*i, 256, 50), lvlbutton, buttonStyle)) {
					if (levelUnlockStatus [i] == 1) {
						setLevelName ("Level" + (i + 1), (i + 1));
						state.mode = 1;
					}
				}
			}
			GUI.EndScrollView ();
		}
	}



	private void initBackground ()
	{
		//creates background tile
		GameObject bg_object = new GameObject ();			
		bg_object.name = "BG Object";
		Background bg = bg_object.AddComponent<Background> ();	
		bg.transform.position = new Vector3 (0, 0, 0);		
		bg.init (0, 0, this);										
		bg.name = "Background";
		this.bg = bg;							
	}

	private void startGame(){

		menuAudio.mute = true;
		GameObject sqmanObject = new GameObject ();

		Debug.Log ("START GAEM");

		initBoard ();

		squarePath = new List<Square>();

		sqman = sqmanObject.AddComponent<SquareManager> ();
		sqman.name = "Square Manager";

		//		print ("initing sqman");
		if (!useQueue) {
			q = null;
		}
		sqman.init (this, board, q, rsq, inBoardSquares);

		sqman.destination = destination;
		sqman.beginning = beginning;
//		sqman.addBoardSquares (inBoardSquares);

		initBackground ();
		go = true;

		GameObject hiObject = new GameObject ();
		hi = hiObject.AddComponent<Highlight>();
		hi.init (sqman.queue, sqman);

		gameAudio1.Play();
		gameAudio2.Play();
		gameAudio3.Play();
		gameAudio4.Play();
		gameAudio5.Play();
		gameAudio6.Play();
		gameAudio7.Play();

	}
}
