using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
	private int type;			//0 for blank, 1 
	//private float clock;		// Keep track of time since creation for animation.
	private GameManager gm;			// Pointer to the parent object.
	private Square window;
	public Material mat;		// Material for setting/changing texture and color.
	//	public Rigidbody2D rb;

	public void init(Vector2 pos, Square window, GameManager gm) {
		this.window = window;
		this.gm = gm;
		transform.parent = window.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(.9f,(-.5f)*pos.y,0);		// Center the model on the parent.
		name = "Tower";									// Name the object.

		transform.localScale = new Vector3 (2.5f, 1.4f*pos.y,1);

		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/tower");
		}

}