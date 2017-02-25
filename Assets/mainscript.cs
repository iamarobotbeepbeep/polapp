using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class mainscript: MonoBehaviour {

	float startTime;
	float timeLimit;
	int currentLvl;
	int subLvl;
	bool lvlTimer = false;
	float currentScoreInf;
	float currentScoreMon;
	float scoreGoalInf;
	Animator barAnim, polAnim;
	//SpriteRenderer lvlAlph, sublvlAlph;
	Sprite[] /*numberSprites,*/wordSprites;
	Text monTxt,lvlTxt,totlMonTxt;
	int maxWords=5;
	int fWordsPointer=0;
	int activWords=0;
	GameObject[] fWords;
	float[] fWordsTime;
	float[] fWordsHForce;
	float[] fWordsVForce;
	float[] fWordsMVal;
	float[] fWordsMoneyPop;
	SpriteRenderer[] fWordsRend;
	float killMon=0f;
	//byte wordsCounter=0;
	//GameObject politician;
	// Use this for initialization

	void Start() {
		monTxt= GameObject.Find ("monPop").GetComponent<Text> ();
		lvlTxt= GameObject.Find ("level").GetComponent<Text> ();
		totlMonTxt= GameObject.Find ("money").GetComponent<Text> ();

		barAnim = GameObject.Find("bar").GetComponent < Animator > ();
		polAnim = GameObject.Find("politician").GetComponent < Animator > ();
		//lvlAlph = GameObject.Find("lvlAlph").GetComponent < SpriteRenderer > ();

		//sublvlAlph = GameObject.Find("sublvlAlph").GetComponent < SpriteRenderer > ();

		fWords = new GameObject[maxWords];
		fWordsTime = new float[maxWords];
		fWordsHForce= new float[maxWords];
		fWordsVForce= new float[maxWords];
		fWordsRend = new SpriteRenderer[maxWords];
		fWordsMoneyPop = new float[maxWords];

		//numberSprites = Resources.LoadAll < Sprite > ("Sprites/alphav1");
		wordSprites = Resources.LoadAll < Sprite > ("Sprites/wordicons");
		fWordsMVal = new float[wordSprites.Length];
		for (int i = 0; i < fWordsMVal.Length; i++) {
			fWordsMVal [i] = i;
		}
		///initialize falling words gameobjects
		for (int i = 0; i < fWords.Length; i++) {

			fWords [i] = new GameObject ("word" + i);
			fWords [i].AddComponent<SpriteRenderer> ();
			fWordsRend[i] = fWords[i].GetComponent<SpriteRenderer> ();
			fWordsRend [i].enabled = false;
			fWordsRend[i].sprite = wordSprites [0];
			fWordsRend [i].transform.localScale = new Vector3 (3,3,1);
		}
		///end faling words initialzation

		//politician = GameObject.Find ("politiican");
		//Event clickPol = new Event ();
		//	clickPol.button;
		//politician.ev
		//politician.GetComponent<BoxCollider2D> ().;
		currentLvl = 1;
		subLvl = 0;
		currentScoreMon = 0f;
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
				if (currentScoreInf >= scoreGoalInf) {
					lvlTimer = false;
					if (subLvl == 5) {
						currentLvl++;
						//lvlAlph.sprite = numberSprites[currentLvl - 1];
						subLvl = 0;
						//sublvlAlph.sprite = numberSprites[9];
						lvlTxt.text=currentLvl + "." + subLvl;
					} else {
						subLvl++;
						//sublvlAlph.sprite=(Resources.LoadAll<Sprite>("Assets/Standard Assets/2D/Sprites/alphav1.png"))[5];
						lvlTxt.text=currentLvl + "." + subLvl;
					}
					print("you win. moving to level " + currentLvl + "." + subLvl);
					startLvlTimer();
				}
				if (activWords != 0) {
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
		currentScoreInf = 0;
		scoreGoalInf = (currentLvl * 5) + (subLvl * 5);

	}

	public void addScore(int wordIndex) {
		//each word has a influence and money value, for now we are assuming all influence is 1, so just ++
		currentScoreInf++;
		advanceBar();
		//add unique money value based on word
		currentScoreMon += fWordsMVal [wordIndex];
		//instead of being display in a bar, money is displayed right now as a text, so update it
		totlMonTxt.text=currentScoreMon + "$";

		polAnimate("talk");
		print("clicked. score is " + currentScoreInf + " current money is " + currentScoreMon);
	}

	public void advanceBar() {
		float barPos = currentScoreInf / scoreGoalInf;
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

		//Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
		int wordIndex;
		wordIndex =Random.Range(0,11);

		addScore(wordIndex);
		spitWord (wordIndex);
		//print ("mouse pos " + mousePosition.x + " y " + mousePosition.y + " ");    

		/*to make it just work on clicking politcian
		if (hitCollider) {
			//print ("Hit " + hitCollider.transform.name + " x" + hitCollider.transform.position.x + " y " + hitCollider.transform.position.y);    

			if (hitCollider.transform.name == "politician") {
				addScore();
				spitWord ();
			}
		}
		*/
	}

	public void spitWord(int wordIndex)
	{
		fWordsPointer++;
		fWordsPointer = fWordsPointer%maxWords;
		//if (fWords[fWordsPointer]==null)
		//	print("is null");
		if (activWords!=maxWords)
			activWords++;
		fWordsRend [fWordsPointer].enabled = true;
		//fWordsRend[fWordsPointer].transform.position = new Vector3 (Random.Range(-25.0f, -15.0f),Random.Range(15.0f, 20.0f), 0);


		//int randInt =Random.Range(0,11);
		fWordsRend[fWordsPointer].sprite = wordSprites [wordIndex];
		fWordsMoneyPop[fWordsPointer]= fWordsMVal [wordIndex];
		//currentScoreMon += fWordsMoneyPop [fWordsPointer];


		fWordsRend[fWordsPointer].transform.position = new Vector3 (-200F,9F, 0);
		fWordsTime[fWordsPointer]= Time.time+2;
		fWordsHForce[fWordsPointer]=Random.Range(50f, 200f);
		fWordsVForce[fWordsPointer]=Random.Range(0f, 30.0f);
		//initalize a new word to fall, add it to array.
	}

	public void wordFall()
	{
		int start = fWordsPointer -activWords +1;
		int killed = 0;
		if (start < 0)
			start = start + maxWords;

		//print("start is " + start);

		for (int i = 0; i <activWords; i++) {
			//print ("in loop. current word is " + activWords +" index is " + fWordsPointer);
			if (fWordsTime [start] < Time.time) {
				//print ("killing index " + start);
				killWord (start);
				killed++;
			} else {
				fWordsHForce [start]=fWordsHForce [start]*.7F;
				fWordsVForce [start] = fWordsVForce [start] - 2F;
				if (fWordsVForce [start] + fWordsRend [start].transform.position.y<=-120f) {
					if (!(fWordsRend [start].transform.position.y == -120))
						/* float downwards =fWordsRend [start].transform.position = fWordsRend [start].transform.position + new Vector3 (fWordsHForce [start],-1f,0);*/				
						fWordsRend [start].transform.position = fWordsRend [start].transform.position + new Vector3 (fWordsHForce [start],(fWordsRend [start].transform.position.y*-1)-120f,0);
					else {
						fWordsVForce [start] = (fWordsVForce [start])*-.6F;
						fWordsRend [start].transform.position = fWordsRend [start].transform.position + new Vector3 (fWordsHForce [start],fWordsVForce[start],0);

					}
				}

				else	
					fWordsRend [start].transform.position = fWordsRend [start].transform.position + new Vector3 (fWordsHForce [start],fWordsVForce[start],0);

				//move object
			}


			if (start == maxWords-1)
				start = 0;
			else
				start++;
		}
		activWords = activWords - killed;
		//makes all sprites in array fall. 
		//if they are to expire, remove them from array.
	}
	public void killWord(int index)
	{
		//Destroy (fWords [index]);
		//fWords [index] = null;
		fWordsRend[index].enabled=false;
		displayMoney (fWordsRend[index].transform.position,fWordsMoneyPop[index]);
		//fWordsTime[index]=0;
		//fWordsHForce[index]=0;
		//fWordsVForce[index]=0;
	}

	public void displayMoney(Vector3 pos, float scoreindex)
	{
		monTxt.text = "+" + scoreindex;
		monTxt.transform.position = pos + new Vector3 (70,0,0);
		killMon = Time.time+.9f;
		StartCoroutine (killMoney ());
	}

	IEnumerator killMoney()
	{
		yield return new WaitForSeconds(1);
		print ("time is " + Time.time + " killmon is "+ killMon);
		if (killMon<=Time.time)
			monTxt.text = "";

	}


}