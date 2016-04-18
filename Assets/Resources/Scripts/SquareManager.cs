using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareManager : MonoBehaviour {
	
	public GameObject squareFolder;
	public List<Square> squares;
	public GameObject rsFolder;
	public List<RigidShape> rigidshapes;
	public AudioSource settleAudio;
	public AudioClip settleClip;
	public AudioSource conflictAudio;
	public AudioClip conflictClip;
	public AudioSource movOnAudio;
	public AudioClip movOnClip;
	public AudioSource movOffAudio;
	public AudioClip movOffClip;

	Queue<Square> queue;			// Add is enqueue, RemoveAt(0) is dequeue
	public int BOARDSIZEX = 24;
	public int BOARDSIZEY = 16;
	static int queueY = -1;
	Vector2 qpos1 = new Vector2(-4f, (float) queueY);
	Vector2 qpos2 = new Vector2(-3f, (float) queueY);
	Vector2 qpos3 = new Vector2(-2f, (float) queueY);
	Square[,] board;
	int[] q;
	public int[] rsq;

	float counter = 0f;
	public Square moving = null;
	public bool firstSquare = true;
	public Square destination;		//TODO: TEMPORARY :O
	public Square beginning;		//TODO: TEMPORARY :O

//	float randFreq = .2;


	public void init(Square[,] board, int[] q, int[] rsq = null){
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
	}

	void initSound(){
		settleAudio = this.gameObject.AddComponent<AudioSource> ();
		settleAudio.loop = false;
		settleAudio.playOnAwake = false;
		settleAudio.time = 1.0f;
		settleClip = Resources.Load<AudioClip> ("Audio/Blocks Settle");
		settleAudio.clip = settleClip;

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



	public Square makeExtremeSquare(string type){
		int x = 0;
		if (type == "beginning") {
			x = -1;
		} else if (type == "destination") {
			x = BOARDSIZEX;
		}
		int y = Random.Range (4, BOARDSIZEY);

		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

//		square.transform.parent = squareFolder.transform;
		square.transform.position = new Vector3 (x, y, 0);
		square.init(new Vector2((float) x, (float) y), 4, false);

		square.name = type;

		return square;
	}




	//dequeues square and places it at pos, then updates queue
	public void placeSquare(Vector2 pos){
		if (checkBounds (pos)) {		//check to make sure clicking in the board
			Square atPos = board [(int)pos.x, (int)pos.y];
			Square above = board [(int)pos.x, (int)pos.y + 1];
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
						square.setFalling (true);
						square.setPosition (pos);
						board [(int)pos.x, (int)pos.y] = square;
						updateQueue ();
						if (square.isAnchor ()) {
							// do rigid stuff
							square.rigid.grow ();

						} else {
							// Place it like normal if it's not an anchor
							StartCoroutine (settleSquare (square));
						}
					}

				} else {						//if placing a movable block, place the moving block
					Vector2 oldpos = moving.getPosition ();
					board [(int)oldpos.x, (int)oldpos.y] = null;
					moving.setPosition (pos);
					moving.setModelColor (2f);
					board [(int)pos.x, (int)pos.y] = moving;
					movOffAudio.Play ();
					StartCoroutine(settleSquare (moving));
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
		if (pos.y < BOARDSIZEY - 1) {
			Square above = board [(int)pos.x, (int)pos.y + 1];
			if (above != null) {
				StartCoroutine(settleSquare (above));
			}
		}

	}

	IEnumerator settleSquare(Square s){
		//Debug.Log("Settling square!");
		if (s.rigid == null) {
			Vector2 pos = s.getPosition ();
			Square below = board [(int)pos.x, (int)(pos.y - 1)];
			Square above = null;
			if (pos.y < BOARDSIZEY - 1) {
				above = board [(int)pos.x, (int)(pos.y + 1)];
			}

				
			if (below == null) {
				yield return new WaitForSeconds (.5f);
				counter = 0f;
				board [(int)pos.x, (int)pos.y] = null;
				board [(int)pos.x, (int)pos.y - 1] = s;
				s.setPosition (new Vector2 (pos.x, pos.y - 1));
				if (above != null) {
					StartCoroutine (settleSquare (above));
				}
				//("Square is at: " + s.getPosition ());
				StartCoroutine (settleSquare (s));
			} else {
				if (below != null && below.isFalling ()) {
					yield return new WaitForSeconds (.25f);
					StartCoroutine (settleSquare (s));
				} else {
					//Debug.Log("Checking conflicts!");
//					print(s + " has landed on " + below);
					s.setFalling(false);
					settleAudio.Play ();
					StartCoroutine (checkConflicts (s));
				}
			}
		} else {
			StartCoroutine(s.rigid.settleShape ());
		}
	}

	public IEnumerator checkConflicts(Square s){
		yield return new WaitForSeconds (.25f);
		Vector2 pos = s.getPosition ();
		Square below = null;
		Square left = null;
		Square right = null;
		Square above = null;
		if(!(pos.y-1 < 0)){
			below = board [(int)pos.x, (int)(pos.y - 1)];
		}
		if (!(pos.x - 1 < 0)) {
			left = board [(int)(pos.x - 1), (int)pos.y];
		}
		if(!(pos.x + 1 >= BOARDSIZEX)){
			right = board [(int)(pos.x+1), (int)pos.y];
		}
		if(!(pos.y +1 >= BOARDSIZEY)){
			above = board [(int)(pos.x), (int)pos.y+1];
		}
		bool conflict = false;
		if (below != null && below.getColor () == s.getColor ()) {
			conflict = true;
			conflictAudio.Play();
			resolveConflict (s, below);
		}
		if (left != null && left.getColor () == s.getColor ()) {
			conflict = true;
			conflictAudio.Play();
			resolveConflict (s, left);
		}
		if (right != null && right.getColor () == s.getColor ()) {
			conflict = true;
			conflictAudio.Play();
			resolveConflict (s, right);
		}
		if (above != null && above.getColor () == s.getColor ()) {
			conflict = true;
			conflictAudio.Play();
			resolveConflict (s, above);
		}
		if (s.getType () > 1) {
			activate (s);
		}
		if (conflict) {
			Destroy (s.gameObject);
			yield return new WaitForSeconds (.25f);
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
			StartCoroutine(settleSquare (s));
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
			if (firstSquare) {
				type = 0;
				firstSquare = false;
			}
			square.init (pos, getColor (type), false, type);

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
		foreach (Square s in rs.getSquares()) {
			if (s != null) {
				s.anchor = false;
				s.rigid = null;
				s.setFalling (true);
				StartCoroutine (settleSquare (s));
			}
		}
		DestroyImmediate (rs);
	}

	public bool boardSolved() {
		bool solved;
		// Short circuit, because if the last column's block 1 below the destination isn't a square, then there's no point running the alg.
		if (destinationClose() && pathValid()) {
			solved = true;
		} else {
			solved = false;
		}
		Debug.Log ("Did you solve the board? : " + solved);
		return solved;
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

	private bool pathValid() {
		// TODO: Lol this is not real code! Validate paths
		if (true) {
			return true;
		} else {
			return false;
		}
	}

	private Square getNextSquare(Square sq) {
		Square twoUp = board [(int) sq.getPosition ().x + 1, (int) sq.getPosition ().y + 1];
		Square[] possibleNextSquares = new Square[3];
		possibleNextSquares[0] = board [(int) sq.getPosition ().x + 1, (int) sq.getPosition ().y - 1];
		possibleNextSquares[1] = board [(int) sq.getPosition ().x + 1, (int) sq.getPosition ().y];
		possibleNextSquares[2] = board [(int) sq.getPosition ().x + 1, (int) sq.getPosition ().y + 1];

		// Check the cases of these!

		// First, return false if we don't have a clearance of two spaces!
		bool clear = twoUp == null;
		if (!clear) {
			// Don't meet clearance, return that fact.
			return null;
		}

		// Check next possible squares to touch
		foreach (Square possibleNext in possibleNextSquares) {
			if (possibleNext != null) {
				return possibleNext;
			}
		}

		// If we make it here, return null to indicate that there's more than a two difference
		return null;
	}

	// -------------
	// All GUI code down here, basically just because lol
	void OnGUI() {
		if (GUI.Button(new Rect(30, 30, 100, 40), "Test your path.")) {
			boardSolved ();
		}

	}

}

