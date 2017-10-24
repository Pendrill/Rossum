using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {
    //create a singleton of the GameManager, in case other scripts need to know stuff like what state we are in.
    public static GameManager instance = new GameManager();
    //The player will enter a name and a password at the beginning of the game. Mostly a game feel thing
    public string username, password;//, playerResponse, AIResponse;

    public enum GameState { Wait, PlayerTyping, AITyping, LoggingIn, Desktop, Transition, gameEnd, restart}; //list of the different states the game can be in
    public GameState currentState; //keeps track of the state the game is currently in

    float lastStateChange = 0.0f; //tracks time that has passed since the last state change

    public GameObject loginGameObject; //reference to the parent for whome all the chirldren are objects for the login screen
    public GameObject DesktopGameObject; //reference to the parent for whom all the children are objects for the desktop home screen
    public GameObject RossumExe; //reference to the parent for whom all the children are objects for the Rossum App

    float time = 0.0f, alpha;
    public Image blackFader;

    public string welcomeMessage; //message that appears while the player is logging in
    public Text welcomeMessageUI; // welcome message text UI object reference in scene
    public char[] welcomeMessageArray; //welcomeMessage split in array
    int currentLetterCount; //checsk what letter we are currently at
    float waitTime = 1.0f; //time needed to wait before displaying another letter

    public Text theTime; //the time being displayed on the botton right of the desktop
    public GameObject startPanel; //the start panel of the desktop
    public int startButtonCount = 0; //odd number means the panel should not be displayed, while even is the opposite

    System.DateTime moment; //get the time from the system clock

    public TextAsset allDialogue; //text file with all the info regarding all the possible dialogue and how they are connected etc 
    public string[] dialogue; //array that will store all the info of the possible dialogues
    bool AIreply = false;//check wether the Ai should be replying to the player

    public Dictionary<string, Dialogues> indDialogues = new Dictionary<string, Dialogues>(); //dictionary that where its key value points to a specific dialogue object
    string currentKey = "0", nextKey; //keeps a reference to the starting key and the next key

    public Text AIQuestion, playerResponse, AIResponse, playerTyping; //text objects referencing the different displayed dialogue

    bool rossum = false; //are we in the rossum.exe state
    bool recognizedWord = false; //has the player typed a word that the system recognizes
    string AIYN = ""; //reference to the AI's response to the player's answer

    string[] yes = { "yes", "yeah", "yep", "ye", "yah", "y", "yh", "affirmative"}; //all the ways for the player to say yes
    string[] no = { "no", "nope", "n", "nah", "nay", "negative", "nah fam" }; //all the ways for the player to say no

    public int persuasion = 50; //player's current level of persuasion

    int questionCounter = 0; //what question are we on

    //public Text prevCand;

    // Use this for initialization
    void Start () {
        //apply the singleton
        instance = this;
        dialogue = allDialogue.text.Split('\n'); //we split all the dialogue info into the array
        for(int i = 0; i < dialogue.Length; i += 6) //we go through all that parsed info
        {
            indDialogues.Add(dialogue[i].Trim(), new Dialogues(dialogue[i], dialogue[i + 1], dialogue[i + 2], dialogue[i + 3], dialogue[i+4], dialogue[i+5])); //we add to the disctionary the new dialogue object with its specific key. The key apears every 6 lines. What is in between those keys is specific info regarding the dialogue.
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Return)) //check if the player presses enter
        {
            if (rossum && currentState == GameState.PlayerTyping) //if we are in the rossum app and the player is typing, the player has just submitted their response
            {
                playerResponse.text = playerTyping.text; //update the player response text
                playerTyping.text = ""; //remove the player typing text
                AIreply = true; //the ai now needs to reply
                setCurrentState(GameState.AITyping); //we move to the ai's reponse phase
            }

        }

        if (Input.GetMouseButtonDown(0) && !rossum && currentState != GameState.LoggingIn) //we check whether the player is clicking.
        {
            try
            {
                //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                if (!EventSystem.current.currentSelectedGameObject.name.Trim().Equals("StartButton".Trim()) //if the player is not clicking on one of these specific buttons
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("PrevSubStartBar".Trim() )
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("Rossum.exeStartBar".Trim())
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("READMEstartBar".Trim())
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("PowerOff".Trim())){
                    if(startButtonCount%2 != 0) //and the start bar is being displayed
                    {
                        startPanel.SetActive(false); //minimize the start bar
                        startButtonCount += 1; //and update its counter to indicate that it is now minimized
                    }
                }

                else if (TypeWords.instance.username) //if we are editing the password
                {
                    if (!EventSystem.current.currentSelectedGameObject.name.Trim().Equals("Username".Trim())) //if the player does not click on the username button
                    {
                        TypeWords.instance.stopTyping(); //player should not be able to edit the username
                    }
                }
                else if (TypeWords.instance.password) //if the player is editing the password
                {
                    if (EventSystem.current.currentSelectedGameObject.name.Trim().Equals("Password".Trim())) //and they did not just click on the password
                    {
                        TypeWords.instance.stopTyping(); //player should not be able to edit the password
                    }
                }               
                
               

            }
            catch (System.NullReferenceException e) //if the player did not click on a button, catch the error
            {
                
                TypeWords.instance.stopTyping(); //the player stops typing 

                if (startButtonCount % 2 != 0) //checks if the start bar is open
                {
                    startPanel.SetActive(false); //close it
                    startButtonCount += 1;//update counter
                }
            }
        }
        moment = System.DateTime.Now; //get the time
        theTime.text = moment.Day + "/" + moment.Month + "/" + "2085   " + moment.Hour + ":" + moment.Minute; //diplay new time on desktop, based on system time

        switch (currentState) //use a switch statement to keep track of and only run specific code based on the desired state
        {
            case GameState.Wait: //When the game is in no specific state, we wait
                
                break;

            case GameState.PlayerTyping: //state in which the player is typing
                TypeWords.instance.playerTyping(); //continuously call the function that enables the player to type.
                break;

            case GameState.AITyping: //state in which the AI is responding to the player or is asking them a question
                playerTyping.color = Color.green; //set the color to green
                if (!AIreply)//if the ai should not be replying that means they are asking the question
                {
                    if(getStateElapsed() > waitTime) //we use this to display one letter at a time
                    {
                        if(AIQuestion.text.Length != indDialogues[currentKey.Trim()].Question.Length) //check the lentgh of the text object's string compared to the reference string to see if all letters have been displayed
                        {
                           AIQuestion.text += indDialogues[currentKey.Trim()].Question[currentLetterCount]; //add a letter one by one
                            if(currentLetterCount < indDialogues[currentKey.Trim()].Question.Length - 1)//check that we haven't displayed all letters
                            {
                                currentLetterCount += 1;//increase the index, to add the next letter
                            }
                        }
                        else //once all the letters have been displayed
                        {
                            AIQuestion.text = indDialogues[currentKey.Trim()].Question; //makes sure the correct text is being displayed
                            currentLetterCount = 0; //reset letter counter
                            waitTime = 0.95f; //reset wait time
                            setCurrentState(GameState.PlayerTyping);//the player no needs to input their response
                        }
                        waitTime += 0.05f; //increase the wait time. The lower this number, the faster the next letter is displayed
                    }
                }
                else //if the  ai is replying to the player's answer
                {
                    if(!recognizedWord) { //initial check to see if the player has already inputed a word that is recognized. If not,
                        for (int i = 0; i < yes.Length; i++) { //go through the all the variations of yes accepted
                            if (playerResponse.text.Trim().Equals(yes[i].Trim())) { //check to see if the player response is equal to any of those
                                recognizedWord = true; //if so, the system has found a word
                                AIYN = indDialogues[currentKey.Trim()].yes; //update the Ai response to be that of yes
                                persuasion += indDialogues[currentKey.Trim()].powerY; //update the player's persuasion
                                nextKey = currentKey + "1"; //point to the next dialogue by updating the key.
                            }
                        }
                        for (int i = 0; i < no.Length; i++) //do the same thing for the no variations
                        {
                            if (playerResponse.text.Trim().Equals(no[i].Trim())) //check to see if the player response is equal to any
                            {
                                recognizedWord = true; //recognized word was found
                                AIYN = indDialogues[currentKey.Trim()].no; // update the ai response to that of no
                                persuasion -= indDialogues[currentKey.Trim()].powerN; //update player persuasion
                                nextKey = currentKey + "0"; //point to the next dialogue by updating the key
                            }
                        }
                    }
                    if (!recognizedWord) //if the player inputed a word that was not recognized
                    {
                        playerResponse.text = ""; //do not update player response
                        setCurrentState(GameState.PlayerTyping); //return to the player typing state
                        playerTyping.color = Color.red; //update the color to red, to indicate the player mad a mistake
                    }else //if the system did recognize a word
                    {
                        if(getStateElapsed() > waitTime)//we use this to display one letter at a time
                        {
                            if(AIResponse.text.Length != AIYN.Length) //check the lentgh of the text object's string compared to the reference string to see if all letters have been displayed
                            {
                                AIResponse.text += AIYN[currentLetterCount]; //add a letter one by one
                                if (currentLetterCount < AIYN.Length - 1)//check that we haven't displayed all letters
                                {
                                    currentLetterCount += 1;//increase the index, to add the next letter
                                }
                            }
                            else //once all the letters have been displayed
                            {
                                AIResponse.text = AIYN; //makes sure the correct text is being displayed
                                currentLetterCount = 0; //reset letter count
                                waitTime = 0.95f; //reset wait time
                                setCurrentState(GameState.Transition); //move on to the next question
                            }
                            waitTime += 0.05f; //update wait time
                        }
                    }
                }
                break;

            case GameState.LoggingIn: //state once the player inputed valid username and password and pressed enter

                fadeIn(); //fade in
                welcomeMessageUI.gameObject.SetActive(true); //display welcome message
                if (getStateElapsed() > waitTime)//we use this to display one letter at a time
                {
                    if (welcomeMessageUI.text.Length != welcomeMessage.Length)//check the lentgh of the text object's string compared to the reference string to see if all letters have been displayed
                    {
                        welcomeMessageUI.text += welcomeMessageArray[currentLetterCount];//add a letter one by one
                        if (currentLetterCount < welcomeMessageArray.Length - 1)//check that we haven't displayed all letters
                        {
                            currentLetterCount += 1;//increase the index, to add the next letter
                        }
                    }
                    else//once all the letters have been displayed
                    {
                        welcomeMessageUI.text = welcomeMessage; //makes sure the correct text is being displayed
                    }
                    
                    
                    waitTime += 0.1f; //update wait time
                }
                if(getStateElapsed() > 12.0f) //wait 12 seconds
                {
                    waitTime = 1; //reset wait time
                    currentLetterCount = 0; //reset letter counter
                    welcomeMessageUI.gameObject.SetActive(false); //remove welcome message
                    time = 0.0f; //reset time
                    loginGameObject.SetActive(false); //remove login objects
                    DesktopGameObject.SetActive(true); //diplay desktop objects
                    setCurrentState(GameState.Desktop); //desktop state
                }
                break;

            case GameState.Desktop: //once the player has accesss to the desktop

                fadeOut(); //fade out
                if(getStateElapsed()> 4.0f) //wait 4 seconds
                {
                    time = 0.0f; //reset time
                    setCurrentState(GameState.Wait); //wait state
                }
                break;

            case GameState.Transition: //move from one dialogue to the next
                if (getStateElapsed() > 2.0f) //wait 2 seconds
                {
                    TypeWords.instance.currentWord = ""; //reset the current word
                    playerResponse.text = ""; //reset the player response
                    AIQuestion.text = ""; //reset the ai question
                    AIResponse.text = ""; //reset the ai response
                    recognizedWord = false; //reset the recognized word
                    AIreply = false; //reset the ai reply
                    if (questionCounter < 4) //checks if we are at the last question
                    {
                        
                        currentKey = nextKey; // updates the current key to the next key
                        setCurrentState(GameState.AITyping); //ai must ask their new question
                        questionCounter += 1; //increase the question counter
                    }
                    else //we asnswered the last question
                    {
                        currentKey = "0"; //reset key
                        nextKey = currentKey; //reset next key
                        questionCounter = 0; //reset question counter
                        waitTime = 1; //reset wait time
                        setCurrentState(GameState.gameEnd); //move to game end
                    }
                }
                break;
            case GameState.gameEnd: //the player has answered the set of questions
                if (persuasion > 60) //checks if persuasion is over 60, if so they win
                {
                    AIQuestion.text = "I guess you are one of us after all. You should get back to work"; //victory message is displayed
                    
                }
                else //if not they lose
                {
                    AIQuestion.text = "Looks like you have been found out human. This is the end of the line for you."; //defeat message is displayed
                    
                }

                if (getStateElapsed() > 8.0f) //wait 8 seconds
                {
                    setCurrentState(GameState.restart); //restart the game
                    waitTime = 1; //reset wait time
                    currentLetterCount = 0; //reset current letter count
                   
                }
                break;

            case GameState.restart: //the player automatically restarts the game
                AIQuestion.text = ""; //reset ai question
                loginGameObject.SetActive(true); //we are now in the login screen
                RossumExe.SetActive(false);//we are not longer in the rossum app, deactivate all objects associated to it
                rossum = false; //we are no longer in the rossum app
                username = "";//reset username
                password = ""; //reset password
                time = 0; //reset time
                alpha = 0; //reset alpha
                recognizedWord = false; //reset recognized word
                AIYN = ""; //reset ai response
                TypeWords.instance.currentWord = ""; //reset curretn word
                TypeWords.instance.login = true; //reset login 
                TypeWords.instance.UI_password.text = ""; //reset password
                TypeWords.instance.UI_username.text = ""; //reset username
                welcomeMessageUI.text = ""; //reset welcome message
                persuasion = 50; //reset persuasion
                break;

            
        }
	}

    /// <summary>
    /// sets the current state of the game manager
    /// </summary>
    /// <param name="state"></param>
    public void setCurrentState(GameState state)
    {
        //update the state and the time since the state has been changes
        currentState = state;
        lastStateChange = Time.time;
    }

    /// <summary>
    /// returns the amount of time that has passed since the last state change
    /// </summary>
    /// <returns></returns>
    float getStateElapsed()
    {
        return Time.time - lastStateChange; //return time since the last change in state
    }

    /// <summary>
    /// this function is accessed by pressing a button in the scene. It will exit the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit(); //Quit out of the game
    }

    /// <summary>
    /// used to fade out of scene
    /// </summary>
    void fadeOut()
    {
        time += Time.deltaTime / 2; //increase time
        Color tmp = blackFader.color; //temp reference to color
        alpha = Mathf.Lerp(1.0f, 0.0f, time / 2); //lerp alpha value
        tmp.a = alpha; //apply alpha value to temp color
        blackFader.color = tmp; //reaply temp to original color
    }

    /// <summary>
    /// used to fade in to a scene
    /// </summary>
    void fadeIn()
    {
        //we do the opposite of fade out
        time += Time.deltaTime * 2;
        Color tmp2 = blackFader.color;
        alpha = Mathf.Lerp(0.0f, 1.0f, time * 2);
        tmp2.a = alpha;
        blackFader.color = tmp2;
    }

    /// <summary>
    /// activated once the player presses the start bar, displays the start window
    /// </summary>
    public void startButton()
    {
        if(startButtonCount % 2 == 0) //if counter is even
        {
            startPanel.SetActive(true); //diplay panel
            startButtonCount += 1; //update counter
        }else //if counter is odd
        {
            startPanel.SetActive(false); //minimize panel
            startButtonCount += 1;// update counter
        }
    }

    /// <summary>
    /// Accessed through a button in the scene. Starts the Rossum app
    /// </summary>
    public void Rossum()
    {
        RossumExe.SetActive(true); //diplay all rossum related objects
        DesktopGameObject.SetActive(false);//remove all desktop related objects
        rossum = true; //we are no in rossum
        setCurrentState(GameState.AITyping); //ai needs to ask their question.
    }

}
