using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class politicianScript : MonoBehaviour {
	mainscript main;
	// Use this for initialization
	void Start () {
		main = GameObject.Find ("Main Camera").GetComponent<mainscript>();
		
	}

	//void OnMouseDown()
	//{
	//	main.addScore ();
	//}

	// Update is called once per frame
	//void Update () {		
	//}
}
