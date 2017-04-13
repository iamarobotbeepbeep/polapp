using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class charselect : MonoBehaviour {

	SpriteRenderer algfg, lisafg, grimfg;
	public static string charstring = "lisa";

	// Use this for initialization
	void Start () {
		algfg = GameObject.Find ("algfg").GetComponent<SpriteRenderer> ();
		grimfg = GameObject.Find ("grimfg").GetComponent<SpriteRenderer> ();
		lisafg = GameObject.Find ("lisafg").GetComponent<SpriteRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)){
			//regular click, menu is not open.
			click ();
		}
	}

	public static string getString()
	{
		return charstring;
	}

	void click(){

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
		print ("test");

		if (hitCollider.transform.name == "grimbg") {
			
			charstring = "grim";
			grimfg.enabled = false;
			algfg.enabled = true;
			lisafg.enabled = true;
		}

		else if (hitCollider.transform.name == "lisabg") {
			
			charstring = "lisa";
			grimfg.enabled = true;
			algfg.enabled = true;
			lisafg.enabled = false;
		}

		else if (hitCollider.transform.name == "algbg") {
			
			charstring = "alg";
			grimfg.enabled = true;
			algfg.enabled = false;
			lisafg.enabled = true;
		}

		else if (hitCollider.transform.name == "startbutton") {
			SceneManager.LoadScene("Mainscene", LoadSceneMode.Single);
		}

	}
}
