using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour {


	SquareManager sqman;
	public AudioSource gameAudio;
	public AudioClip gameClip;
	public int w;
	public int h;
	Square[,] board;
	int[] q;

	public GameObject groundSquareFolder;
	public List<Square> groundSquares;

	void Start () {
		groundSquareFolder = new GameObject();
		groundSquareFolder.name = "Ground";
		groundSquares = new List<Square> ();

		initSound ();
	}

	void initSound(){
		gameAudio = this.gameObject.AddComponent<AudioSource> ();
		gameAudio.loop = true;
		gameAudio.playOnAwake = false;
		gameClip = Resources.Load<AudioClip> ("Audio/Soundtrack Draft 1");
		gameAudio.clip = gameClip;
		gameAudio.Play ();
	}

	void Update(){
		if (Input.GetMouseButtonUp(0)) {
			Vector3 worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			int mousex = (int) Mathf.Floor(worldPos.x);
			int mousey = (int) Mathf.Ceil(worldPos.y);
			sqman.placeSquare(new Vector2((float)mousex, (float)mousey));
		}

	}


	public void initBoard(){
		StreamReader sr = new StreamReader ("Assets/Resources/Levels/" + getLevelName ());
		string line = "";
		line = sr.ReadLine ();
		w = int.Parse (line);
		line = sr.ReadLine ();
		h = int.Parse (line);
		board = new Square[w, h];
		fixCamera ();
		for (int i = 0; i < w; i++) {
			line = sr.ReadLine ();
			addColumn (int.Parse (line), i);
		}
		line = sr.ReadLine ();
		int beginHeight = int.Parse (line);
		makeBeginning (beginHeight);
		line = sr.ReadLine();
		int destHeight = int.Parse (line);
		makeDestination (destHeight);
		q = new int[10];
		if (int.Parse (sr.ReadLine()) == 1) {
			int start = 0;
			for (int i = 0; i < 6; i++) {
				int prob = int.Parse (sr.ReadLine ());
				for (int j = 0; j < prob; j++) {
					q [j + start] = i;
				}
				start = start + prob;
			}

		}
	}

	public void fixCamera(){
		Camera cam = Camera.main;
		int height = (int)(h / 2);
		int width = (int)(w / 2);
		cam.orthographicSize = height + 2;
		cam.transform.position = new Vector3(width-1, height-1, -10);



	}

	//adds a column of ground from 0 to height
	public void addColumn(int height, int x){
		for (int i = 0; i <= height; i++) {
			Square s = addSquare (new Vector2 (x, i), true); 
			print (s == null);
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

		//		square.transform.parent = squareFolder.transform;
		square.transform.position = new Vector3 (-1, height, 0);
		square.init(new Vector2((float) -1, (float) height), 4, false);

		square.name = "Beginning";

		return square;
	}

	public Square makeDestination(int height){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		//		square.transform.parent = squareFolder.transform;
		square.transform.position = new Vector3 (w, height, 0);
		square.init(new Vector2((float) w, (float) height), 4, false);

		square.name = "Destination";

		return square;
	}

	public Square addSquare(Vector2 pos, bool isGround){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = groundSquareFolder.transform;
		square.transform.position = new Vector3 (pos.x, pos.y, 0);
		square.init (pos, -1, true);

		groundSquares.Add (square);
		square.name = "Ground " + groundSquares.Count;

		return square;

	}

	public string getLevelName(){		//TODO: make it good
		return "LTest.txt";

	}

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
			startGame ();
			break;
		}
	}

	private void menuScreen(){
		int xpos;
		int ypos;
		if (!go && !done) {
			xpos = ((Screen.width) - (100)) / 2;
			ypos = ((Screen.height) - (80)) / 2 - ((Screen.height / 3)-(Screen.height/10));
			GUI.Label (new Rect (xpos, ypos, 100, 50), "Stepping Stones");
		}
		if (!go && !done) {
			xpos = ((Screen.width) - (60)) / 2;
			ypos = ((Screen.height) / 2);
			if (GUI.Button (new Rect (xpos, ypos, 60, 90), "START")) {
				state.mode = 1;
			}
		}
	}

	private void startGame(){
		go = true;
		initBoard ();
		GameObject sqmanObject = new GameObject ();
		sqman = sqmanObject.AddComponent<SquareManager> ();
		sqman.name = "Square Manager";
		sqman.init (board, q);
	}
}