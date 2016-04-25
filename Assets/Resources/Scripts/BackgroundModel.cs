
using UnityEngine;
using System.Collections;

public class BackgroundModel : MonoBehaviour
{
	private float clock;		// Keep track of time since creation for animation.
	private Background owner;			// Pointer to the parent object.
	public Material mat;		// Material for setting/changing texture and color.
	private float BGSCALE = 4f;
	private float quadHeight;
	private float quadWidth;

	public void init(Background owner) {
		this.owner = owner;

		transform.parent = owner.transform;					// Set the model's parent to the background.
		transform.localPosition = new Vector3(owner.gm.cam.transform.position.x,owner.gm.cam.transform.position.y,1f);		// Center the model on the parent.
		//quadHeight = Camera.main.orthographicSize * 2.0f;
		//quadWidth = quadHeight * Screen.width / Screen.height;
		//		transform.localScale = new Vector3(quadWidth * BGSCALE, quadHeight * BGSCALE,1f);
		transform.localScale = new Vector3(GameManager.x_coord, GameManager.y_coord,1f);
		name = "Background Model";									// Name the object.

		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find ("Sprites/Default");						// Tell the renderer that our textures have transparency.
		mat.mainTexture = Resources.Load<Texture2D> ("Textures/background");	// Set the texture.  Must be in Resources folder.

	}
}
