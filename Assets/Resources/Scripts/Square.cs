using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Square : MonoBehaviour {
	private SquareModel model;
	private int color;		//0-r,1-g,2-b
	private Vector2 pos;
	private bool ground;
//	private bool inqueue;

	private int type; 	//0-normal, 1-movable, 2-erase, 3-bomb, 4-rainbow



	public void init(Vector2 pos, int color, bool isGround = false, int type = 0){
		this.pos = pos;
		this.color = color;
		this.ground = isGround;
		this.type = type;

		var modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		model = modelObject.AddComponent<SquareModel> ();
		model.init (this);

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


	public Square checkConflicts(){
		//check for conflicts and return the conflicting square
		return this;
	}

	public bool isGround(){
		return ground;
	}

	public void destroy(){
		DestroyImmediate (this.model);
		DestroyImmediate (this.gameObject);

	}


}
