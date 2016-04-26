using UnityEngine;
using System.Collections;

public class HeroModel : MonoBehaviour
{
	private Hero owner;			// Pointer to the parent object.
	public Material mat;		// Material for setting/changing texture and color.

	public float counter = 0;
	public int speed = 5;

	public void init(Hero owner) {
		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(.5f,-.5f,0);		// Center the model on the parent.
		name = "Hero Model";									// Name the object.



		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 

		mat.mainTexture = Resources.Load<Texture2D> ("Textures/hero");
	}

	public void Update() {
		counter += Time.deltaTime * speed;
		if (counter >= 2) {
			counter = 0;
		}
		if (counter >= 1) {
			transform.localPosition = new Vector3 (.5f, -.45f, 0);
		} else if(counter >= 0){
			transform.localPosition = new Vector3 (.5f, -.5f, 0);
		}
	}


}


