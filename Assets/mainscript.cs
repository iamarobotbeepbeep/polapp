using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class mainscript : MonoBehaviour
{

	bool lvlTimer = false,isScrolling=false,falling=false,
	mouseHeld=false,tickInflated=false,tickTransitioning=false,
	inCShop=false,inPShop=false,dbleClick=false;

	int currentLvl,subLvl,maxWords,fWordsPointer,activWords,scndLeftTick;

	float startTime,timeLimit,currentCampaignScore,
	curFund,currentScorePrem,campaignScoreGoal,scrollAmount,killMon,polInf,bossInf,
	cshMltplr,artic;
	float[] fWordsTime,fWordsHForce,fWordsVForce,fWordsMoneyPop;

	Sprite[] wordSprites,bgSprites,lisaSprites,algSprites,grimSprites;

	Text monTxt, lvlTxt, totlMonTxt,infStat,fundStat,goldStat,artStat;
	Text[] tickerText;

	GameObject[] fWords;
	GameObject tickerObj,menu,eCanvasBox;

	SpriteRenderer bgRenderer,politician;
	SpriteRenderer[] fWordsRend;

	Animator barAnim, polAnim; 
	string[] tickerStrings;
	Word[] premWords,cashWords,myWords;
	Collider2D heldObj=null;
	Vector3 mouseOrig;

	void Start()
	{
		//initiate some variables


		//ints
		maxWords = 50;
		fWordsPointer = 0;
		activWords = 0;
		currentLvl = 1;
		subLvl = 0;
		scndLeftTick=0;

		//floats
		killMon = 0f;
		fWordsTime = new float[maxWords];
		fWordsHForce = new float[maxWords];
		fWordsVForce = new float[maxWords];
		fWordsMoneyPop = new float[maxWords]; 
		curFund = 0f;
		currentScorePrem = 500f;
		artic = 1;
		//polInf = 10f;
		cshMltplr=1;
		polInf=charselect.getInf();
		bossInf = 0f;

		//text variables
		monTxt = GameObject.Find("monPop").GetComponent<Text>();
		lvlTxt = GameObject.Find("level").GetComponent<Text>();
		totlMonTxt = GameObject.Find("money").GetComponent<Text>();
		tickerText = new Text[2];
		tickerText[0] = GameObject.Find("ticker1").GetComponent<Text>();
		tickerText[1] = GameObject.Find("ticker2").GetComponent<Text>();
		fundStat= GameObject.Find("statFund").GetComponent<Text>();
		fundStat.text = "Fund " + curFund;
		goldStat= GameObject.Find("statGold").GetComponent<Text>();
		goldStat.text = "Gold " + currentScorePrem;
		artStat= GameObject.Find("statArt").GetComponent<Text>();
		artStat.text = "Art " + artic;
		infStat= GameObject.Find("statInf").GetComponent<Text>();
		infStat.text = "Inf " + polInf;

		//game objects
		tickerObj = GameObject.Find ("ticker");
		eCanvasBox=GameObject.Find("editCanvas");
		menu=GameObject.Find ("menu");
		fWords = new GameObject[maxWords];


		//spritesheets
		wordSprites = Resources.LoadAll<Sprite>("Sprites/wordicons");
		bgSprites = Resources.LoadAll<Sprite>("Sprites/BGS");
		lisaSprites = Resources.LoadAll<Sprite>("Sprites/lisa");
		grimSprites = Resources.LoadAll<Sprite>("Sprites/grimwald");
		algSprites = Resources.LoadAll<Sprite>("Sprites/algernon");


		//spriterenderers
		bgRenderer = GameObject.Find("bg").GetComponent<SpriteRenderer>();
		fWordsRend = new SpriteRenderer[maxWords];
		politician = GameObject.Find("politician").GetComponent<SpriteRenderer>();
		print(charselect.getString());
		if 	(charselect.getString()=="lisa") //check which character was picked
		{
			politician.sprite = lisaSprites [0];
		}


		//animators
		barAnim = GameObject.Find("bar").GetComponent<Animator>();
		polAnim = GameObject.Find("politician").GetComponent<Animator>();
		if (charselect.getString () == "lisa") { //check which character was picked
			polAnim.Play("lisaIdleTran");
			GameObject.Find("charportrait").GetComponent<SpriteRenderer>().sprite=lisaSprites[13];
		}
		else if (charselect.getString () == "grim") { //check which character was picked
			polAnim.Play("grimIdleTran");
			GameObject.Find("charportrait").GetComponent<SpriteRenderer>().sprite=grimSprites[11];

		}
		else if (charselect.getString () == "alg") { //check which character was picked
			polAnim.Play("algIdleTran");
			GameObject.Find("charportrait").GetComponent<SpriteRenderer>().sprite=algSprites[17];

		}




		//strings
		tickerStrings= new string[16];
		initTick ();


		//end variable initiations

		initWord (); //set up word arrays, cash words, prem words, and my words

		//init words, i should redo this with a new system.
		for (int i = 0; i < fWords.Length; i++)
		{

			fWords[i] = new GameObject("word" + i);
			fWords[i].AddComponent<SpriteRenderer>();
			fWordsRend[i] = fWords[i].GetComponent<SpriteRenderer>();
			fWordsRend[i].enabled = false;
			fWordsRend[i].sprite = wordSprites[0];
			fWordsRend[i].transform.localScale = new Vector3(3, 3, 1);
		}


		//menu.SetActive (false); //should think of a way to not need this

		initLvlTimer();
		print("Game initiated.");

	}//end of start()

	public void Update() //update looks for mouse clicks, and handles them.
	{
		if (Input.GetMouseButtonDown (0) && !tickInflated){
			//regular click, menu is not open.
			click ();
		}
		else if (Input.GetMouseButtonDown (0) && mouseHeld == false) {
			//regular click, menu is open. mouse is in 'held down' state.
			mouseHeld = true;
			mouseOrig = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			heldObj = Physics2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		} else if (Input.GetMouseButtonUp(0)==false && heldObj!=null) {
			//mouse is held. use it to scroll menu.
			scrollAmount=Camera.main.ScreenToWorldPoint (Input.mousePosition).y-mouseOrig.y;
			if (scrollAmount > 5f)
				scrollAmount = 5f;
			else if (scrollAmount < -5f)
				scrollAmount = -5f;
			eCanvasBox.transform.localPosition+= new Vector3(0,scrollAmount,0);

		}
		else if (Input.GetMouseButtonUp(0)&&mouseHeld==true){
			//mouse button is released in menu.
			mouseHeld=false;
			clickShop (Physics2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition)));
			heldObj = null;

		}
	} //end update()

	public void FixedUpdate() //handles level timers and checks win condition.
	{
		if (lvlTimer)//if level timer running
		{
			if (Time.time < timeLimit)//if timelimit not expired
			{
				if (currentCampaignScore >= campaignScoreGoal)//if user reached score threshold
				{
					lvlTimer = false; //turn off timer, advance level and sublevel
					if (subLvl == 5)//each level has 5 sublevels.
					{
						currentLvl++;
						subLvl = 0;
						lvlTxt.text = currentLvl + "." + subLvl;
						bgRenderer.sprite = bgSprites [currentLvl];
					}
					else
					{
						subLvl++;
						lvlTxt.text = currentLvl + "." + subLvl;
					}
					print("you win. moving to level " + currentLvl + "." + subLvl);

					initLvlTimer();//level automatically starts, should change this.
					//i dont like how level progression is seperate from the bar.
					//this should be redone as invoke repeating
					//the advance bar should return a true/false i think
					//examples of how to call invoke
					//InvokeRepeating ("wordFall", 0.1f, 0.03f);
					//CancelInvoke("wordFall");

				}

			}
			else //if time ran out
			{
				lvlTimer = false;
				print("you lose, restarting level.");
				initLvlTimer();
				advanceBar();
			}
		}
		//scroll tickers and reinstate them
		if (tickerText [scndLeftTick].rectTransform.position.x <= -10F) {
			//if the trailing (rightmost ticker) has reached leftmost part of screen
			//reinstate the leftmost ticker, move it to the right and retext it
			renewTick();
		} else {
			//otherwise scroll
			tickerText[0].transform.position+= new Vector3(-3,0,0);
			tickerText[1].transform.position+= new Vector3(-3,0,0);

		}

	} //end fixed update


	public void initLvlTimer()//starts level timer based on level
	{
		startGameScore(); //restarts score needed to win level
		lvlTimer = true;

		if (subLvl == 5) //only sublevel 5 has a time for now
			timeLimit = 30;
		else
			timeLimit = 9999;

		startTime = Time.time;
		timeLimit = startTime + timeLimit;

	}

	public void startGameScore()
	{
		currentCampaignScore = 0;
		campaignScoreGoal = (currentLvl * 5) + (subLvl * 5);

	}

	public void addScore(int wordIndex)//adds to current score or money
	{

		//add campaign score value (CSV) times articulation
		//arituclation determined by level, clothes, and powerups
		currentCampaignScore += myWords[wordIndex].getCSV()*artic;;
		advanceBar();

		//add unique money value based on word times cash multiplier
		//cash multiplier determined by powerups, clothes
		curFund += myWords[wordIndex].getMVal()*cshMltplr;
		totlMonTxt.text = "$" + curFund; //money display

		polAnimate("Talk");//make animation set to talk
		//print("clicked. score is " + currentCampaignScore + " current money is " + curFund);
	}

	public void advanceBar() //CS (campaign score) is displayed by a bar
	{
		float barPos = currentCampaignScore / campaignScoreGoal;

		if (barPos >= 1) //if bar is full, score reached
		{
			barAnim.Play("barfull");
		}
		else //else choose animation frame based on ratio of curscore to goal score
			barAnim.Play("barfill", -1, barPos);
	}

	public void polAnimate(string state) //change an animation to 'state'
	{

		if (charselect.getString () + state=="lisaTalk")
			polAnim.Play ("lisaTalk");
		else if (charselect.getString () + state=="grimTalk")
			polAnim.Play ("grimTalk");
		else if (charselect.getString () + state=="algTalk")
			polAnim.Play ("algTalk");
		//if (state == "talk")
		//{
		//    polAnim.Play("talk");
		//}
	}

	public void clickShop(Collider2D releasedObj)
	{


		if (heldObj == null || releasedObj == null) {
			if (heldObj == null && releasedObj == null) {
				if (tickTransitioning == false&&tickInflated==true) {
					inflateDeflate ();

					//reset scrolling
					eCanvasBox.transform.position = GameObject.Find ("prefabs").transform.position;
				}
			} else
				return;
		}
		else if (heldObj != releasedObj) {
			return;
		} else {
			if (releasedObj.transform.name == "premium") {

				populateShop ("p");

			} else if (releasedObj.transform.name == "cash") {
				populateShop ("c");
			} else if (releasedObj.transform.name == "reset") {
				resetChar ();

			}

			else if (inPShop) {
				if (releasedObj.transform.name.Contains("lvl"))
				{
					string wrdindex = releasedObj.transform.name.Substring (3);
					int intindex = int.Parse (wrdindex);
					purchaseLvl (premWords[intindex],"p");
					releasedObj.transform.gameObject.GetComponent<Animator> ().Play ("lvlfill", -1, premWords [intindex].getlvl() / 5f);
					//barAnim.Play("barfill", -1, barPos);
				}
			}
			else if (inCShop) {
				if (releasedObj.transform.name.Contains("lvl"))
				{
					string wrdindex = releasedObj.transform.name.Substring (3);
					int intindex = int.Parse (wrdindex);

					//print ("you are buying " + cashWords [intindex].getWordName ());

					purchaseLvl (cashWords[intindex],"c");
					releasedObj.transform.gameObject.GetComponent<Animator> ().Play ("lvlfill", -1, cashWords [intindex].getlvl() / 5f);
					//barAnim.Play("barfill", -1, barPos);
				}
			}




		}
	}


	public void click()
	{
		//handles clicking
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

		//print ("colider name is " + hitCollider.transform.name);

		//print ("mouse pos " + mousePosition.x + " y " + mousePosition.y + " ");    
		//print (hitCollider.transform.name);
		if (hitCollider == null) {

			int wordIndex; //right now i dont know how words will be chosen, so random
			wordIndex = Random.Range (0, myWords.Length);

			addScore (wordIndex); //adds score
			spitWord (wordIndex); //make the word spit animation
			if (dbleClick == true) {
				addScore (wordIndex);
				spitWord (wordIndex);
			}

		} else if (hitCollider.transform.name == "ticker") {
			inflateDeflate ();
		}
		//end method
	}

	public void inflateDeflate()
	{

		if (tickTransitioning) {
			if (tickInflated) {//tick is shrinking right now, make it stop shrinking and expand
				tickInflated = false;
				CancelInvoke ("deflateTicker");
				InvokeRepeating ("inflateTicker", 0.1f, 0.03f);			
			} else {//tick is growing right now, make it stop growing and shrink
				tickInflated = true;
				CancelInvoke ("inflateTicker");
				InvokeRepeating ("deflateTicker", 0.1f, 0.03f);			
			}

		} else if (tickInflated) {//tick is big, set tick to shrink
			tickTransitioning = true;
			inPShop = false;
			inCShop = false;
			//
			//exampleshop.enabled = false;
			//menu.SetActive (false);
			//

			int childcount = GameObject.Find ("editCanvas").transform.childCount;
			for (int i = 0; i < childcount; i++) {
				Destroy (GameObject.Find ("editCanvas").transform.GetChild (i).gameObject);
			}


			InvokeRepeating ("deflateTicker", 0.1f, 0.03f);	
		} else {//tick is small, set tick to grow
			tickTransitioning = true;

			//
			//exampleshop.enabled = false;
			//menu.SetActive (false);
			//

			InvokeRepeating ("inflateTicker", 0.1f, 0.03f);	
		}

		//end method
	}

	public void populateShop(string type)
	{
		float yoffset = 90f;
		if (type == "p") {

			print ("in premium shop");
			inPShop = true;
			menu.SetActive (false);
			Text prefabName = GameObject.Find ("prefabName").GetComponent<Text> ();
			Text prefabCost = GameObject.Find ("prefabCost").GetComponent<Text> (); 
			SpriteRenderer prefabIcon = GameObject.Find ("prefabIcon").GetComponent<SpriteRenderer> ();
			SpriteRenderer prefabLvl = GameObject.Find ("prefabLvl").GetComponent<SpriteRenderer> ();

			for (int i=0; i<premWords.Length;i++)
			{
				print ("iteration is " + i);

				SpriteRenderer tempIcon = Instantiate (prefabIcon, prefabIcon.transform.localPosition-new Vector3(0,i*yoffset,0), prefabIcon.transform.rotation) as SpriteRenderer;
				tempIcon.enabled = true;
				tempIcon.sprite = premWords [i].getSprite();
				tempIcon.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				Text tempTextName = Instantiate (prefabName, prefabName.transform.localPosition-new Vector3(0,i*yoffset,0), prefabName.transform.rotation) as Text;
				tempTextName.text = premWords[i].getWordName();
				tempTextName.enabled = true;
				tempTextName.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				SpriteRenderer tempLvl = Instantiate (prefabLvl, prefabLvl.transform.localPosition-new Vector3(0,i*yoffset,1), prefabLvl.transform.rotation) as SpriteRenderer;
				tempLvl.enabled = true;
				tempLvl.transform.name = "lvl"+i;
				tempLvl.gameObject.GetComponent<Animator> ().Play ("lvlfill", -1, premWords [i].getlvl() / 5f);

				tempLvl.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				Text tempTextCost = Instantiate (prefabCost, prefabCost.transform.localPosition-new Vector3(0,i*yoffset,0), prefabCost.transform.rotation) as Text;
				tempTextCost.text = "upgrade $" + premWords[i].getCost();
				tempTextCost.enabled = true;
				tempTextCost.transform.SetParent (GameObject.Find ("editCanvas").transform, false);

			}//end ploop
		}//end p if


		if (type == "c") {

			print ("in cash shop");
			inCShop = true;
			menu.SetActive (false);
			Text prefabName = GameObject.Find ("prefabName").GetComponent<Text> ();
			Text prefabCost = GameObject.Find ("prefabCost").GetComponent<Text> (); 
			SpriteRenderer prefabIcon = GameObject.Find ("prefabIcon").GetComponent<SpriteRenderer> ();
			SpriteRenderer prefabLvl = GameObject.Find ("prefabLvl").GetComponent<SpriteRenderer> ();

			for (int i=0; i<cashWords.Length;i++)
			{
				print ("iteration is " + i);

				SpriteRenderer tempIcon = Instantiate (prefabIcon, prefabIcon.transform.localPosition-new Vector3(0,i*yoffset,-1), prefabIcon.transform.rotation) as SpriteRenderer;
				tempIcon.enabled = true;
				tempIcon.sprite = cashWords [i].getSprite();
				tempIcon.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				Text tempTextName = Instantiate (prefabName, prefabName.transform.localPosition-new Vector3(0,i*yoffset,-1), prefabName.transform.rotation) as Text;
				tempTextName.text = cashWords[i].getWordName();
				tempTextName.enabled = true;
				tempTextName.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				SpriteRenderer tempLvl = Instantiate (prefabLvl, prefabLvl.transform.localPosition-new Vector3(0,i*yoffset,1), prefabLvl.transform.rotation) as SpriteRenderer;
				tempLvl.enabled = true;
				tempLvl.transform.name = "lvl"+i;
				tempLvl.gameObject.GetComponent<Animator> ().Play ("lvlfill", -1, cashWords [i].getlvl() / 5f);

				tempLvl.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				Text tempTextCost = Instantiate (prefabCost, prefabCost.transform.localPosition-new Vector3(0,i*yoffset,-1), prefabCost.transform.rotation) as Text;
				tempTextCost.text = "upgrade $" + cashWords[i].getCost();
				tempTextCost.enabled = true;
				tempTextCost.transform.SetParent (GameObject.Find ("editCanvas").transform, false);

			}//end ploop
		}//end p if
	}

	public void purchaseLvl(Word purchaseWord, string type)
	{
		if (type == "p") {
			print ("in purchase level method premium. word being purchased is " + purchaseWord.getWordName ());


			float cost = purchaseWord.getCost ();
			if (purchaseWord.getlvl () == 0f) {
				cost = purchaseWord.getCost ();
				if (currentScorePrem > cost) {
					purchaseWord.initLvl ();
					Word[] temparray = new Word [myWords.Length + 1];
					for (int i = 0; i < myWords.Length; i++) {
						temparray [i] = myWords [i];
					}
					temparray [myWords.Length] = purchaseWord;
					myWords = temparray;
					currentScorePrem -= cost;
					print ("bought level, premimum is now " + currentScorePrem);
				} else {
					print ("not enough premium to buy skill");
				}

			} else if (purchaseWord.getlvl () >= 4f) {
				print ("max level");
				return;
			} else if (currentScorePrem > cost) {
				currentScorePrem -= cost;
				purchaseWord.raiseLvl ();
				print ("raised level, premimum is now " + currentScorePrem);
			}
			else {
				print ("not enough premium to upgrade skill");
			}
		}//end of p if

		if (type == "c") {
			print ("in purchase level method cash. word being purchased is " + purchaseWord.getWordName ());


			float cost = purchaseWord.getCost () * purchaseWord.getlvl ();
			if (purchaseWord.getlvl () == 0f) {
				cost = purchaseWord.getCost ();
				if (curFund > cost) {
					purchaseWord.initLvl ();
					Word[] temparray = new Word [myWords.Length + 1];
					for (int i = 0; i < myWords.Length; i++) {
						temparray [i] = myWords [i];
					}
					temparray [myWords.Length] = purchaseWord;
					myWords = temparray;
					curFund -= cost;

					//print ("bought word " + purchaseWord.getWordName ());

					print ("bought level, cash is now " + curFund);

				}else {
					print ("not enough cash to buy skill");
				}

			} else if (purchaseWord.getlvl () >= 4f) {
				print ("max level");
				return;
			} else if (curFund > cost) {
				curFund -= cost;
				purchaseWord.raiseLvl ();
				print ("raised level, cash is now " + curFund);
			}
			else {
				print ("not enough cash to upgrade skill");
			}
		}//end of p if




	}

	public void inflateTicker()
	{
		if (menu.transform.localPosition.y <-150)
			menu.transform.localPosition += new Vector3 (0, 10, 0);
		else {
			CancelInvoke ("inflateTicker");
			tickInflated = true;
			tickTransitioning = false;
			//
			//exampleshop.enabled = true;
			//.SetActive (true);
			//
		}
	}

	public void deflateTicker()
	{
		if (menu.transform.localPosition.y > -435)
			menu.transform.localPosition += new Vector3 (0, -10, 0);
		else {
			CancelInvoke ("deflateTicker");
			tickInflated = false;
			tickTransitioning = false;
		}
	}



	public void spitWord(int wordIndex)
	{
		//i only need to really keep track of how many active words i have


		//spitwords is complicated
		//it has a bunch of arrays, and a 'pointer' so it can cycle through the array faster

		fWordsPointer++; //move the pointer up for this new word
		fWordsPointer = fWordsPointer % maxWords; //make sure pointer doesnt exceed array bounds

		if (activWords != maxWords) //only 'maxwords' amount of words can fall at once
			activWords++;
		else if (activWords == maxWords) {
			killWord (fWordsPointer);	
		}

		fWordsRend[fWordsPointer].enabled = true;

		fWordsRend[fWordsPointer].sprite = myWords[wordIndex].getSprite();
		fWordsMoneyPop[fWordsPointer] = myWords[wordIndex].getMVal();



		fWordsRend[fWordsPointer].transform.position = new Vector3(-200F, 9F, 0);
		fWordsTime[fWordsPointer] = Time.time + 2;
		fWordsHForce[fWordsPointer] = Random.Range(50f, 200f);
		fWordsVForce[fWordsPointer] = Random.Range(0f, 30.0f);
		//initalize a new word to fall, add it to array.

		if (!falling) {
			falling = true;
			InvokeRepeating ("wordFall", 0.1f, 0.03f);
		}
		//InvokeRepeating("LaunchProjectile", 2.0f, 0.3f);

	}
	void wordFall()
	{
		if (activWords == 0) {
			CancelInvoke("wordFall");
			falling = false;
		}

		//print ("test");
		int start = fWordsPointer - activWords + 1;
		int killed = 0;
		if (start < 0)
			start = start + maxWords;

		//print("start is " + start);

		for (int i = 0; i < activWords; i++)
		{
			//print ("in loop. current word is " + activWords +" index is " + fWordsPointer);
			if (fWordsTime[start] < Time.time)
			{
				//print ("killing index " + start);
				killWord(start);
				killed++;
			}
			else
			{
				fWordsHForce[start] = fWordsHForce[start] * .7F;
				fWordsVForce[start] = fWordsVForce[start] - 2F;
				if (fWordsVForce[start] + fWordsRend[start].transform.position.y <= -120f) {
					if (!(fWordsRend[start].transform.position.y == -120))
						/* float downwards =fWordsRend [start].transform.position = fWordsRend [start].transform.position + new Vector3 (fWordsHForce [start],-1f,0);*/
						fWordsRend[start].transform.position = fWordsRend[start].transform.position + new Vector3(fWordsHForce[start], (fWordsRend[start].transform.position.y * -1) - 120f, 0);
					else
					{
						fWordsVForce[start] = (fWordsVForce[start]) * -.6F;
						fWordsRend[start].transform.position = fWordsRend[start].transform.position + new Vector3(fWordsHForce[start], fWordsVForce[start], 0);

					}
				} else
					fWordsRend[start].transform.position = fWordsRend[start].transform.position + new Vector3(fWordsHForce[start], fWordsVForce[start], 0);

				//move object
			}


			if (start == maxWords - 1)
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
		fWordsRend[index].enabled = false;
		StartCoroutine(displayMoney(fWordsRend[index].transform.position, fWordsMoneyPop[index]));
		//fWordsTime[index]=0;
		//fWordsHForce[index]=0;
		//fWordsVForce[index]=0;
	}







	IEnumerator displayMoney(Vector3 pos, float scoreindex)
	{
		Text displayMoney = Instantiate (monTxt, pos + new Vector3 (70, 0, 0), monTxt.transform.rotation) as Text;

		displayMoney.text = "$" + scoreindex;
		displayMoney.transform.SetParent (GameObject.Find ("gameui").transform, false);
		//monTxt.text = "+" + scoreindex;

		yield
		return new WaitForSeconds(1);
		Destroy (displayMoney);
		//print("time is " + T	ime.time + " killmon is " + killMon);
		//if (killMon <= Time.time)
		//    monTxt.text = "";

	}

	/****menu methods***/
	/**ticker methods***/

	public void initTick() //fill ticker string array
	{
		//jokes
		tickerStrings [0] = "People upset that too many people upset";
		tickerStrings [1] = "Earth to transition to people being 'just a friend'; new koala love interest";
		tickerStrings [2] = "Voting centers urge young adults to not take stupid pictures in voting booths";
		tickerStrings [3] = "Local youth center grows up into middle-aged disenfranchised mini-mall";
		//status report
		tickerStrings [4] = "New politician emerges, dwarfed by news of new Arby's opening in Denver";
		tickerStrings [5] = "New politician makes waves, but not as big of a wave as new tidal wave that hit Arby's in Denver";
		tickerStrings [6] = "Hot new politician taking on the establishment -- white house officials quote \"he's so hot right now\"";
		tickerStrings [7] = "Tyrannical new politican/overlord feared yet respected by cowering community";
		//hints
		tickerStrings [8] = "Trade cash to upgrade words, work smarter not harder. Or really, probably do both, like a decent respected professional";
		tickerStrings [9] = "Sometimes the news can hint to which words will be more effective";
		tickerStrings [10] = "Tap faster to win faster";
		tickerStrings [11] = "Make sure to take frequent bathroom breaks. Its a phone game after all";
		//events
		tickerStrings [12] = "Global warming all the rage. Literally, as wild badgers madly flee dying forests";
		tickerStrings [13] = "Money -- people wish they had more of it while working same amount.";
		tickerStrings [14] = "Citizens bored of hearing the word 'no', they want a politician who can say \"Yes!\" or at least \"si\"";
		tickerStrings [15] = "You have been caught cheating on your spouse with iguana, showing your true colors!";


	} //end initiate ticker strings

	public void renewTick()//reposition and reinitiate tickers
	{
		//get index of ticker we are going to reset
		int tickIndex = scndLeftTick - 1;
		if (tickIndex == -1)
			tickIndex = tickerText.Length - 1;

		//chose a random into for our new random string to fill ticker
		int randomint = Random.Range(0,tickerStrings.Length); 

		//set width to fit new text. i think 100 width is like 5 letters
		tickerText [tickIndex].rectTransform.sizeDelta = new Vector2 (tickerStrings [randomint].Length*30, 46);

		//calculate its new x. trailling tickers x + length.
		float startx = tickerText[scndLeftTick].rectTransform.position.x;
		float length = tickerText [scndLeftTick].rectTransform.sizeDelta.x;

		//unforuatnely startx is middle of ticker. so its really x + 1/2length
		float startPoint = startx + length / 2;

		//which means our new x should be our start x, plus half its new length.
		float startPointReal = startPoint + tickerText [tickIndex].rectTransform.sizeDelta.x / 2;
		tickerText [tickIndex].transform.position = new Vector3 (startPointReal, tickerText [tickIndex].transform.position.y, 0);


		//now finally place new text inside ticker
		tickerText [tickIndex].text = tickerStrings [randomint];

		//now set it as the new 'trailing' ticker
		scndLeftTick++;
		if (scndLeftTick >= tickerText.Length) {
			scndLeftTick = 0;
		}

	}//end renewTick


	/***word methods**/
	public void initWord ()
	{
		float premCost=5; //default cost to buy prem
		float cashCost = 5; //default cost to buy cash
		float basicCost = 1; //default cost to buy basic
		float defMVal = 1; //default money value
		float defCSV = 1; //default campaign score value
		premWords = new Word[3]{new Word("terror",wordSprites[5],premCost,0f,defMVal,defCSV),new Word("usa",wordSprites[6],premCost,0f,defMVal,defCSV),new Word("cash3",wordSprites[7],premCost,0f,defMVal,defCSV)};
		cashWords = new Word[8]{new Word("ok",wordSprites[0],basicCost,1,defMVal,defCSV),new Word("no",wordSprites[1],basicCost,1,defMVal,defCSV),new Word("well",wordSprites[2],basicCost,1,defMVal,defCSV),new Word("pssh",wordSprites[3],basicCost,1,defMVal,defCSV),new Word("yes",wordSprites[4],basicCost,1,defMVal,defCSV),new Word("cash1",wordSprites[8],cashCost,0,defMVal,defCSV),new Word("terrific",wordSprites[9],cashCost,0,defMVal,defCSV),new Word("global",wordSprites[10],cashCost,0,defMVal,defCSV)};
		myWords  = new Word[5] {cashWords[0],cashWords[1],cashWords[2],cashWords[3],cashWords[4]};
	}

	//powerups

	IEnumerator activatePowerup(string powerup, float time){

		if (powerup == "bonusCash") {
			cshMltplr +=1;
			yield
			return new WaitForSeconds (time);
			cshMltplr -=1;
		}
		else if (powerup == "doubleClick") {
			dbleClick = true;
			yield
			return new WaitForSeconds (time);
			dbleClick = false;

		} 

	}



	//powerups

	//reset character
	void resetChar()
	{
		/*
		lvlTimer = 0;
		polInf = currentLvl / 10 + bossInf;
		curFund = 0;
		currentLvl = 1;
		subLvl = 1;
		deflateTicker();
		initLvlTimer;
*/     
		float newInf = currentLvl / 10 + bossInf + 10;
		charselect.setInf (newInf);
		Scene scene = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (scene.name);
		//deflate shop
		//reset level

		//influence is level/10 plus character level/10 plus boss influence
	}
	//reset character end

} //end of mainscript class

public class Word
{
	string wordName;
	Sprite sprite;
	float cost;
	float lvl;
	float mVal;
	float CSV;

	public Word(string iWordName, Sprite iSprite, float iCost, float iLvl, float iMVal, float iCSV)
	{
		wordName = iWordName;
		sprite = iSprite;
		cost = iCost;
		lvl = iLvl;
		mVal = iMVal;
		CSV = iCSV; 

	}

	public float getMVal()
	{
		return mVal;
	}

	public float getCSV()
	{
		return CSV;
	}

	public Sprite getSprite()
	{
		return sprite;
	}
	public string getWordName()
	{
		return wordName;
	}
	public float getlvl()
	{
		return lvl;
	}
	public float getCost()
	{
		return cost;
	}

	public void initLvl()
	{
		lvl++;
	}
	public void raiseLvl()
	{
		lvl++;
		mVal ++;
		CSV ++;
		cost = cost * 2;
	}
}