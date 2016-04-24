using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Square : MonoBehaviour {
	private float counter = 0;
	private float speed = 5; // Need a "GOOOT TTAA GUCKDSBLJKAF,.JH;  FAST" button
	public SquareModel model;
	private int color;		//0-c,1-m,2-y,-1-k
	private Vector2 pos;
	private bool ground;
//	private bool inqueue;

	private int type; 	//-normal, 1-movable, 2-erase, 3-bomb, 4-rainbow, 5-shape
	public bool falling;
	public bool anchor;
	public RigidShape rigid;
	public SquareManager sqman;
	public Square[,] board;

	public bool wait = false;
	public float conflictCounter = 0;

	public AudioSource settleAudio;
	public AudioClip settleClip;
	public AudioSource conflictAudio;
	public AudioClip conflictClip;


	public void init(Vector2 pos, int color, bool isGround = false, int type = 0){
		this.pos = pos;
		this.color = color;
		this.ground = isGround;
		this.type = type;

		var modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		model = modelObject.AddComponent<SquareModel> ();
		model.init (this);

		initSound ();

	}

	public void initSound(){
		settleAudio = this.gameObject.AddComponent<AudioSource> ();
		settleAudio.loop = false;
		settleAudio.playOnAwake = false;
		settleAudio.time = 1.0f;
		settleClip = Resources.Load<AudioClip> ("Audio/Blocks Settle");
		settleAudio.clip = settleClip;

		conflictAudio = this.gameObject.AddComponent<AudioSource> ();
		conflictAudio.loop = false;
		conflictAudio.playOnAwake = false;
		conflictClip = Resources.Load<AudioClip> ("Audio/Block Colors Match");
		conflictAudio.clip = conflictClip;
	}

	public void addSqman(SquareManager sqman) {
		this.sqman = sqman;
		this.board = sqman.board;
	}

	public int getColor(){
		return color;
	}

	public int getType(){
		return type;
	}

	public int setType(int t){
		this.type = t;
		return this.type;
	}

	public int setColor(int c){
		this.color = c;
		return color;
	}

	public int setModelColor(float c){
		model.mat.color = model.mat.color * c;
		return color;
	}

	public void setPosition(Vector2 newpos){
		this.pos = newpos;
		this.transform.position = newpos;
	}

	public Vector2 getPosition(){
		return pos;
	}
		
	public bool isGround(){
		return ground;
	}
		
	public void destroy(){
		DestroyImmediate (this.model);
		DestroyImmediate (this.gameObject);
	}

	public bool isFalling(){
		return falling;
	}

	public void setFalling(bool f){
		falling = f;
	}

	// Rigid Class Stuff
	public bool isAnchor(){
		return anchor;
	}

	public void setAnchor(){
		anchor = true;
	}

	public IEnumerator tempHighlight() {
		Color originalColor = (Color) this.model.mat.color;
		this.model.mat.color = Color.green;
		yield return new WaitForSeconds(0.5f);
		this.model.mat.color = originalColor;
//		yield return null;
	}


	public void checkFall() {
		// Move down and stuff?j
		Square above = null;
		if (pos.y + 1 < sqman.BOARDSIZEY) {
			above = board [(int)pos.x, (int)pos.y + 1];
		}
		Square below = board[(int) pos.x, (int) pos.y - 1];


		// If there's no square under you, fall!
		if (below == null) {
			fall ();
			if (above != null) {
				above.setFalling (true);
			}
			counter = 0f;
		} else if (!below.isFalling ()) {
			// & it's not falling ....
			settleAudio.Play ();
			falling = false;
			counter = 0f;
			wait = true;
		} else {
			// Do nothing, wait for Update() to call checkFall again ;)))))))
		}
	}

	public void fall() {
		board [(int) pos.x,(int) pos.y] = null;
		board [(int) pos.x,(int) pos.y - 1] = this;
		setPosition (new Vector2 (pos.x, pos.y - 1));
	}

	public Square[] getNeighbors(){
		Square[] directedBlocks = new Square[4];
		if (pos.y + 1 < sqman.BOARDSIZEY) {
			directedBlocks [0] = board [(int)pos.x, (int)pos.y + 1];
		}
		if (pos.x + 1 < sqman.BOARDSIZEX) {
			directedBlocks [1] = board [(int)pos.x + 1, (int)pos.y];
		}
		if(pos.y - 1 >= 0){
		directedBlocks[2] = board[(int) pos.x, (int) pos.y - 1];
		}
		if (pos.x - 1 >= 0) {
			directedBlocks [3] = board [(int)pos.x - 1, (int)pos.y];
		}
		return directedBlocks;
	}

	public void checkConflicts() {
		wait = false; 
		conflictCounter = 0;
		bool conflicted = false;
		Square[] directedBlocks = getNeighbors ();

		foreach (Square sq in directedBlocks) {
			if (sq != null && sq.getColor() == color && !sq.isFalling()) {
				sqman.chainSettle (sq.getPosition());
				Destroy (sq.gameObject);
				conflicted = true;
				conflictAudio.Play (); //we need more time???
			}
		}

		GameObject self = this.gameObject;

		if (conflicted) {
			Destroy (self);
		}
	}


	public void Destroy(GameObject s){
		DestroyImmediate (s);
	}

	public void Update() {
		if (wait) {
			if (type != 4) {
				conflictCounter += Time.deltaTime * speed;
				if (conflictCounter >= 1) {
					
					checkConflicts ();
				}
			} else {
				wait = false;
			}
		}
		if (isFalling()) {
			counter += Time.deltaTime * speed;
			if (counter >= 1) {
				checkFall();
			}
		}
	}
}
