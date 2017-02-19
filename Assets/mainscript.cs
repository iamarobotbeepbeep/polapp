﻿using System.Collections;
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
	int maxWords=5;
	int wordsIndex=0;
	int curWord=0;
	GameObject[] fWords;
	float[] fWordsTime;
	float[] fWordsHForce;
	float[] fWordsVForce;
	SpriteRenderer[] fWordsRend;
	//byte wordsCounter=0;
	//GameObject politician;
	// Use this for initialization

	void Start() {
		barAnim = GameObject.Find("bar").GetComponent < Animator > ();
		polAnim = GameObject.Find("politician").GetComponent < Animator > ();
		lvlAlph = GameObject.Find("lvlAlph").GetComponent < SpriteRenderer > ();
		sublvlAlph = GameObject.Find("sublvlAlph").GetComponent < SpriteRenderer > ();

		fWords = new GameObject[maxWords];
		fWordsTime = new float[maxWords];
		fWordsHForce= new float[maxWords];
		fWordsVForce= new float[maxWords];
		fWordsRend = new SpriteRenderer[maxWords];

		numberSprites = Resources.LoadAll < Sprite > ("Sprites/alphav1");

		///initialize falling words gameobjects
		for (int i = 0; i < fWords.Length; i++) {

			fWords [i] = new GameObject ("word" + i);
			fWords [i].AddComponent<SpriteRenderer> ();
			fWordsRend[i] = fWords[i].GetComponent<SpriteRenderer> ();
			fWordsRend [i].enabled = false;
			fWordsRend[i].sprite = numberSprites [33];
			fWordsRend [i].transform.localScale = new Vector3 (.25F,.25F,1);
		}
		///end faling words initialzation

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

	public void Update() {
		if (Input.GetMouseButtonDown(0))
			click();
	}
	// Update is called once per frame
	public void FixedUpdate() {
		if (lvlTimer) {
			if (Time.time < timeLimit) {
				if (currentScore >= scoreGoal) {
					lvlTimer = false;
					if (subLvl == 5) {
						currentLvl++;
						lvlAlph.sprite = numberSprites[currentLvl - 1];
						subLvl = 0;
						sublvlAlph.sprite = numberSprites[9];
					} else {
						subLvl++;
						print(AssetDatabase.GetAssetPath(sublvlAlph.sprite));
						//sublvlAlph.sprite=(Resources.LoadAll<Sprite>("Assets/Standard Assets/2D/Sprites/alphav1.png"))[5];
						sublvlAlph.sprite = numberSprites[subLvl - 1];
					}
					print("you win. moving to level " + currentLvl + "." + subLvl);
					startLvlTimer();
				}
				if (curWord != 0) {
					wordFall ();
				}
			} else {
				lvlTimer = false;
				print("you lose, restarting level.");
				startLvlTimer();
				advanceBar();
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
		advanceBar();
		polAnimate("talk");
		print("clicked. score is " + currentScore);
	}

	public void advanceBar() {
		float barPos = currentScore / scoreGoal;
		//barAnim.Play ("barfill");
		if (barPos >= 1) {
			barAnim.Play("barfull");
		} else
			barAnim.Play("barfill", -1, barPos);
		//barAnim.SetFloat ("barfill",.5F);
	}

	public void polAnimate(string state) {
		if (state == "talk") {
			polAnim.Play("talk");
		}
	}

	public void click() {

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);


		//print ("mouse pos " + mousePosition.x + " y " + mousePosition.y + " ");    


		if (hitCollider) {
			//print ("Hit " + hitCollider.transform.name + " x" + hitCollider.transform.position.x + " y " + hitCollider.transform.position.y);    

			if (hitCollider.transform.name == "politician") {
				addScore();
				spitWord ();
			}
		}
	}

	public void spitWord()
	{
		wordsIndex++;
		wordsIndex = wordsIndex%maxWords;
		//if (fWords[wordsIndex]==null)
		//	print("is null");
		if (curWord!=maxWords)
		curWord++;
		fWordsRend [wordsIndex].enabled = true;
		//fWordsRend[wordsIndex].transform.position = new Vector3 (Random.Range(-25.0f, -15.0f),Random.Range(15.0f, 20.0f), 0);
		fWordsRend[wordsIndex].transform.position = new Vector3 (-18F,17F, 0);
		fWordsTime[wordsIndex]= Time.time+5;
		fWordsHForce[wordsIndex]=Random.Range(-8.0f, 8.0f);
		fWordsVForce[wordsIndex]=Random.Range(-10.0f, 10.0f);
		//initalize a new word to fall, add it to array.
	}

	public void wordFall()
	{
		int start = wordsIndex -curWord +1;
		int killed = 0;
		if (start < 0)
			start = start + maxWords;

		//print("start is " + start);

		//our max is five
		//so say index is 3, and current is 1, what happens
		//start = 1 - 3 +1
		// stat would equal -3
		/*
		what if we switch, stat = index - curword + 1
		so start = 3 - 1 +1 that works
		what if cur word is 3
		so start = 3 - 3 +1
		start would be 1 is that correct
		it would run 1, 2, and 3
		seems right
		what if index is 1 and current is 3
		stat = 1 -3 +1
would be -1
so -1 + 5 = 4.
so it would run 4 0 1


		*/
		//consider adding a 'current amount variable
		//could start array at current index - current amount % max words
		// so say current amount is five, current index is 3 and max is five
		// it would be  3 -5, so -2. -2%5 is 3.

		//or, we just subtract current amount minus current index and add one.
		// so in this case, 3-5=-2. +1 = -1.
		//if the resulting number is negative, say -1. just subtract -1 from max.
		//3 addition operations not so bad, probably less work then a modulo anyway.

		for (int i = 0; i <curWord; i++) {
			//print ("in loop. current word is " + curWord +" index is " + wordsIndex);
				if (fWordsTime [start] < Time.time) {
				//print ("killing index " + start);
					killWord (start);
				killed++;
				} else {
				fWordsHForce [start]=fWordsHForce [start]*.9F;
				fWordsVForce [start] = fWordsVForce [start] - 2F;
				if ((fWordsVForce [start] * -1F) > fWordsRend [start].transform.position.y)
					fWordsVForce [start] = fWordsRend [start].transform.position.y * -1F;

					
				fWordsRend [start].transform.position = fWordsRend [start].transform.position + new Vector3 (fWordsHForce [start],fWordsVForce[start],0);
			
					//move object
				}

	
			if (start == maxWords-1)
				start = 0;
			else
				start++;
		}
		curWord = curWord - killed;
		//makes all sprites in array fall. 
		//if they are to expire, remove them from array.
	}
	public void killWord(int index)
	{
		//Destroy (fWords [index]);
		//fWords [index] = null;
		fWordsRend[index].enabled=false;
		fWordsTime[index]=0;
		fWordsHForce[index]=0;
		fWordsVForce[index]=0;
	}


	public void dummyMove(){
		//assign force to main sprite just to test

		//lvlAlph.transform.
	}


}