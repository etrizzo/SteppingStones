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

	Square next = null;


	// Save our color here so it's consistent :)
	Color defaultColor, redTransparent, color;

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
		defaultColor = makeTransparent(new Color(0.2F, 0.3F, 0.4F));
		redTransparent = makeTransparent(Color.red, 0.5f);
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
				s.destroy ();
			}
		}
		DestroyImmediate (made.gameObject);
	}

	public void Update() {
		updateMouse ();
		// get first element in queue & change based on that
		updateModel ();

		if(sqman.gm.success){
			made.model.mat.color = Color.clear;
		}
			
	}

	void updateMouse() {
		worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mouse.x = (int)Mathf.Floor (worldPos.x);
		mouse.y = (int)Mathf.Ceil (worldPos.y);

	}

	Color makeTransparent(Color color, float transparency = 0.3f) {
		return new Color (color.r, color.g, color.b, transparency);
	}

	void updateModel() {
//		color = checkHighlightConflicts ();
		made.transform.position = new Vector3 (mouse.x, mouse.y, -1);
//		checkConflictAndColor (made);
		if (sqman.moving != null) {
			next = sqman.moving;
		} else {
			if (sqman.queue != null) {
				next = sqman.queue.Peek ();
			}
		}

		int type = 0;
		if (sqman.moving == null && next != null) {
			type = next.getType ();
		} else {
			type = 1;
		}

		// 0-c,1-m,2-y,-1-k
//		switch (next.getColor()) {
//		case 0:
//			color = makeTransparent (Color.cyan);
//			break;
//		case 1:
//			color = makeTransparent (Color.magenta);
//			break;
//		case 2:
//			color = makeTransparent (Color.yellow);
//			break;
//		}
		if (next != null) {
			color = makeTransparent (next.model.mat.color);
		}

		if (type == 2) {
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
		if (type == 2) {
			for (int i = 0; i < EXTRASHAPES; i++) {
	//			rigidShapes [i].model.mat.color = getHighlightColor ();
				Color rigidColor = getRigidHighlightColor (i);
				rigidShapes [i].model.mat.color = rigidColor;
			}
		}
	}

	bool insideBoard(float pos_x, float pos_y) {
		return (pos_x >= 0) && (pos_x < sqman.BOARDSIZEX) && (pos_y > 0) && (pos_y < sqman.BOARDSIZEY);
	}

	bool movableBlock(float pos_x, float pos_y) {
		Square s = board [(int)pos_x, (int)pos_y];
		if (s != null){
			return s.type == 1;
		}
		return false;
	}


	Color getHighlightColor () {
		Color retColor = defaultColor;

		if (sqman.queue == null) {
			return Color.clear;
		}

//		Square next = sqman.queue.Peek ();
		int type = next.getType ();
		// 0-c,1-m,2-y,-1-k
		switch (next.getColor()) {
		case 0:
			retColor = makeTransparent (Color.cyan);
			break;
		case 1:
			retColor = makeTransparent (Color.magenta);
			break;
		case 2:
			retColor = makeTransparent (Color.yellow);
			break;
		}
		if (next.type == 1) {
			Debug.Log ("Setting movable block color :)");
			return makeTransparent (next.model.mat.color);
		} else if (next.type == 2) {
			retColor = makeTransparent (next.rigid.color1);
		}

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
		if (next.type == 2) {
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
//		if (retColor != redTransparent) {
//			Debug.Log (("I'm not coloring this block red!"));
//		}
		return retColor;
	}


	Color getRigidHighlightColor(int i) {
		if (getHighlightColor() != redTransparent) {
//			Square next = sqman.queue.Peek ();
			if (i % 2 == 0) {
				return makeTransparent(next.rigid.color2);
			} else {
				return makeTransparent(next.rigid.color1);
			}
		} else {
			return getHighlightColor ();
		}
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
