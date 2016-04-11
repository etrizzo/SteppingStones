using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareManager : MonoBehaviour {
	
	public GameObject squareFolder;
	public List<Square> squares;
	public GameObject rsFolder;
	public List<RigidShape> rigidshapes;

	Queue<Square> queue;			// Add is enqueue, RemoveAt(0) is dequeue
	static int BOARDSIZEX = 24;
	static int BOARDSIZEY = 16;
	static int queueY = -2;
	Vector2 qpos1 = new Vector2(-5f, (float) queueY);
	Vector2 qpos2 = new Vector2(-4f, (float) queueY);
	Vector2 qpos3 = new Vector2(-3f, (float) queueY);
	Square[,] board;

	float counter = 0f;
	public Square moving = null;
	public bool firstSquare = true;
//	float randFreq = .2;


	public void init(){
		squareFolder = new GameObject();
		squareFolder.name = "Squares";
		squares = new List<Square> ();
		rsFolder = new GameObject ();
		rsFolder.name = "Rigid Shapes";
		rigidshapes = new List<RigidShape> ();
		initBoard();
		initQueue ();		//initialize queue w/ 3 initial blocks

	}


	public int getColor(int type){
		if (type <= 1) {
			return Random.Range (0, 3);
		} 
		return 3;
	}

	public int getSquareType(){
		float r = Random.value;
		if (r < .4) {
//			return (int)Mathf.Floor (r * 10);
			return 5;
		} 
		return 0;
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

	public void initBoard(){
		board = new Square[BOARDSIZEX, BOARDSIZEY];
		//initialize level w/ ground squares (read from text file?)
		for (int i = 0; i < BOARDSIZEX; i++) {
			Square s = addSquare (new Vector2(i,0), true);
//			s.init (new Vector2 (i, 0), -1, true);
			board [i, 0] = s;

		}
	}

	//dequeues square and places it at pos, then updates queue
	public void placeSquare(Vector2 pos){
		if (checkBounds (pos)) {		//check to make sure clicking in the board
				Square atPos = board [(int)pos.x, (int)pos.y];
			if (atPos == null) {			//check if clicking on an existing block
				if (moving == null) {			//if not moving a movable block, try to place from queue
					Square square = queue.Dequeue ();
					square.setPosition (pos);
					board [(int)pos.x, (int)pos.y] = square;
					updateQueue ();
					if (square.isAnchor ()) {
						// do rigid stuff
						square.rigid.grow ();
						StartCoroutine (square.rigid.settleShape ());
					} else {
						// Place it like normal if it's not an anchor
						StartCoroutine (settleSquare (square));
					}

				} else {						//if placing a movable block, place the moving block
					Vector2 oldpos = moving.getPosition ();
					board [(int)oldpos.x, (int)oldpos.y] = null;
					moving.setPosition (pos);
					moving.setModelColor (2f);
					board [(int)pos.x, (int)pos.y] = moving;
					StartCoroutine(settleSquare (moving));
					moving = null;
					chainSettle (oldpos);


				}
			} else {		//if clicking on an existing block
				clickOnBlock (atPos, pos);
			}
		} else {
			print ("nO");
		}
	}


	//performs actions when clicking on an existing block depending on the block type
	public void clickOnBlock(Square atPos, Vector2 pos){
		if (atPos.getType () == 1) {		// if it's a movable block, move it.
			if (atPos != moving) {
				if (moving != null) {
					moving.setModelColor (2);
				}
				moving = atPos;
				moving.setModelColor (.5f);
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
		yield return new WaitForSeconds (.25f);
		Vector2 pos = s.getPosition ();
		Square below = board [(int)pos.x, (int)(pos.y - 1)];
		Square above = null;
		if (pos.y < BOARDSIZEY - 1) {
			above = board [(int)pos.x, (int)(pos.y + 1)];
		}
		if (below == null) {
			counter = 0f;
			board [(int)pos.x, (int)pos.y] = null;
			board [(int)pos.x, (int)pos.y - 1] = s;
			s.setPosition (new Vector2 (pos.x, pos.y - 1));
			if (above != null) {
				StartCoroutine(settleSquare (above));
			}
			yield return new WaitForSeconds (.25f);
			//("Square is at: " + s.getPosition ());
			StartCoroutine (settleSquare (s));
		} else {
			//Debug.Log("Checking conflicts!");
			checkConflicts (s);
			if (s.getType () > 1) {
				activate (s);
			}
		}
	}

	public void checkConflicts(Square s){
		Vector2 pos = s.getPosition ();
		Square below = board [(int)pos.x, (int)(pos.y - 1)];
		Square left = board [(int)(pos.x-1), (int)pos.y];
		Square right = board [(int)(pos.x+1), (int)pos.y];
		if (below != null && below.getColor () == s.getColor ()) {
			resolveConflict (s, below);
		}
		if (left != null && left.getColor () == s.getColor ()) {
			resolveConflict (s, left);
		}
		if (right != null && right.getColor () == s.getColor ()) {
			resolveConflict (s, right);
		}
	}

	public void resolveConflict (Square s, Square c){
		Vector2 sPos = s.getPosition ();
		Vector2 cPos = c.getPosition ();
		board [(int)sPos.x, (int)sPos.y] = null;
		board [(int)cPos.x, (int)cPos.y] = null;
		if (s.rigid != null) {
			print ("breaking " + s.rigid);
			breakShape (s.rigid);
		}
		Destroy (s.gameObject);
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
			checkConflicts (s);
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
			s.rigid = null;
			StartCoroutine(settleSquare (s));
		}
		DestroyImmediate (rs);
	}

}



