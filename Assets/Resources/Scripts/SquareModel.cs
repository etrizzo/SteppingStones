using UnityEngine;
using System.Collections;

public class SquareModel : MonoBehaviour
{
	private int type;			//0 for blank, 1 
	//private float clock;		// Keep track of time since creation for animation.
	private Square owner;			// Pointer to the parent object.
	private Material mat;		// Material for setting/changing texture and color.

	public void init(Square owner) {

		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(.5f,-.5f,0);		// Center the model on the parent.
		name = "Square Model";									// Name the object.
		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 
		mat.mainTexture = Resources.Load<Texture2D>("Textures/tileBlank");	// Set the texture.  Must be in Resources folder.
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
			default:
				mat.color = Color.black;
				break;
		}
	}


}

