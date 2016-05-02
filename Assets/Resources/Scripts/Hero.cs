using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {
	private float counter = 0;
	private float speed = 5; // Need a "GOOOT TTAA GUCKDSBLJKAF,.JH;  FAST" button
	public HeroModel model;
	private Vector2 pos;
	//	private bool inqueue;

	public GameManager gm;
	public SquareManager sqman;
	public Square[,] board;


	public void init(Vector2 pos, GameManager gm){
		this.gm = gm;
		this.pos = pos;

		var modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		model = modelObject.AddComponent<HeroModel> ();
		model.init (this);

		initSound ();

	}

	public void initSound(){
		/*settleAudio = this.gameObject.AddComponent<AudioSource> ();
		settleAudio.loop = false;
		settleAudio.playOnAwake = false;
		settleAudio.time = 1.0f;
		settleClip = Resources.Load<AudioClip> ("Audio/Blocks Settle");
		settleAudio.clip = settleClip;

		conflictAudio = this.gameObject.AddComponent<AudioSource> ();
		conflictAudio.loop = false;
		conflictAudio.playOnAwake = false;
		conflictClip = Resources.Load<AudioClip> ("Audio/Block Colors Match");
		conflictAudio.clip = conflictClip;*/
	}

	public void addSquareManager(SquareManager sqman){
		this.sqman = sqman;
		this.board = sqman.board;
	}

	public void setPosition(Vector2 newpos){
		this.pos = newpos;
		this.transform.position = newpos;
	}

	public Vector2 getPosition(){
		return pos;
	}

	public void destroy(){
		DestroyImmediate (this.model);
		DestroyImmediate (this.gameObject);
	}

	public void nextMove(Vector2 newPos){
		model.nextMove (newPos, pos);
	}

	public void changeColor(){
		model.mat.color = Color.blue;
	}

	public void Update() {
		counter += Time.deltaTime;
		if (counter >= 1) {
		
		} else {
		}
	}
}

