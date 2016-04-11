using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RigidShape : MonoBehaviour {
	// Starting with _
	// 0 = _
	// 1 = l
	int shapeType = -1;
	int color1;
	int color2;
	Square[] squares = new Square[5];
	Square anchor;
	Square[,] board;

	SquareManager sm;

	public void init(Square a, Square[,] b, SquareManager sm) {
		anchor = a;
		anchor.rigid = this;
		anchor.setAnchor ();
		board = b;
		this.sm = sm;
		shapeType = generateTypeProbability();
		color1 = getColor ();
		color2 = color1;
		while (color2 == color1) {
			color2 = getColor ();
		}
		squares [0] = anchor;

		updateModel ();

		//Debug.Log ("Rigid Shape inited!");
	}

	private void updateModel() {
		// Do stuff to update model MANUALLY UGH
		for (int i = 0; i < 5; i++) {
			Square s = squares [i];
			if (s != null) {
				//update model
			}
		}
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
		square.rigid = this;

		sm.squares.Add (square);
		square.name = "Square " + sm.squares.Count;

		return square;

	}

	public IEnumerator settleShape(){
		Debug.Log("Settling shape!");
		Vector2 pos;
		yield return new WaitForSeconds (.25f);
		bool settle = checkSettle ();

		if (settle) {
			foreach(Square s in squares){
				pos = s.getPosition ();
				board [(int)pos.x, (int)pos.y] = null;
				board [(int)pos.x, (int)pos.y - 1] = s;
				s.setPosition (new Vector2 (pos.x, pos.y - 1));
			}
			yield return new WaitForSeconds (.25f);
			StartCoroutine (settleShape ());
		}
		else {
			//Debug.Log("Checking conflicts!");
			checkConflicts ();
		}
	}

	bool checkSettle(){
		Square s;
		Vector2 pos;
		Square below;
		for (int i = 0; i < 5; i++) {
			s = squares [i];
			pos = s.getPosition ();
			below = board [(int)pos.x, (int)(pos.y - 1)];
			if (below != null && below.rigid != s.rigid) {
				return false;
			}
		}
		return true;
	}

	void checkConflicts(){
	}

	public int getColor(){
		return Random.Range (0, 3);
	}
}