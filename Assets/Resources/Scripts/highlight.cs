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
	Color defaultColor = new Color(0.2F, 0.3F, 0.4F, 0.5F);
	Color redTransparent = new Color (1f, 0, 0, 0.5f);
	Color color;

	public Square addHighlightSquare(int x,int  y) {
//		Square sq = sqman.addSquare (new Vector2 (x, y), false);
		color = defaultColor;
		GameObject squareObject = new GameObject ();
		Square sq = squareObject.AddComponent<Square> ();
		sq.init (new Vector2 ((int)mouse.x, (int)mouse.y), 100, false, 0);
		sq.initSound ();
		sq.model.mat.color = color;
		sq.setType (0);
		sq.type = 0;
//		sq.model.transform.parent = sqman.squareFolder.transform;
		sq.model.name = "Highlight Square";
		sq.model.gameObject.name = "Highlight Square Model";
		return sq;
	}

	public void init(Queue<Square> q, SquareManager sqm) {
		sqman = sqm;
		board = sqman.board;
		lastMousePos = new Vector2 (-1, 1);
		queue = q;

		// Init made actually lolololol
		made = addHighlightSquare ((int) mouse.x, (int) mouse.y);


		rigidShapes = new Square[EXTRASHAPES];

		// Make the rigid blocks even if we don't need them lol
		for (int i = 0; i < EXTRASHAPES; i++) {
			// TODO: Need to replace addSquare with a addHighlightSquare method :))))
			rigidShapes[i] = addHighlightSquare((int) mouse.x + i + 1, rigidYOffset);
		}


		updateMouse ();
		updateModel ();

		//
	}

	public void clear(){
		foreach (Square s in rigidShapes) {
			if (s != null) {
				Destroy (s.gameObject);
			}
		}
		Destroy (made.gameObject);
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
//		color = checkHighlightConflicts ();
		made.transform.position = new Vector3 (mouse.x, mouse.y, -1);
//		checkConflictAndColor (made);
		Square next = sqman.queue.Peek ();
		int type = next.getType ();

		if (type == 5) {
//			if (color == defaultColor) {
//			}
			// Maybe make a delegate to run all of this code for all the 4 blocks????
//			for (int i = 0; i <= EXTRASHAPES; i++) {
//				rigidShapes[i].model.mat.color = color;
//			}
			// If it's a RigidShape, do extra things based on what kind of shape it is!
			RigidShape rs = next.rigid;
			int shapeType = rs.shapeType;
			if (shapeType == 0) {
				// it's _, spawn some additional shapes!
				for (int i = 0; i < EXTRASHAPES; i++) {
//					rigidShapes[i].model.mat.color = color;
					rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, mouse.y, -1);
				}
			} else {
				// it's I vertical!
				for (int i = 0; i < EXTRASHAPES; i++) {
					rigidShapes [i].transform.position = new Vector3 (mouse.x, mouse.y + i + 1, -1);
				}
			}
			// Color all the extra shapes :)
			for (int i = 0; i < EXTRASHAPES; i++) {
				//Debug.Log ("Checking extra shape: " + i);
//				checkConflictAndColor(rigidShapes[i]);
			}

		} else {
			for (int i = 0; i < EXTRASHAPES; i++) {
				rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, rigidYOffset, -1);
//				rigidShapes[i].model.mat.color = Color.clear;
				// TODO: Need to replace add
			}
		}

		made.model.mat.color = getHighlightColor ();
		for (int i = 0; i < EXTRASHAPES; i++) {
			rigidShapes [i].model.mat.color = getHighlightColor ();
		}
	}

	bool insideBoard(float pos_x, float pos_y) {
		return (pos_x >= 0) && (pos_x < sqman.BOARDSIZEX) && (pos_y > 0) && (pos_y < sqman.BOARDSIZEY);
	}

	bool movableBlock(float pos_x, float pos_y) {
		return board [(int)pos_x, (int)pos_y].type == 1;
	}


	Color getHighlightColor () {
		Color retColor = defaultColor;
		Square next = sqman.queue.Peek ();

//		Debug.Log ("getHighlightColor is firing???");
		// Check the mouse conflict
		Vector3 madePos = made.transform.position;
		if (insideBoard(madePos.x, madePos.y)) {
			if (board [(int)madePos.x, (int)madePos.y] != null && !movableBlock(madePos.x, madePos.y)) {
				//			Debug.Log ("There's a block at " + madePos.x + ", " + madePos.y + ", so I'm coloring this red!");
				retColor = redTransparent;
			}
		} else {
			//Debug.Log ("Outside of bounds, coloring it red!");
			retColor = redTransparent;
		}
		if (next.type == 5) {
			for (int i = 0; i < EXTRASHAPES; i++) {
				Vector2 pos = rigidShapes [i].transform.position;

				if (insideBoard (pos.x, pos.y)) {
					if (board [(int)pos.x, (int)pos.y] != null && !movableBlock(madePos.x, madePos.y)) {
						//			Debug.Log ("There's a block at " + madePos.x + ", " + madePos.y + ", so I'm coloring this red!");
//						sqman.badPlaceAudio.Play ();
						retColor = redTransparent;
					}
				} else {
//					Debug.Log ("Outside of bounds, coloring it red!");
//					sqman.badPlaceAudio.Play ();
					retColor = redTransparent;
				}
			}
		}
		if (retColor != redTransparent) {
			Debug.Log (("I'm not coloring this block red!"));
		}
		return retColor;
	}

//	void checkConflictAndColor(Square sq) {
//		// TODO: Also need to check if the square is off the board, and highlight it red there too!
//		Color theColor = defaultColor;
//		Vector2 pos = new Vector2(sq.transform.position.x, sq.transform.position.y);
//		// If there's a block here, color this highlight block red!
//		Debug.Log ("checkCon is firing???");
//		if (board[(int) pos.x, (int) pos.y] != null) {
//			Debug.Log ("There's a block at " + pos.x + ", " + pos.y + ", so I'm coloring this red!");
//			theColor = redTransparent;
//		} else {
////			Debug.Log ("There's no block at " + pos.x + ", " + pos.y + "!");
////			theColor = defaultColor;
//		}
//
//	}

	void updateModel__defunct() {
//		Debug.Log ("Got the queue, it looks like: " + queue.Count);
		Square next = sqman.queue.Peek ();
		int type = next.getType ();

		int lowestY = getLowestYCoordForColumn ((int) mouse.x);

//		if (type == 5) {
//			// If it's a rigid shape, we have to get the lowest possible y for all the rigids
//			foreach (Square sq in rigidShapes) {
//				int this_y = (int) sq.getPosition ().y;
//				if (this_y > lowestY) {
//					Debug.Log ("Changing this_y to:" + this_y);
//					lowestY = this_y;
//					color = Color.red;
//				}
//			}
//		}

		// If we're over the board do the things
//		Debug.Log("Lowest y coordinate is: " + lowestY);
		if (lowestY != -1) {
			if (mouseMoved()) {
				// If the mouse moved, update the position of the anchor
				made.transform.position = new Vector3(mouse.x, lowestY, 0);
//				Debug.Log ("Next queued block is of type: " + type);
				if (type == 5) {
					// If it's a RigidShape, do extra things based on what kind of shape it is!
					RigidShape rs = next.rigid;
					int shapeType = rs.shapeType;
					if (shapeType == 0) {
						// it's _, spawn some additional shapes!
						for (int i = 0; i < EXTRASHAPES; i++) {
							rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, lowestY, 0);

//							foreach (Square sq in rigidShapes) {
//								int this_y = (int) sq.getPosition ().y;
//								// TODO: Okay, no, you actually have to check that this is occupied, and then get the y in this
//								// column :////////////
//								if (squareAtYCoord((int) mouse.x, this_y) != null) {
//									Debug.Log ("Changing this_y to:" + this_y);
//									lowestY = this_y;
//									color = Color.red;
//								}
//							}
//
//							rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, lowestY, 0);

							// TODO: Need to replace add
//							rigidShapes[i] = sqman.addSquare(new Vector2((int) mouse.x + i + 1, lowestY), false);
						}
					} else {
						// it's I vertical!
					}
					// We know this is an anchor

				} else {
					color = Color.gray;
					// Reset the anchor highlight blocks to be off the screen when we don't need them!
					for (int i = 0; i < EXTRASHAPES; i++) {
						rigidShapes [i].transform.position = new Vector3 (mouse.x + i + 1, rigidYOffset, 0);
						// TODO: Need to replace add
					}
				}
			made.model.mat.color = color;
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

	private int getLowestYCoordForColumn(int x) {
		int highestBlockY = (int) mouse.y;

		// Search down from our mouse.y for the first not null thing
		while (squareAtYCoord(x, highestBlockY) == null && highestBlockY >= 0) {
			highestBlockY--;
		}

		// If the highest block is below us then we're good, because we're just going to return this anyway lol
		// But if it's equal to where we started, we got a problem, sooooo
		if (highestBlockY == (int) mouse.y) {
			// ... check, going up from the current block, for the first null block to put it in
			while (squareAtYCoord(x, highestBlockY) != null && highestBlockY <= sqman.BOARDSIZEY - 1) {
				highestBlockY++;
			}
			return highestBlockY;
		} else {
			return highestBlockY + 1;
		}

		return -1;
	}

	private Square squareAtYCoord(int x, int y) {
		return board[x, y];
	}
}
