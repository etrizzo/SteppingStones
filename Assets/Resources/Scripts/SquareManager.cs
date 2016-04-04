using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareManager : MonoBehaviour {
	
	GameObject squareFolder;
	List<Square> squares;

	Queue<Square> queue;			// Add is enqueue, RemoveAt(0) is dequeue
	static int BOARDSIZE = 5;
	static int queueY = BOARDSIZE * -1;
	Vector2 qpos1 = new Vector2((BOARDSIZE-4) * -1, (float) queueY);
	Vector2 qpos2 = new Vector2((BOARDSIZE-3) * -1, (float) queueY);
	Vector2 qpos3 = new Vector2((BOARDSIZE-2) * -1, (float) queueY);
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
		board = new Square[BOARDSIZE, BOARDSIZE];
		//initialize level w/ ground squares (read from text file?)

	}

	//dequeues square and places it at pos, then updates queue
	public void placeSquare(Vector2 pos){
		Square square = queue.Dequeue ();
		square.setPosition (pos);
		updateQueue();

	}

	//change positions of the blocks in the queue, create a new block at position 3
	public void updateQueue(){
		Queue<Square> newq = new Queue<Square> ();
		while (queue.Count > 0) {
			Square s = queue.Dequeue ();
			s.setPosition (new Vector2(s.getPosition ().x + 1f, queueY));
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
		square.init (pos, getColor (), isGround);

		squares.Add (square);
		square.name = "Square " + squares.Count;

		return square;

	}
}

