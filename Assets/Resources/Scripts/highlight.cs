using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highlight : MonoBehaviour {
	Vector3 worldPos;
	Vector2 mouse;
	Vector2 lastMousePos;
	int mousex;
	int mousey;
	SquareManager sqman;
	Queue<Square> queue;
	SquareModel model;

	Square made;
	Square[,] board;

	// Save our color here so it's consistent :)
	Color color = Color.gray;

	public void init(Queue<Square> q, SquareManager sqm) {
		sqman = sqm;
		board = sqman.board;
		lastMousePos = new Vector2 (-1, 1);
		queue = q;

		// Init made actually lolololol
		made = sqman.addSquare(new Vector2(mouse.x, mouse.y), false);
		made.model.mat.color = color;
		made.setType (0);

		updateMouse ();
		updateModel ();

		//
	}

	public void Update() {
		updateMouse ();
		// get first element in queue & change based on that
		updateModel ();
	}

	void updateMouse() {
		worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mouse.x = (int)Mathf.Floor (worldPos.x);
		mouse.y = (int)Mathf.Ceil (worldPos.y);

	}

	void updateModel() {
		int lowestY = getLowestYCoord ();
		// If we're over the board do the things
		Debug.Log("Lowest y coordinate is: " + lowestY);
		if (lowestY != -1) {
			made.model.mat.color = color;
			if (mouseMoved()) {
				// If the mouse moved, update the position of the anchor
				made.transform.position = new Vector3(mouse.x, lowestY, 0);
//				Debug.Log ("Got the queue, it looks like: " + queue.Count);
				Square next = sqman.queue.Peek ();
				int type = next.getType ();
//				Debug.Log ("Next queued block is of type: " + type);
				if (type == 5) {
					// If it's a RigidShape, do extra things based on what kind of shape it is!
					RigidShape rs = next.rigid;
					int shapeType = rs.shapeType;
					if (shapeType == 0) {
						// it's _, spawn some additional shapes!

					} else {
						// it's I vertical!
					}
					// We know this is an anchor

				}

			}
		} else {
			// If we're **not over the board**, make the square transparent lol
			made.model.mat.color = Color.clear;
		}
		lastMousePos = new Vector2((int)mouse.x, (int) mouse.y);
	}

	private bool mouseMoved() {
		// This should actually check if it moved over 1 square or more since last update :/
		return lastMousePos == mouse;
	}

	private int getLowestYCoord() {
		int ret = -1;
		int highestBlockY = sqman.BOARDSIZEY - 1;
		while (board[(int) mouse.x, highestBlockY] == null) {
			highestBlockY--;
		}
		int low = (int) mouse.y;
		if (highestBlockY < mouse.y) {
			highestBlockY = (int) mouse.y;
			low = (int) 0;
		}

		Debug.Log ("---------------------");
		Debug.Log ("highest block rn is:" + highestBlockY);
		Debug.Log ("---------------------");
		// TODO: Should check for mouse.x being on the board first, to prevent null exceptions???
		for (int y = highestBlockY; y >= low; y--) {
			// Check all squares below your mouse
			Debug.Log ("Checking the y coord:" + y);
			// Specifically, check to see if this one **doesn't have something on top of it lol
			if (board[(int) mouse.x, y] != null && board[(int) mouse.x, y + 1] == null) {
				// Return the y **above** the lowest :)
				ret = y + 1;
			} else {
				Debug.Log (y + "doesn't have a block :/");
			}
		}
		// If we exit that loop, we're not above the board, sooooo return -1
		return ret;
	}
}
