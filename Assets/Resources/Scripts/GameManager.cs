using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


	SquareManager sqman;

	void Start () {
		GameObject sqmanObject = new GameObject ();
		sqman = sqmanObject.AddComponent<SquareManager> ();
		sqman.init ();
	}


	void Update(){
		if (Input.GetMouseButtonUp(0)) {
			Vector3 worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			int mousex = (int) Mathf.Floor(worldPos.x);
			int mousey = (int) Mathf.Ceil(worldPos.y);
			sqman.placeSquare(new Vector2((float)mousex, (float)mousey));
		}

	}
}