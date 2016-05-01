using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareManager : MonoBehaviour {
	
	public GameObject squareFolder;
	public List<Square> squares;
	public GameObject rsFolder;
	public List<RigidShape> rigidshapes;
	public AudioSource successAudio;
	public AudioClip successClip;
	public AudioSource conflictAudio;
	public AudioClip conflictClip;
	public AudioSource movOnAudio;
	public AudioClip movOnClip;
	public AudioSource movOffAudio;
	public AudioClip movOffClip;

	public Queue<Square> queue;			// Add is enqueue, RemoveAt(0) is dequeue
	public int BOARDSIZEX = 24;
	public int BOARDSIZEY = 16;
	static int queueY = -1;
	Vector2 qpos1 = new Vector2(-4f, (float) queueY);
	Vector2 qpos2 = new Vector2(-3f, (float) queueY);
	Vector2 qpos3 = new Vector2(-2f, (float) queueY);
	public Square[,] board;
	int[] q;
	public int[] rsq;
	public int height;

	float counter = 0f;
	public Square moving = null;
	public bool firstSquare = true;
	public Square destination;		//TODO: TEMPORARY :O
	public Square beginning;		//TODO: TEMPORARY :O
	public GameManager gm;

	public bool conflict = false;

//	float randFreq = .2;


	public void init(GameManager gm, Square[,] board, int[] q, int[] rsq = null){
		this.gm = gm;
		squareFolder = new GameObject();
		squareFolder.name = "Squares";
		squares = new List<Square> ();
		rsFolder = new GameObject ();
		rsFolder.name = "Rigid Shapes";
		rigidshapes = new List<RigidShape> ();
//		initBoard();
		this.board = board;
		this.q = q;
		this.rsq = rsq;
		this.BOARDSIZEX = board.GetLength (0);
		this.BOARDSIZEY = board.GetLength(1);
		initQueue ();		//initialize queue w/ 3 initial blocks
		initSound ();
		//getHeight ();
	}

	void initSound(){
		successAudio = this.gameObject.AddComponent<AudioSource> ();
		successAudio.loop = false;
		successAudio.playOnAwake = false;
		successAudio.time = 1.0f;
		successClip = Resources.Load<AudioClip> ("Audio/Victory Climb");
		successAudio.clip = successClip;

		conflictAudio = this.gameObject.AddComponent<AudioSource> ();
		conflictAudio.loop = false;
		conflictAudio.playOnAwake = false;
		conflictClip = Resources.Load<AudioClip> ("Audio/Block Colors Match");
		conflictAudio.clip = conflictClip;

		movOnAudio = this.gameObject.AddComponent<AudioSource> ();
		movOnAudio.loop = false;
		movOnAudio.playOnAwake = false;
		movOnClip = Resources.Load<AudioClip> ("Audio/Special Blocks/Movable - Pick Up");
		movOnAudio.clip = movOnClip;

		movOffAudio = this.gameObject.AddComponent<AudioSource> ();
		movOffAudio.loop = false;
		movOffAudio.playOnAwake = false;
		movOffClip = Resources.Load<AudioClip> ("Audio/Special Blocks/Movable - Put Down");
		movOffAudio.clip = movOffClip;
	}

	void Update(){
		getHeight ();

		if (conflict) {
			conflictAudio.Play ();
			conflict = false;
		}
	}

	private void getHeight (){
		int tallest = 0;
		foreach (Square s in squares) {
			if ((int)s.getPosition ().y > tallest) {
				tallest = (int)s.getPosition ().y;
			}
		}
		height = tallest;
	}

	public int getColor(int type){
		if (type <= 1) {
			return Random.Range (0, 3);
		} 
		return 3;
	}

	////-normal, 1-movable, 2-erase, 3-bomb, 4-rainbow, 5-shape
	public int getSquareType(){		
		int r = Random.Range(0,10);
		return q [r];
	}


	public void initQueue(){
		queue = new Queue<Square> ();
		//initialize first 3 blocks
		Square s1 = addSquare(qpos1, false);
		Square s2 = addSquare (qpos2, false);
		Square s3 = addSquare (qpos3, false);

		queue.Enqueue (s1);
		queue.Enqueue (s2);
		queue.Enqueue (s3);

	}

	//dequeues square and places it at pos, then updates queue
	public void placeSquare(Vector2 pos){
		if (checkBounds (pos)) {		//check to make sure clicking in the board
			Square atPos = board [(int)pos.x, (int)pos.y];
			Square above = null;
			if (pos.y + 1 < BOARDSIZEY) {
				above = board [(int)pos.x, (int)pos.y + 1];
			}
			if (atPos == null && (above == null || !above.isFalling())) {			//check if clicking on an existing block
				if (moving == null) {			//if not moving a movable block, try to place from queue
					bool place = true;
					Square next = queue.Peek();
					if (next.isAnchor ()) {
						place = false;
						switch(next.rigid.shapeType){
							case 0: // _
								if (next.rigid.checkValidGrow (pos, 0, 5)) {	//TODO: figure out for different shapes 
									place = true;
								}
								break;
							case 1: // l
								if (next.rigid.checkValidGrow (pos, 5, 0)) {
									place = true;
								}
								break;
							default:
								place = false;
								break;
						}
					}

					if (place) {
						
						Square square = queue.Dequeue ();
						if (!square.isAnchor ()) {
							square.setFalling (true);
						}
						square.setPosition (pos);
						board [(int)pos.x, (int)pos.y] = square;
						updateQueue ();
						if (square.isAnchor ()) {
							// do rigid stuff
							square.rigid.grow ();

						} else {
							// Place it like normal if it's not an anchor

						}
					}

				} else {						//if placing a movable block, place the moving block
					Vector2 oldpos = moving.getPosition ();
					board [(int)oldpos.x, (int)oldpos.y] = null;
					moving.setPosition (pos);
					moving.setModelColor (2f);
					board [(int)pos.x, (int)pos.y] = moving;
//					moving.setFalling (true);
					moving.wait = true;
//					moving.checkConflicts();
					movOffAudio.Play ();
					moving = null;
					chainSettle (oldpos);


				}
			} else {		//if clicking on an existing block
				if (atPos != null) {
					clickOnBlock (atPos, pos);
				}
			}
		} else {
			print ("nO");
		}
	}
		

	//performs actions when clicking on an existing block depending on the block type
	public void clickOnBlock(Square atPos, Vector2 pos){
		if (!atPos.isFalling()) {
			if (atPos.getType () == 1) {		// if it's a movable block, move it.
				if (atPos != moving) {
					if (moving != null) {
						moving.setModelColor (2);
					}
					moving = atPos;
					moving.setModelColor (.5f);
					movOnAudio.Play ();
				} else {					// if you click on the moving block again, drop it
					moving.setModelColor (2f);
					moving = null;
					movOffAudio.Play ();
				}
			} else {
				Square next = queue.Peek ();		//if next block in the queue is eraser,
				if (next.getType () == 2) {			//erase the block clicked on
					queue.Dequeue ().destroy ();
					atPos.destroy ();
					updateQueue ();
					chainSettle (pos);
				}
			}
		}
	}


	public void activate(Square s){
		int type = s.getType ();
		Vector2 pos = s.getPosition ();
		Square below = board [(int)pos.x, (int)pos.y - 1];
		if (below != null) {
			if (type == 2) {			//erase one below
				
				if (!below.isGround ()) {
					below.destroy ();
				}
				s.destroy ();
			} else if (type == 3) {			//booomb
				for (int i = -1; i < 2; i++) {
					for (int j = 1; j > -2; j--) {
						int newx = (int)pos.x + i;
						int newy = (int)pos.y + j;
						if (newx < BOARDSIZEX && newx >= 0 && newy < BOARDSIZEY && newy >= 0) {
							Square neighbor = board [newx, newy];
							if (neighbor != null && !neighbor.isGround ()) {
								neighbor.destroy ();
								chainSettle (new Vector2 (newx, newy));
							}
						}
					}
				}

			}
		}


	}
	//checks to see if the square above pos needs to be settled
	public void chainSettle(Vector2 pos){
		Square s = board [(int)pos.x, (int)pos.y];
//		print (s + " is chain settling" + s.isFalling());
		if (pos.y < BOARDSIZEY - 1) {
			for (int i = 1; i < BOARDSIZEY - pos.y; i++) {
				Square above = board [(int)pos.x, (int)pos.y + 1];
				if (above != null) {
					if (above.rigid != null ) {
//						above.rigid.settleShape ();
						above.rigid.setShapeFalling (true);
					} else {
//						print ("Setting " + above + "to falling");
						above.setFalling (true);
					}
				}
			}
		}
	}

	public void resolveConflict (Square s, Square c){
		Vector2 sPos = s.getPosition ();
		Vector2 cPos = c.getPosition ();
		board [(int)sPos.x, (int)sPos.y] = null;
		board [(int)cPos.x, (int)cPos.y] = null;
		if (c.rigid != null) {
//			print ("breaking " + c.rigid);
			breakShape (c.rigid);
		}
		if (s.rigid != null) {
			//			print ("breaking " + c.rigid);
			breakShape (s.rigid);
		}

		Destroy (c.gameObject);

		s = board [(int)sPos.x, (int)(sPos.y + 1)];
		c = board [(int)cPos.x, (int)(cPos.y + 1)];
		resolveConflictHelper (s);
		resolveConflictHelper (c);
	}

	public void resolveConflictHelper(Square s){
		while (s != null) {
			Vector2 sPos = s.getPosition ();
			Square sAbove = board [(int)sPos.x, (int)(sPos.y + 1)];
			s = sAbove;
		}
	}


	public bool checkBounds(Vector2 pos){
		if ((pos.x >= 0 && pos.x < BOARDSIZEX) && (pos.y >= 0 && pos.y < BOARDSIZEY)) {
			return true;
		}
		return false;

	}

	//change positions of the blocks in the queue, create a new block at position 3
	public void updateQueue(){
		Queue<Square> newq = new Queue<Square> ();
		while (queue.Count > 0) {
			Square s = queue.Dequeue ();
			s.setPosition (new Vector2(s.getPosition ().x - 1f, queueY));
			newq.Enqueue (s);
		}
		queue = newq;
		Square end = addSquare (qpos3, false);
		queue.Enqueue (end);

	}


	public Square addSquare(Vector2 pos, bool isGround){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = squareFolder.transform;
		square.transform.position = new Vector3 (pos.x, pos.y, 0);
		if (isGround) {
			square.init (pos, -1, true);
		} else {
			int type = getSquareType ();
			//hardcode normal square for first square
			/*if (firstSquare) {
				type = 0;
				firstSquare = false;
			}*/
			square.init (pos, getColor (type), false, type);
			square.addSqman (this);

			if (type == 5) {
				RigidShape rs = makeRigidShape(square);
			}
		}

		squares.Add (square);
		square.name = "Square " + squares.Count;

		return square;

	}

	public RigidShape makeRigidShape(Square square){
		GameObject rsObject = new GameObject ();
		RigidShape rs = rsObject.AddComponent<RigidShape> ();
		rs.transform.parent = rsFolder.transform;
		rs.transform.position = square.transform.position;
		rigidshapes.Add (rs);
		rs.name = "Rigid Shape " + rigidshapes.Count;
		rs.init (square, board, this); // initeedededddd
		return rs;
	}

	public void breakShape(RigidShape rs){
		print ("Breaking " + rs);
		Square above;
		foreach (Square s in rs.getSquares()) {
			above = null;
			if (s != null) {
				s.anchor = false;
				s.rigid = null;
				s.setFalling (true);
				Vector2 spos = s.getPosition ();
				if (spos.y + 1 < BOARDSIZEY) {
					above = board [(int)spos.x, (int)spos.y + 1];
				}
				if (above != null) {
					if (above.rigid != null) {
						print ("A RIGID SHAPE: " + above + " " + above.rigid);
						if (above.rigid != this) {
							print (above + " " + above.rigid + " shape being made to fall");
							//					above.rigid.settleShape ();
							above.rigid.setShapeFalling (true);
						}
					}else {
						above.setFalling (true);
					}
				}

				//				s.counter = 0;
				//				sqman.chainSettle (s.pos); ???? when shapes break, need to settle above appropriately
				//				StartCoroutine (settleSquare (s));
			}
		}
		Destroy (rs);
	}

	public void breakShapes(LinkedList<RigidShape> shapes){
		foreach (RigidShape rs in shapes) {
			if (rs != null) {
				breakShape (rs);
			}
		}
	}

	public bool boardSolved() {
		bool solved;
		// Short circuit, because if the last column's block 1 below the destination isn't a square, then there's no point running the alg.
		if (destinationClose() && pathValid(beginning)) {
			solved = true;

			playSuccess ();
		} else {
			solved = false;
		}
		Debug.Log ("Did you solve the board? : " + solved);
		return solved;
	}

	private void playSuccess(){
		gm.success = true;
		successAudio.Play ();
	}

	private bool destinationClose() {
		bool ret;
		if (board[BOARDSIZEX - 1, (int) destination.getPosition().y - 1] != null) {
			ret = true;
		} else {
			ret = false;
		}

		print ("destinationClose turned out to be " + ret);
		return ret;
	}

	private bool pathValid(Square curSquare) {
		if (curSquare != null) {
			Vector2 pos = curSquare.getPosition ();
			Debug.Log ("Got a square @: " + pos.x + ", " + pos.y + ")");
			if (curSquare.getPosition() == destination.getPosition()) {
				Debug.Log("Destination's position:" + destination.getPosition().x + ", " + destination.getPosition().y);
				return true;
			} else {
				return pathValid (getNextSquare(curSquare));
			}
		}
		return false;
	}

	private Square getNextSquare(Square sq) {
		// If the square is the last one in the column, return
		if (sq == board[BOARDSIZEX - 1, (int) destination.getPosition().y - 1]) {
			Debug.Log (sq);
			StartCoroutine(sq.tempHighlight ());
			return destination;
		}
		Square twoUp = board [(int) sq.getPosition ().x + 1, (int) sq.getPosition ().y + 1];
		Square[] possibleNextSquares = new Square[3];

		for (int i = 0; i <= 2; i++) {
			getNextPossibleSquare(i, possibleNextSquares, sq);
			Debug.Log ("pns @ " + i + ": " + possibleNextSquares[i]);
		}

		// Check the cases of these!

		// First, return null if we don't have a clearance of two spaces!

		// TODO Temporarily disable this for the test boards lol
//		bool clear = (twoUp == null);
//		if (!clear) {
//			// Don't meet clearance, return that fact.
//			return null;
//		}

		// Check next possible squares to touch
		foreach (Square possibleNext in possibleNextSquares) {
			if (possibleNext != null) {
				Debug.Log ("Getting square @::: " + possibleNext.getPosition().x + ", " + possibleNext.getPosition().y);
				StartCoroutine(sq.tempHighlight ());
//				possibleNext.printColor ();
				return possibleNext;
			}
		}

		// If we make it here, return null to indicate that there's more than a two difference
		// Note, it's downward here!
		return null;
	}

	private void getNextPossibleSquare(int i, Square[] possibleNextSquares, Square sq) {
		int y_inc = 0;
		switch (i) {
		case 0:
			y_inc = 1;
			break;
		case 1:
			y_inc = 0;
			break;
		case 2:
			y_inc = -1;
			break;
		default:
			print ("Damn, you fool! Inaccessible possible square.");
			break;
		}

		if (!((y_inc == -1 && sq.getPosition().y <= 0) | sq.getPosition().y >= BOARDSIZEY)) {
			possibleNextSquares[i] = board [(int) sq.getPosition ().x + 1, (int) sq.getPosition ().y + y_inc];
//			Debug.Log ("Y_inc loop: letting square @: " + possibleNextSquares[i].getPosition().x + ", " + possibleNextSquares[i].getPosition().y);
		}
		else {
			Debug.Log("Square wasn't on the board!");
		}
	}

	// -------------
	// All GUI code down here, basically just because lol
	void OnGUI() {
		if (GUI.Button(new Rect(30, 30, 100, 40), "Test your path.")) {
			boardSolved ();
		}
		if (GUI.Button (new Rect (Screen.width-160, 30, 100, 40), "Menu")) {
			Application.LoadLevel (Application.loadedLevel);

		}
	}



}

