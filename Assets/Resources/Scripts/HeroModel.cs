using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroModel : MonoBehaviour
{
	private Hero owner;			// Pointer to the parent object.
	public Material mat;		// Material for setting/changing texture and color.

	public float counter = 0;
	public int speed = 5;
	int moveSpeed = 100;
	public float clock = 0f;
	bool move = false;
	public bool canMove = false;
	List<Square> squarePath;
	bool first = true;
	private float farthestX;
	private float highestY;
	private Square curSquare;
	private int sqcounter= 0; //start on square after beginning, since hero is alreayd on the beginning square
	private bool end = false;



	public void init(Hero owner) {
		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(.5f,-.5f,-2);		// Center the model on the parent.
		name = "Hero Model";									// Name the object.



		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 

		mat.mainTexture = Resources.Load<Texture2D> ("Textures/hero");

		highestY = -.5f;
		farthestX = .5f;
	}

	public void Update() {

		if (canMove) {
			Debug.Log ("I am in solved hero person siht");
			clock += Time.deltaTime;
			if (first) {
				farthestX = 1;
				highestY = transform.position.y;
				first = false;
			}
			moveAlong ();
		} else { 		//Jumping in Place at the beginning
			counter += Time.deltaTime * speed;
			if (counter >= 2) {
				counter = 0;
			}
			if (counter >= 1) {
				transform.localPosition = new Vector3 (farthestX, highestY+.05f, 0);
			} else if (counter >= 0) {
				transform.localPosition = new Vector3 (farthestX, highestY, 0);
			}
		}
	}

	public void moveAlong(){	//List<Square> squarePath
			if (clock >= farthestX) {
				updateSquareInfo ();
			}
			transform.position = new Vector3 (clock+, highestY, 0);

//		foreach (Square sq in owner.gm.squarePath) {
//			//Vector2 nextPos = sq.getPosition ();
//			//hero.model.nextMove (nextPos);
//
//			//while (clock < nextPos.x+1) {
//				//transform.position = new Vector3 (clock, 1, 0);
//			//}
//			Debug.Log ("I am looking at square: " + sq.name + " and the clock is at: " + clock + "canMove is set to: " + canMove);
//		}
	}

	void updateSquareInfo(){
		sqcounter++;
		if (sqcounter == (owner.gm.squarePath.Count)) {
//			Debug.Log ("sq = count.... so farthest x is: " + farthestX + " highest y is: " + highestY);
			farthestX = farthestX + .5f;
			highestY = highestY - 1f;
			canMove = false;
		} else {
			curSquare = owner.gm.squarePath [sqcounter];
			Vector2 pos = curSquare.getPosition ();
			farthestX = pos.x + 1; // may have to change
			highestY = curSquare.model.transform.position.y + 1;
			Debug.Log("on square: " + curSquare.name + " farthest x is: " + farthestX + " highest y is: " + highestY);
		}

	}

	//Takes in the newsquare position and the current hero position
	public void nextMove(Vector2 nextPos, Vector2 pos){
		float xpos = transform.position.x;
		float ypos = transform.position.y;
//		while (xpos < nextPos.x) {			
//			xpos = xpos + (Time.deltaTime / moveSpeed);
//			owner.transform.position = new Vector3 (xpos, ypos, 0);
//			//pos.x = xpos;
//		}
//
//		while (pos.y < nextPos.y+1) {		
//			ypos = pos.y + (Time.deltaTime / moveSpeed);
//			transform.position = new Vector3 (xpos, ypos, 0);
//			pos.y = ypos;
//		}
		transform.position = new Vector3 (xpos, ypos, 0);
		Debug.Log ("xpos is: "+xpos + "ypos is: " +ypos);


		//transform.localPosition = new Vector3( 

	}


}


