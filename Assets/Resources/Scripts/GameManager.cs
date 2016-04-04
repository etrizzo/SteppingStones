using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class GameManager : MonoBehaviour {

	GameObject squareFolder;
	List<Square> squares;
	SquareManager sqman;

	void Start () {
		squareFolder = new GameObject();
		squares = new List<Square> ();
		sqman = new SquareManager ();
		sqman.init ();
	}


	void Update(){
		if (Input.GetMouseButtonUp(0)) {
			Vector3 worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			int mousex = (int) worldPos.x;
			int mousey = (int) worldPos.y;
			sqman.placeSquare(new Vector2((float)mousex, (float)mousey));
		}

	}
}