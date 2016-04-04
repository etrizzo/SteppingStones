using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareManager : MonoBehaviour {
	
	GameObject squareFolder;
	List<Square> squares;

	Queue<Square> queue;			// Add is enqueue, RemoveAt(0) is dequeue
	static int BOARDSIZEX = 24;
	static int BOARDSIZEY = 16;
	static int queueY = -2;
	Vector2 qpos1 = new Vector2(-5f, (float) queueY);
	Vector2 qpos2 = new Vector2(-4f, (float) queueY);
	Vector2 qpos3 = new Vector2(-3f, (float) queueY);
	Square[,] board;


	public void init(){
		squareFolder = new GameObject();
		squares = new List<Square> ();
		initQueue ();		//initialize queue w/ 3 initial blocks
		initBoard();
	}


	public int getColor(){
		return Random.Range (0, 3);
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
		if (checkBounds (pos) && board [(int) pos.x, (int)pos.y] == null) {
			Square square = queue.Dequeue ();
			square.setPosition (pos);
			board [(int)pos.x, (int)pos.y] = square;
			updateQueue ();
		} else {
			print ("nO");
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
			square.init (pos, getColor (), false);
		}

		squares.Add (square);
		square.name = "Square " + squares.Count;

		return square;

	}
}

