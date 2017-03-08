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
	Color defaultColor, redTransparent, color;

	public void init(Queue<Square> q, SquareManager sqm) {
		defaultColor = makeTransparent(new Color(0.2F, 0.3F, 0.4F), 0.1f);
		redTransparent = makeTransparent(Color.red, 0.15f);
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
			foreach (Square s in rigidShapes) {
				s.model.mat.color = Color.clear;
			}
		}
			
	}

	void updateMouse() {
		worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mouse.x = (int)Mathf.Floor (worldPos.x);
		mouse.y = (int)Mathf.Ceil (worldPos.y);

	}

	Color makeTransparent(Color color, float transparency = 0.15f) {
		return new Color (color.r, color.g, color.b, transparency);
	}

	void updateModel() {
//		color = checkHighlightConflicts ();
		made.transform.position = new Vector3 (mouse.x, mouse.y, -1);
//		checkConflictAndColor (made);
		Square next = new Square();
		if (sqman.moving == null) {
			if (sqman.queue != null) {
				next = sqman.queue.Peek ();
			}
		} else { 
			next = sqman.moving;
		}
		int type = next.getType ();
		// 0-c,1-m,2-y,-1-k
		switch (next.getColor()) {
		case 0:
			color = makeTransparent (Color.cyan);
			break;
		case 1:
			color = makeTransparent (Color.magenta);
			break;
		case 2:
			color = makeTransparent (Color.yellow);
			break;
		default:
			color = makeTransparent (defaultColor);
			break;
		}

		if (type == 2) {
			made.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileBlank");
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
			if (type == 1) {
				made.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileMovable");
			} else {
				made.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileBlank");
			}
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
		if (insideBoard(pos_x, pos_y)){
			Square s = board [(int)pos_x, (int)pos_y];
			if (s != null){
				return s.type == 1;
			}
		}
		return false;
	}

	Color getRigidHighlightColor(int i) {
		if (getHighlightColor() != redTransparent) {
			Square next = new Square();
			if (sqman.queue != null) {
				next = sqman.queue.Peek ();
			}
			if (i % 2 == 0) {
				return makeTransparent(next.rigid.color2);
			} else {
				return makeTransparent(next.rigid.color1);
			}
		} else {
			return getHighlightColor ();
		}
	}


	Color getHighlightColor () {
		Color retColor = defaultColor;
		Square next = new Square();
		if (sqman.moving == null) {
			if (sqman.queue != null) {
				next = sqman.queue.Peek ();
			}
		} else { 
			next = sqman.moving;
		}
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
		default:
			retColor = makeTransparent (defaultColor);
			break;
		}
		if (next.type == 2) {
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
						retColor = redTransparent;
					}
				} else {
					retColor = redTransparent;
				}
			}
		}
		return retColor;
	}

}
