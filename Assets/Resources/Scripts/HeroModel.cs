using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroModel : MonoBehaviour
{
	private Hero owner;			// Pointer to the parent object.
	public Material mat;		// Material for setting/changing texture and color.

	public float counter = 0;
	public float speed = 5f;
	int moveSpeed = 100;
	public float clock = -1f;
	bool move = false;
	public bool canMove = false;
	List<Square> squarePath;
	bool first = true;
	private float farthestX;
	private float highestY;
	private Square curSquare;
	private int sqcounter= -1; //start on square after beginning, since hero is alreayd on the beginning square
	private bool end = false;



	public void init(Hero owner) {
		if (owner.gm.bambiQwop) {
			speed = 30;
		}

		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(.5f,-.5f,-2);		// Center the model on the parent.
		name = "Hero Model";									// Name the object.



		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 

		if (owner.gm.bambiQwop) {
			mat.mainTexture = Resources.Load<Texture2D> ("Textures/sonich");
		} else {
			mat.mainTexture = Resources.Load<Texture2D> ("Textures/hero");
		}

		highestY = -.5f;
		farthestX = -.5f;
	}

	public void Update() {

		if (canMove) {
			Debug.Log ("I am in solved hero person siht");
			if (owner.gm.bambiQwop) {
				clock += Time.deltaTime * speed * 5;
			} else {
				clock += Time.deltaTime * (speed * .75f);
			}
			if (first) {
				farthestX = -1;
				highestY = transform.position.y;
				first = false;
			}
			moveAlong ();
//		}
		} else { 		//Jumping in Place when it is not moving
//			print ("JUMP! " + farthestX + ", " + highestY);
			counter += Time.deltaTime * speed;
			if (counter >= 2) {
				counter = 0;
			}
			if (counter >= 1) {
//				transform.position = new Vector3 (farthestX, highestY+.05f, 0);
				transform.localPosition = new Vector3 (0, .05f, 0);
			} else if (counter >= 0) {
//				transform.position = new Vector3 (farthestX, highestY, 0);
				transform.localPosition = new Vector3 (0, 0, 0);
			}
		}
	}

	public void moveAlong(){	//List<Square> squarePath
		if (clock >= farthestX) {
			updateSquareInfo ();
//			if (canMove) {
//				print ("updating position to : " + (clock + .5f) + ", " + highestY);
//				print (" or actually... " + (curSquare.getPosition ().x + .5f) + " , " + (curSquare.getPosition ().y + .5f));
//
//
//			}
		}
		print ("moving to : " + farthestX + ", " + highestY);
		owner.transform.position = new Vector3 (farthestX, highestY, 0);
			



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
		if (sqcounter >= (owner.gm.squarePath.Count)) {
//			Debug.Log ("sq = count.... so farthest x is: " + farthestX + " highest y is: " + highestY);
//			farthestX = farthestX + .5f;
//			highestY = highestY 1f;
//			print("cursquare is : " + curSquare.getPosition() + " And we're stopping there.");
			if (!owner.gm.success) {
				resetHero ();
			} else { //success!
				if (owner.gm.bambiQwop) {
					mat.mainTexture = Resources.Load<Texture2D> ("Textures/sonichVictory");
					owner.gm.saved = true;
				} else {
					mat.mainTexture = Resources.Load<Texture2D> ("Textures/heroSuccess");
					owner.gm.saved = true;
				}
			}
			canMove = false;
		} else if (sqcounter >= (owner.gm.squarePath.Count-1)) {
			if (owner.gm.bambiQwop) {
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/sonichRoll");
			}
			curSquare = owner.gm.squarePath [sqcounter];
			Vector2 pos = curSquare.getPosition ();
			print ("Counter " + sqcounter + ", Cursquare: " + curSquare.getPosition()); 
			//			farthestX = pos.x + 1; // may have to change
			farthestX = pos.x + .25f;
			//			highestY = pos.y + .5f;
			highestY =pos.y + .5f;
		}
		else {
			if (owner.gm.bambiQwop) {
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/sonichRoll");
			}
			curSquare = owner.gm.squarePath [sqcounter];
			Vector2 pos = curSquare.getPosition ();
			print ("Counter " + sqcounter + ", Cursquare: " + curSquare.getPosition()); 
//			farthestX = pos.x + 1; // may have to change
			farthestX = pos.x + .5f;
//			highestY = pos.y + .5f;
			highestY =pos.y + .5f;
//			Debug.Log("on square: " + curSquare.name + " farthest x is: " + farthestX + " highest y is: " + highestY);
		}

	}

	void resetHero(){
		sqcounter = -1;
		counter = 0;
		clock = -1f;
		print ("resetting to: -.5, " +  (owner.gm.beginning.getPosition().y + .5f));
		owner.transform.position = new Vector3 (-.5f, owner.gm.beginning.getPosition().y + .5f, 0);
		print (owner.transform.position);
		first = true;
		highestY = owner.gm.beginning.getPosition ().y + .5f;
		farthestX = -.5f;
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


