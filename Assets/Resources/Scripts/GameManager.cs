using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


	SquareManager sqman;
	public AudioSource gameAudio;
	public AudioClip gameClip;

	void Start () {
		GameObject sqmanObject = new GameObject ();
		sqman = sqmanObject.AddComponent<SquareManager> ();
		sqman.name = "Square Manager";
		sqman.init ();

		initSound ();
	}

	void initSound(){
		gameAudio = this.gameObject.AddComponent<AudioSource> ();
		gameAudio.loop = true;
		gameAudio.playOnAwake = false;
		gameClip = Resources.Load<AudioClip> ("Audio/Soundtrack Draft 1");
		gameAudio.clip = gameClip;
		gameAudio.Play ();
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