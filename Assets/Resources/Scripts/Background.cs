using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour
{

	private BackgroundModel bg_model;
	public Material bgMat;
	private GameObject modelObject;
	public GameManager gm;

	public GameObject bgSquareFolder;
	public List<Square> bgSquares;

	private int speed = 1;
	private float counter = 5f;

	private float x;
	private float y;

	public void init (int x, int y, GameManager gm)
	{
		this.gm = gm;
		this.modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		modelObject.name = "BG Model";
		this.bg_model = modelObject.AddComponent<BackgroundModel> ();
		this.bg_model.init (this);
		bgMat = bg_model.GetComponent<Renderer> ().material;
		bgMat.color = new Color (.74f, .74f, .74f, .6f);

		bgSquareFolder = new GameObject ();
		bgSquareFolder.name = "BG Squares";
		bgSquares = new List<Square> ();
		makeBgSquares ();
	}

	private void makeBgSquares ()
	{
		for (int i = 0; i < 30; i++) {
			GameObject squareObject = new GameObject ();
			Square square = squareObject.AddComponent<Square> ();

			square.transform.parent = bgSquareFolder.transform;
			x = randFloat (-8, gm.sqman.BOARDSIZEX + 5);
			y = randFloat (-5, gm.sqman.BOARDSIZEY + 5);
			square.transform.position = new Vector3 (x, y, 1);
			square.init (new Vector2 ((float)x, (float)y), 4, false);

			square.name = "BGSquare " + i;



			if (gm.bambiQwop) {
				float f1 = randFloat (.3f, 1f);
				float f2 = randFloat (.3f, 1f);
				float f3 = randFloat (.3f, 1f);
				square.model.mat.color = new Color (f1, f2, f3, randFloat (.4f, .9f));


			} else {
				float f = randFloat (0f, 1f);
				square.model.mat.color = new Color (f, f, f, randFloat (0f, .8f));
			}
			float lw = randFloat (.1f, 6.5f);
			square.model.transform.localScale = new Vector3 (lw, lw, 2);
			square.model.mat.mainTexture = Resources.Load<Texture2D> ("Textures/bgSquare");
			bgSquares.Add (square);
		}
	}

	private float randFloat (float lowerlimit, float upperlimit)
	{
		return Random.Range (lowerlimit, upperlimit);
	}

	private int randInt (int lowerlimit, int upperlimit)
	{
		return Random.Range (lowerlimit, upperlimit);
	}

	void Update ()
	{	
		if (gm.bambiQwop) {
			counter += Time.deltaTime * speed * 10;
		} else {
			
			counter += Time.deltaTime * speed;
		}

		foreach (Square s in bgSquares) {
			//float newX = randFloat (s.getPosition().x - 1, s.getPosition().x + 1);
			//float newY = randFloat (s.getPosition().y - 1, s.getPosition().y + 1);
			//s.transform.localPosition = new Vector3 (Mathf.Sin (10 * counter / 6), Mathf.Sin (5 * counter / 6), 0);	
			if (counter > 5) {	
				switch (randInt (0, 4)) {
				case 0:
					s.direction = Vector3.up;
					break;
				case 1:
					s.direction = Vector3.right;
					break;
				case 2:
					s.direction = Vector3.down;
					break;
				case 3:
					s.direction = Vector3.left;
					break;
				}
			}
			if (gm.bambiQwop) {
				s.transform.Translate (s.direction * Time.deltaTime * 4);
			} else {
				s.transform.Translate (s.direction * Time.deltaTime);
			}
		}
		if (counter > 5) {	
			counter = 0;
		}
	}
}

