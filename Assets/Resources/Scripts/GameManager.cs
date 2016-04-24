using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour {


	SquareManager sqman;
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
	int[] q;
	int[] rsq;
	string levelName;

	public GameObject groundSquareFolder;
	public List<Square> groundSquares;

	public Square destination;
	public Square beginning;

	void Start () {
		groundSquareFolder = new GameObject();
		groundSquareFolder.name = "Ground";
		groundSquares = new List<Square> ();

		initSound ();
	}

	void initSound(){
		menuAudio = this.gameObject.AddComponent<AudioSource> ();
		menuAudio.loop = true;
		menuAudio.playOnAwake = false;
		menuClip = Resources.Load<AudioClip> ("Audio/Soundtrack Draft 1");
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

	void Update(){
		if (state.mode == 1 && sqman != null) {		//if the game is playing
			if (Input.GetMouseButtonUp (0)) {
				Vector3 worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				int mousex = (int)Mathf.Floor (worldPos.x);
				int mousey = (int)Mathf.Ceil (worldPos.y);
				sqman.placeSquare (new Vector2 ((float)mousex, (float)mousey));
			}
			if (sqman.height < 4) {
				gameAudio1.mute = false;
				gameAudio2.mute = true;
				gameAudio3.mute = true;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 4) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = true;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 6) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = true;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 8) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = true;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 10) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = false;
				gameAudio6.mute = true;
				gameAudio7.mute = true;
			}
			if (sqman.height > 12) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = false;
				gameAudio6.mute = false;
				gameAudio7.mute = true;
			}
			if (sqman.height > 14) {
				gameAudio1.mute = false;
				gameAudio2.mute = false;
				gameAudio3.mute = false;
				gameAudio4.mute = false;
				gameAudio5.mute = false;
				gameAudio6.mute = false;
				gameAudio7.mute = false;
			}
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
		beginning = makeBeginning (beginHeight);
		line = sr.ReadLine();
		int destHeight = int.Parse (line);
		destination = makeDestination (destHeight);
		q = new int[10];
		int prob = 0;
		int start = 0;
		if (int.Parse (sr.ReadLine()) == 1) {
			
			for (int i = 0; i < 6; i++) {
				prob = int.Parse (sr.ReadLine ());
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
			for (int i = 0; i < 2; i++) {		//number of rigid shapes
				rsprob = int.Parse (sr.ReadLine ());
				for (int j = 0; j < rsprob; j++) {
					rsq [j + start] = i;
				}
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
//		sqman.destination = square;

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
		return levelName;

	}

	void setLevelName(string name){
		levelName = name;
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
			if (!go) {			//only initialize the board once
				startGame ();
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
			GUI.Label (new Rect (xpos, ypos, 100, 50), "Stepping Stones");
		}
		if (!go && !done) {
			xpos = ((Screen.width) - (60)) / 2;
			ypos = ((Screen.height) / 2);
			if (GUI.Button (new Rect (xpos-100, ypos, 100, 90), "Test Level 1")) {
				setLevelName ("LTest1.txt");
				state.mode = 1;
			}
			if (GUI.Button (new Rect (xpos, ypos, 100, 90), "Test Level 2")) {
				setLevelName ("LTest2.txt");
				state.mode = 1;
			}
			if (GUI.Button (new Rect (xpos+100, ypos, 100, 90), "Test Level 3")) {
				setLevelName ("LTest3.txt");
				state.mode = 1;
			}
		}
	}

	private void startGame(){
		menuAudio.mute = true;
		GameObject sqmanObject = new GameObject ();
		initBoard ();

		sqman = sqmanObject.AddComponent<SquareManager> ();
		sqman.name = "Square Manager";
		sqman.init (board, q, rsq);
		sqman.destination = destination;
		sqman.beginning = beginning;
		go = true;

		gameAudio1.Play();
		gameAudio2.Play();
		gameAudio3.Play();
		gameAudio4.Play();
		gameAudio5.Play();
		gameAudio6.Play();
		gameAudio7.Play();
	}
}
