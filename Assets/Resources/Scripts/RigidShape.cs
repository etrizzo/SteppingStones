using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RigidShape : MonoBehaviour {
	// Starting with _
	// 0 = _
	// 1 = l
	int shapeType = -1;
	Square[] squares = new Square[5];
	Square anchor;
	Square[,] board;

	SquareManager sm;

	public void init(Square a, Square[,] b, SquareManager sm) {
		anchor = a;
		anchor.rigid = this;
		board = b;
		this.sm = sm;
		shapeType = generateTypeProbability();
		squares [0] = anchor;

		updateModel ();
	}

	private void updateModel() {
		// Do stuff to update model MANUALLY UGH
	}


	private int generateTypeProbability() {
		// Should be adjusted depending on level????????????????????????????????????????????????
		return (int) Random.Range(0,0);
	}

	public void grow() {
		// you know, grow and stuff .....

		switch (shapeType) {
		case 0:
			grow__();
			break;

		default:
			print ("Whoopsies! you hit the default shape case, line 42");
			break;
		}
	}

	private void grow__() {
		Vector2 pos = anchor.getPosition ();
		Vector2 new_pos;
		for (int i = 1; i <= 4; i++) {
			new_pos = new Vector2 (pos.x + i, pos.y);
			squares [i] = addSquare (new_pos);
		}
	}


	private void grow_l() {
		
	}

	// ---
	public Square addSquare(Vector2 pos){
		GameObject squareObject = new GameObject ();
		Square square = squareObject.AddComponent<Square> ();

		square.transform.parent = sm.squareFolder.transform;
		square.transform.position = new Vector3 (pos.x, pos.y, 0);
		square.init (pos, 4, false, 5);

		sm.squares.Add (square);
		square.name = "Square " + sm.squares.Count;

		return square;

	}
}