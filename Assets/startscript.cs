using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class startscript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnMouseDown(){
		print ("hi");		
		SceneManager.LoadScene("Charselect", LoadSceneMode.Single);
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
