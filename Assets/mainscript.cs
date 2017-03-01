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
    bool lvlTimer = false;
    float currentScoreInf;
    float currentScoreMon;
    float scoreGoalInf;
    Animator barAnim, polAnim; 
    Sprite[] wordSprites;
    Text monTxt, lvlTxt, totlMonTxt;
    int maxWords = 5;
    int fWordsPointer = 0;
    int activWords = 0;
    GameObject[] fWords;
    float[] fWordsTime;
    float[] fWordsHForce;
    float[] fWordsVForce;
    float[] fWordsMVal;
    float[] fWordsMoneyPop;
    SpriteRenderer[] fWordsRend;
    float killMon = 0f;
    string[] tickerStrings;
    Text[] tickerText;
	int scndLeftTick=0;

 void Start()
    {
        tickerText = new Text[2];
        //get our text for ui components: level, money, ticker, etc
        monTxt = GameObject.Find("monPop").GetComponent<Text>();
        lvlTxt = GameObject.Find("level").GetComponent<Text>();
        totlMonTxt = GameObject.Find("money").GetComponent<Text>();
        tickerText[0] = GameObject.Find("ticker1").GetComponent<Text>();
        tickerText[1] = GameObject.Find("ticker2").GetComponent<Text>();

        //get animators for our animated objects
        barAnim = GameObject.Find("bar").GetComponent<Animator>();
        polAnim = GameObject.Find("politician").GetComponent<Animator>();

        //load spritesheets
        wordSprites = Resources.LoadAll<Sprite>("Sprites/wordicons");

        //all variables related to falling words
        fWords = new GameObject[maxWords];
        fWordsTime = new float[maxWords];
        fWordsHForce = new float[maxWords];
        fWordsVForce = new float[maxWords];
        fWordsRend = new SpriteRenderer[maxWords];
        fWordsMoneyPop = new float[maxWords]; //money value that shows when words pop
        fWordsMVal = new float[wordSprites.Length]; //how much a word is worth in money


        for (int i = 0; i < fWordsMVal.Length; i++)
        {
            fWordsMVal[i] = i;//for now just assigning arbitrairy money value to them
        }
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

        currentLvl = 1;
        subLvl = 0;
        currentScoreMon = 0f;
        startLvlTimer();
        print("Game initiated.");
    }

    //update checks if someone clicks, 'click()' method handles what was clicked
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            click();
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
				if (activWords != 0) //if there are falling words, make them fall
                {
                    wordFall();
                }
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
        currentScoreInf++;
        advanceBar();

        //add unique money value based on word
        currentScoreMon += fWordsMVal[wordIndex];
        totlMonTxt.text = "$" + currentScoreMon; //money display

        polAnimate("talk");//make animation set to talk
        print("clicked. score is " + currentScoreInf + " current money is " + currentScoreMon);
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

    public void click()
    {
        //handles clicking
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

        int wordIndex; //right now i dont know how words will be chosen, so random
        wordIndex = Random.Range(0, 11);

        addScore(wordIndex); //adds score
        spitWord(wordIndex); //makes words fall from mouth of politician
        //print ("mouse pos " + mousePosition.x + " y " + mousePosition.y + " ");    

        /*this will be how buttons are clicked
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
        //spitwords is complicated
        //it has a bunch of arrays, and a 'pointer' so it can cycle through the array faster

        fWordsPointer++; //move the pointer up for this new word
        fWordsPointer = fWordsPointer % maxWords; //make sure pointer doesnt exceed array bounds

        if (activWords != maxWords) //only 'maxwords' amount of words can fall at once
            activWords++;
        fWordsRend[fWordsPointer].enabled = true;
        

        fWordsRend[fWordsPointer].sprite = wordSprites[wordIndex];
        fWordsMoneyPop[fWordsPointer] = fWordsMVal[wordIndex];
        

        fWordsRend[fWordsPointer].transform.position = new Vector3(-200F, 9F, 0);
        fWordsTime[fWordsPointer] = Time.time + 2;
        fWordsHForce[fWordsPointer] = Random.Range(50f, 200f);
        fWordsVForce[fWordsPointer] = Random.Range(0f, 30.0f);
        //initalize a new word to fall, add it to array.
    }

    public void wordFall()
    {
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
    print("time is " + Time.time + " killmon is " + killMon);
    if (killMon <= Time.time)
        monTxt.text = "";

}


}