using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class mainscript: MonoBehaviour {

	float startTime;
	float timeLimit;
	int currentLvl;
	int subLvl;
	bool lvlTimer = false;
	float currentScore;
	float scoreGoal;
	Animator barAnim, polAnim;
	SpriteRenderer lvlAlph, sublvlAlph;
	Sprite[] numberSprites;
	//GameObject politician;
	// Use this for initialization

	void Start() {
		barAnim =GameObject.Find ("bar").GetComponent<Animator>();
		polAnim =GameObject.Find ("politician").GetComponent<Animator>();
		lvlAlph=GameObject.Find ("lvlAlph").GetComponent<SpriteRenderer>();	
		sublvlAlph=GameObject.Find ("sublvlAlph").GetComponent<SpriteRenderer>();

		numberSprites =Resources.LoadAll<Sprite>("Sprites/alphav1");

		//politician = GameObject.Find ("politiican");
		//Event clickPol = new Event ();
		//	clickPol.button;
		//politician.ev
		//politician.GetComponent<BoxCollider2D> ().;
		currentLvl = 1;
		subLvl = 0;
		startLvlTimer();
		print("Game initiated.");
	}

	public void Update()
	{
		if (Input.GetMouseButtonDown (0))
			click ();
	}
	// Update is called once per frame
	public void FixedUpdate() {
		if (lvlTimer) {
			if (Time.time < timeLimit) {
				if (currentScore >= scoreGoal) {
					lvlTimer = false;
					if (subLvl == 5) {
						currentLvl++;
						lvlAlph.sprite = numberSprites [currentLvl-1];
						subLvl = 0;
						sublvlAlph.sprite = numberSprites [9];
					} else {
						subLvl++;
						print (AssetDatabase.GetAssetPath (sublvlAlph.sprite));
						//sublvlAlph.sprite=(Resources.LoadAll<Sprite>("Assets/Standard Assets/2D/Sprites/alphav1.png"))[5];
						sublvlAlph.sprite = numberSprites [subLvl-1];
					}
					print("you win. moving to level " + currentLvl + "." + subLvl);
					startLvlTimer();
				}
			} else {
				lvlTimer = false;
				print("you lose, restarting level.");
				startLvlTimer();
				advanceBar ();
			}
		}

	}

	public void startLvlTimer() {
		startGameScore();
		lvlTimer = true;

		if (subLvl == 5)
			timeLimit = 30;
		else
			timeLimit = 9999;

		startTime = Time.time;
		timeLimit = startTime + timeLimit;

	}

	public void startGameScore() {
		currentScore = 0;
		scoreGoal = (currentLvl * 5) + (subLvl * 5);

	}

	public void addScore() {
		currentScore++;
		advanceBar ();
		polAnimate ("talk");
		print ("clicked. score is " + currentScore);
	}

	public void advanceBar()
	{
		float barPos = currentScore / scoreGoal;
		//barAnim.Play ("barfill");
		if (barPos >= 1) {
			barAnim.Play("barfull");
		}
		else
		barAnim.Play("barfill", -1, barPos);
		//barAnim.SetFloat ("barfill",.5F);
	}

	public void polAnimate(string state)
	{
		if (state == "talk") {
			polAnim.Play("talk");
		}
	}

	public void click()
	{

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint (mousePosition);
	

		//print ("mouse pos " + mousePosition.x + " y " + mousePosition.y + " ");    


		if (hitCollider) {
			//print ("Hit " + hitCollider.transform.name + " x" + hitCollider.transform.position.x + " y " + hitCollider.transform.position.y);    
		
			if (hitCollider.transform.name == "politician") {
				addScore ();
			}
		}
	}

}