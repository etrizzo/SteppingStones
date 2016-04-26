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

	public void init(Queue<Square> q, SquareManager sqm) {
		sqman = sqm;
		lastMousePos = new Vector2 (-1, 1);
		queue = q;

		// Init made actually lolololol
		made = sqman.addSquare(new Vector2(mouse.x, mouse.y), false);
		made.model.mat.color = Color.gray;
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
		if (mouseMoved()) {
			Debug.Log ("Got the queue, it looks like: " + queue.Count);
			Square next = sqman.queue.Peek ();
			int type = next.getType ();
			Debug.Log ("Next queued block is of type: " + type);
			if (type < 5) {
				// TODO: Note mouse.y, but a new variable that causes it to be the block it woud
				// acutally land on, so something like 'lowestPossibleY' ????
				made.transform.position = new Vector3(mouse.x, mouse.y, 0);
			} else if (type == 5) {
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
		lastMousePos = new Vector2((int)mouse.x, (int) mouse.y);
	}

	private bool mouseMoved() {
		// This should actually check if it moved over 1 square or more since last update :/
		return lastMousePos == mouse;
	}
}
