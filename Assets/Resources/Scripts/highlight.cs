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
	int EXTRASHAPES = 4;
	Square[] rigidShapes;
	int rigidYOffset = -10;

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
		made.model.gameObject.name = "Highlight Square";

		rigidShapes = new Square[EXTRASHAPES];

		// Make the rigid blocks even if we don't need them lol
		for (int i = 0; i < EXTRASHAPES; i++) {
			// TODO: Need to replace addSquare with a addHighlightSquare method :))))
			rigidShapes[i] = sqman.addSquare(new Vector2((int) mouse.x + i + 1, rigidYOffset), false);
		}


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
//		Debug.Log("Lowest y coordinate is: " + lowestY);
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
						for (int i = 0; i <= EXTRASHAPES; i++) {
							rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, lowestY, 0);
							// TODO: Need to replace add
//							rigidShapes[i] = sqman.addSquare(new Vector2((int) mouse.x + i + 1, lowestY), false);
						}
					} else {
						// it's I vertical!
					}
					// We know this is an anchor

				} else {
					// Reset the anchor highlight blocks to be off the screen when we don't need them!
					for (int i = 0; i <= EXTRASHAPES; i++) {
						rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, rigidYOffset, 0);
						// TODO: Need to replace add
					}
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
		int highestBlockY = (int) mouse.y;

		// Search down from our mouse.y for the first not null thing
		while (squareAtYCoord(highestBlockY) == null && highestBlockY >= 0) {
			highestBlockY--;
		}

		// If the highest block is below us then we're good, because we're just going to return this anyway lol
		// But if it's equal to where we started, we got a problem, sooooo
		if (highestBlockY == (int) mouse.y) {
			// ... check, going up from the current block, for the first null block to put it in
			while (squareAtYCoord(highestBlockY) != null && highestBlockY <= sqman.BOARDSIZEY - 1) {
				highestBlockY++;
			}
			return highestBlockY;
		} else {
			return highestBlockY + 1;
		}

		return -1;
	}

	private Square squareAtYCoord(int y) {
		return board[(int) mouse.x, y];
	}
}
