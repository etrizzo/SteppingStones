using UnityEngine;
using System.Collections;

public class SquareModel : MonoBehaviour
{
	private int type;			//0 for blank, 1 
	//private float clock;		// Keep track of time since creation for animation.
	private Square owner;			// Pointer to the parent object.
	public Material mat;		// Material for setting/changing texture and color.
//	public Rigidbody2D rb;

	public void init(Square owner) {
		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(.5f,-.5f,-1);		// Center the model on the parent.
		name = "Square Model";									// Name the object.



		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 


		switch (owner.getType ()) {	//0-normal, 1-movable, 2-erase, 3-bomb, 4-rainbow

			case 1: 
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileMovable");
				break;
			case 2:
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileErase");
				break;
			case 3:
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileBomb");
				break;
			case 4:
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileRainbowBambi");
				break;
			default:
				mat.mainTexture = Resources.Load<Texture2D> ("Textures/tileBlank");	// Set the texture.  Must be in Resources folder.
				break;
		}




		switch(owner.getColor()){
			case 0:
				mat.color = Color.cyan;											// Set the color (easy way to tint things).
				break;
			case 1:
				mat.color = Color.magenta;
				break;
			case 2:
				mat.color = Color.yellow;
				break;
			case 3:
				mat.color = Color.white;
				break;
			case 4:
				mat.color = Color.white;
				break;
			case 5:
				mat.color = Color.gray;
				break;
			default:
				mat.color = Color.black;
				break;
		}


	}


}

