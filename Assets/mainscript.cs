using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class mainscript : MonoBehaviour
{

    float startTime;
    float timeLimit;
    int currentLvl;
    int subLvl;
	bool lvlTimer = false,isScrolling=false,falling=false,mouseHeld=false,tickInflated=false,tickTransitioning=false,inCShop=false,inPShop=false;
    float currentScoreInf;
    float currentScoreMon;
	float currentScorePrem;
    float scoreGoalInf;
    Animator barAnim, polAnim; 
	Sprite[] wordSprites,bgSprites;
    Text monTxt, lvlTxt, totlMonTxt/*premTxt,cashTxt*/;
    int maxWords = 5;
    int fWordsPointer = 0;
    int activWords = 0;
	GameObject[] fWords;
	GameObject tickerObj,menu,eCanvasBox;
    float[] fWordsTime;
    float[] fWordsHForce;
    float[] fWordsVForce;
    float[] fWordsMoneyPop;
	float scrollAmount;
    SpriteRenderer[] fWordsRend;
    float killMon = 0f;
    string[] tickerStrings;
    Text[] tickerText;
	int scndLeftTick=0;
	Word[] premWords;
	Word[] cashWords;
	Word[] myWords;
	SpriteRenderer bgRenderer;
	Collider2D heldObj=null;
	Vector3 mouseOrig;
	//SpriteRenderer exampleshop;

 void Start()
    {
		//exampleshop = GameObject.Find ("exampleshop").GetComponent<SpriteRenderer>();

        tickerText = new Text[2];
        //get our text for ui components: level, money, ticker, etc
        monTxt = GameObject.Find("monPop").GetComponent<Text>();
        lvlTxt = GameObject.Find("level").GetComponent<Text>();
        totlMonTxt = GameObject.Find("money").GetComponent<Text>();
		tickerObj = GameObject.Find ("ticker");
		eCanvasBox=GameObject.Find("editCanvas");
        tickerText[0] = GameObject.Find("ticker1").GetComponent<Text>();
        tickerText[1] = GameObject.Find("ticker2").GetComponent<Text>();

		//premTxt = GameObject.Find("premium").GetComponent<Text>();
		//cashTxt = GameObject.Find("cash").GetComponent<Text>();

        //get animators for our animated objects
        barAnim = GameObject.Find("bar").GetComponent<Animator>();
        polAnim = GameObject.Find("politician").GetComponent<Animator>();
		menu=GameObject.Find ("menu");
		menu.SetActive (false);

		bgRenderer = GameObject.Find("bg").GetComponent<SpriteRenderer>();
        //load spritesheets
        wordSprites = Resources.LoadAll<Sprite>("Sprites/wordicons");
		bgSprites = Resources.LoadAll<Sprite>("Sprites/BGS");

        //all variables related to falling words
        fWords = new GameObject[maxWords];
        fWordsTime = new float[maxWords];
        fWordsHForce = new float[maxWords];
        fWordsVForce = new float[maxWords];
        fWordsRend = new SpriteRenderer[maxWords];
        fWordsMoneyPop = new float[maxWords]; //money value that shows when words pop
        //fWordsMVal = new float[wordSprites.Length]; //how much a word is worth in money


        //for (int i = 0; i < fWordsMVal.Length; i++)
        //{
        //    fWordsMVal[i] = i;//for now just assigning arbitrairy money value to them
       // }
        ///initialize falling words gameobjects, position them and size them
        for (int i = 0; i < fWords.Length; i++)
        {

            fWords[i] = new GameObject("word" + i);
            fWords[i].AddComponent<SpriteRenderer>();
            fWordsRend[i] = fWords[i].GetComponent<SpriteRenderer>();
            fWordsRend[i].enabled = false;
            fWordsRend[i].sprite = wordSprites[0];
            fWordsRend[i].transform.localScale = new Vector3(3, 3, 1);
        }

		initWord ();
        currentLvl = 1;
        subLvl = 0;
        currentScoreMon = 0f;
		currentScorePrem = 500f;
        startLvlTimer();
        print("Game initiated.");

		///
	
		///
    }

    //update checks if someone clicks, 'click()' method handles what was clicked
    public void Update()
	{
		if (Input.GetMouseButtonDown (0) && !tickInflated)
		{		click ();
		//print ("click");
	}
		else if (Input.GetMouseButtonDown (0) && mouseHeld == false) {
			//print ("hold");
			mouseHeld = true;
			mouseOrig = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			heldObj = Physics2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		} else if (Input.GetMouseButtonUp(0)==false && heldObj!=null) {
			//print ("scroll");
			scrollAmount=Camera.main.ScreenToWorldPoint (Input.mousePosition).y-mouseOrig.y;
			if (scrollAmount > 5f)
				scrollAmount = 5f;
			else if (scrollAmount < -5f)
				scrollAmount = -5f;
				eCanvasBox.transform.localPosition+= new Vector3(0,scrollAmount,0);
		
		}
		else if (Input.GetMouseButtonUp(0)&&mouseHeld==true){
			//print ("release");
			mouseHeld=false;

				clickShop (Physics2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition)));
				heldObj = null;
			
		}
    }
  
    public void FixedUpdate()
    {
        if (lvlTimer)//if level has timer running
        {
            if (Time.time < timeLimit)//if theres still time left
            {
                if (currentScoreInf >= scoreGoalInf)//if user reached score threshold
                {
                    lvlTimer = false; //turn off timer, advance level and sublevel
                    if (subLvl == 5)
                    {
                        currentLvl++;
						bgRenderer.sprite = bgSprites [currentLvl];
                        subLvl = 0;
                        lvlTxt.text = currentLvl + "." + subLvl;
                    }
                    else
                    {
                        subLvl++;
                        lvlTxt.text = currentLvl + "." + subLvl;
                    }
                    print("you win. moving to level " + currentLvl + "." + subLvl);
                    startLvlTimer();
                }
  
				//this area should be redone into a coroutine, or invokerepeating
				//if (activWords != 0) //if there are falling words, make them fall
                //{
                //    wordFall();
                //}
            }
            else //if time ran out
            {
                lvlTimer = false;
                print("you lose, restarting level.");
                startLvlTimer();
                advanceBar();
            }
        }
		//two tickers, if second tickers x is at ticker start
		if (tickerText [scndLeftTick].transform.position.x <= 31F) {
			//make first ticker become trailling ticker
			initTickText();

		} else {
		//otherwise just push them both left
			tickerText[0].transform.position+= new Vector3(-1,0,0);
			tickerText[1].transform.position+= new Vector3(-1,0,0);

		}

    }

	public void initTickText()
	{
		//get index of ticker we are going to reset
		int tickIndex = scndLeftTick - 1;
		if (tickIndex == -1)
			tickIndex = tickerText.Length - 1;
		//calculate its new x. trailling x + length.
		float startx = tickerText[scndLeftTick].transform.position.x;
		float length = tickerText [scndLeftTick].GetComponentInParent<RectTransform> ().rect.width;
		//set our resetting ticker to that position
		tickerText [tickIndex].transform.position = new Vector3 (startx+length, tickerText [tickIndex].transform.position.y, 0);
	    //now signify it as the trailing ticker
		scndLeftTick++;
		if (scndLeftTick >= tickerText.Length) {
			scndLeftTick = 0;
		}
	}

    public void startLvlTimer()//starts level timer based on level
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
        currentScoreInf = 0;
        scoreGoalInf = (currentLvl * 5) + (subLvl * 5);

    }

    public void addScore(int wordIndex)//adds to current score or money
    {
        //each word has a influence and money value, for now we are assuming all influence is 1, so just ++

        //add influence
		currentScoreInf+=myWords[wordIndex].getIVal();
        advanceBar();

        //add unique money value based on word
		currentScoreMon += myWords[wordIndex].getMVal();
        totlMonTxt.text = "$" + currentScoreMon; //money display

        polAnimate("talk");//make animation set to talk
        //print("clicked. score is " + currentScoreInf + " current money is " + currentScoreMon);
    }

    public void advanceBar() //influence (level score) is displayed by a bar
    {
        float barPos = currentScoreInf / scoreGoalInf;

        if (barPos >= 1) //if bar is full, score reached
        {
            barAnim.Play("barfull");
        }
        else //else choose animation frame based on ratio of curscore to goal score
            barAnim.Play("barfill", -1, barPos);
    }

    public void polAnimate(string state) //change an animation to 'state'
    {
        if (state == "talk")
        {
            polAnim.Play("talk");
        }
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

				populateShop("p");

			} else if (releasedObj.transform.name == "cash") {
				populateShop("c");
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
			//spitWord(wordIndex); //makes words fall from mouth of politician
			spitWord (wordIndex);

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
			menu.SetActive (false);
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
			menu.SetActive (false);
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


				SpriteRenderer tempLvl = Instantiate (prefabLvl, prefabLvl.transform.localPosition-new Vector3(0,i*yoffset,0), prefabLvl.transform.rotation) as SpriteRenderer;
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

				SpriteRenderer tempIcon = Instantiate (prefabIcon, prefabIcon.transform.localPosition-new Vector3(0,i*yoffset,0), prefabIcon.transform.rotation) as SpriteRenderer;
				tempIcon.enabled = true;
				tempIcon.sprite = cashWords [i].getSprite();
				tempIcon.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				Text tempTextName = Instantiate (prefabName, prefabName.transform.localPosition-new Vector3(0,i*yoffset,0), prefabName.transform.rotation) as Text;
				tempTextName.text = cashWords[i].getWordName();
				tempTextName.enabled = true;
				tempTextName.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				SpriteRenderer tempLvl = Instantiate (prefabLvl, prefabLvl.transform.localPosition-new Vector3(0,i*yoffset,0), prefabLvl.transform.rotation) as SpriteRenderer;
				tempLvl.enabled = true;
				tempLvl.transform.name = "lvl"+i;
				tempLvl.gameObject.GetComponent<Animator> ().Play ("lvlfill", -1, cashWords [i].getlvl() / 5f);

				tempLvl.transform.SetParent (GameObject.Find ("editCanvas").transform, false);


				Text tempTextCost = Instantiate (prefabCost, prefabCost.transform.localPosition-new Vector3(0,i*yoffset,0), prefabCost.transform.rotation) as Text;
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
				if (currentScoreMon > cost) {
					purchaseWord.initLvl ();
					Word[] temparray = new Word [myWords.Length + 1];
					for (int i = 0; i < myWords.Length; i++) {
						temparray [i] = myWords [i];
					}
					temparray [myWords.Length] = purchaseWord;
					myWords = temparray;
					currentScoreMon -= cost;

					//print ("bought word " + purchaseWord.getWordName ());

					print ("bought level, cash is now " + currentScoreMon);

				}else {
					print ("not enough cash to buy skill");
				}

			} else if (purchaseWord.getlvl () >= 4f) {
				print ("max level");
				return;
			} else if (currentScoreMon > cost) {
				currentScoreMon -= cost;
				purchaseWord.raiseLvl ();
				print ("raised level, cash is now " + currentScoreMon);
			}
			else {
				print ("not enough cash to upgrade skill");
			}
		}//end of p if




	}

	public void inflateTicker()
	{
		if (tickerObj.transform.localScale.y < 620)
			tickerObj.transform.localScale += new Vector3 (0, 10, 0);
		else {
			CancelInvoke ("inflateTicker");
			tickInflated = true;
			tickTransitioning = false;
			//
			//exampleshop.enabled = true;
			menu.SetActive (true);
			//
		}
	}

	public void deflateTicker()
	{
		if (tickerObj.transform.localScale.y > 80)
			tickerObj.transform.localScale += new Vector3 (0, -10, 0);
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
				displayMoney(fWordsRend[index].transform.position, fWordsMoneyPop[index]);
				//fWordsTime[index]=0;
				//fWordsHForce[index]=0;
				//fWordsVForce[index]=0;
			}



    


public void displayMoney(Vector3 pos, float scoreindex)
{
    monTxt.text = "+" + scoreindex;
    monTxt.transform.position = pos + new Vector3(70, 0, 0);
    killMon = Time.time + .9f;
    StartCoroutine(killMoney());
}

IEnumerator killMoney()
{
    yield
    return new WaitForSeconds(1);
    //print("time is " + Time.time + " killmon is " + killMon);
    if (killMon <= Time.time)
        monTxt.text = "";

}

	public void initWord ()
	{
		//int premCount = 3, cashCount = 3;
		//premWords = new Word[premCount];
		//cashWords = new Word[cashCount];
		float premCost=5;
		float cashCost = 5;
		float basicCost = 1;
		float defMVal = 1;
		float defIVal = 1;
		premWords = new Word[3]{new Word("terror",wordSprites[5],premCost,0f,defMVal,defIVal),new Word("usa",wordSprites[6],premCost,0f,defMVal,defIVal),new Word("cash3",wordSprites[7],premCost,0f,defMVal,defIVal)};
		cashWords = new Word[8]{new Word("ok",wordSprites[0],basicCost,1,defMVal,defIVal),new Word("no",wordSprites[1],basicCost,1,defMVal,defIVal),new Word("well",wordSprites[2],basicCost,1,defMVal,defIVal),new Word("pssh",wordSprites[3],basicCost,1,defMVal,defIVal),new Word("yes",wordSprites[4],basicCost,1,defMVal,defIVal),new Word("cash1",wordSprites[8],cashCost,0,defMVal,defIVal),new Word("terrific",wordSprites[9],cashCost,0,defMVal,defIVal),new Word("global",wordSprites[10],cashCost,0,defMVal,defIVal)};
		//myWords  = new Word[3]{new Word("ok",wordSprites[0],basicCost,1),new Word("no",wordSprites[1],basicCost,1),new Word("well",wordSprites[2],basicCost,1),new Word("pssh",wordSprites[3],basicCost,1),new Word("yes",wordSprites[4],basicCost,1)};
		myWords  = new Word[5] {cashWords[0],cashWords[1],cashWords[2],cashWords[3],cashWords[4]};
	}


}

public class Word
{
	string wordName;
	Sprite sprite;
	float cost;
	float lvl;
	float mVal;
	float iVal;

	public Word(string iWordName, Sprite iSprite, float iCost, float iLvl, float iMVal, float iIVal)
	{
		wordName = iWordName;
		sprite = iSprite;
		cost = iCost;
		lvl = iLvl;
		mVal = iMVal;
		iVal = iIVal; 
		
	}

	public float getMVal()
	{
		return mVal;
	}

	public float getIVal()
	{
		return iVal;
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
		iVal ++;
		cost = cost * 2;
	}
}