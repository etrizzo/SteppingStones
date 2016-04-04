using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Square : MonoBehaviour {
	private SquareModel model;
	private int color;		//0-r,1-g,2-b
	private Vector2 pos;
	private bool isGround;



	public void init(Vector2 pos, int color, bool isGround){
		this.pos = pos;
		this.color = color;

		var modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		model = modelObject.AddComponent<SquareModel> ();
		model.init (this);

	}




	public int getColor(){
		return color;
	}

	public int setColor(int c){
		color = c;
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

}
