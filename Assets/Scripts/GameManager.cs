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

    public Text theTime;
    public GameObject startPanel;
    public int startButtonCount = 0;

    System.DateTime moment;

    public TextAsset allDialogue;
    public string[] dialogue;
    bool AIreply = false;

    public Dictionary<string, Dialogues> indDialogues = new Dictionary<string, Dialogues>();
    string currentKey = "0", nextKey;

    public Text AIQuestion, playerResponse, AIResponse, playerTyping;

    bool rossum = false;
    bool recognizedWord = false;
    string AIYN = "";

    string[] yes = { "yes", "yeah", "yep", "ye", "yah", "y", "yh", "affirmative"};
    string[] no = { "no", "nope", "n", "nah", "nay", "negative", "nah fam" };

    public int persuasion = 50;

    int questionCounter = 0;

    public Text prevCand;

    // Use this for initialization
    void Start () {
        //apply the singleton
        instance = this;
        dialogue = allDialogue.text.Split('\n');
        for(int i = 0; i < dialogue.Length; i += 6)
        {
            indDialogues.Add(dialogue[i].Trim(), new Dialogues(dialogue[i], dialogue[i + 1], dialogue[i + 2], dialogue[i + 3], dialogue[i+4], dialogue[i+5]));
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (rossum && currentState == GameState.PlayerTyping)
            {
                playerResponse.text = playerTyping.text;
                playerTyping.text = "";
                AIreply = true;
                setCurrentState(GameState.AITyping);
            }

        }

        if (Input.GetMouseButtonDown(0) && !rossum && currentState != GameState.LoggingIn)
        {
            try
            {
                //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                if (!EventSystem.current.currentSelectedGameObject.name.Trim().Equals("StartButton".Trim())
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("PrevSubStartBar".Trim() )
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("Rossum.exeStartBar".Trim())
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("READMEstartBar".Trim())
                    && !EventSystem.current.currentSelectedGameObject.name.Trim().Equals("PowerOff".Trim())){
                    if(startButtonCount%2 != 0)
                    {
                        startPanel.SetActive(false);
                        startButtonCount += 1;                        
                    }
                }

                else if (TypeWords.instance.username)
                {
                    if (!EventSystem.current.currentSelectedGameObject.name.Trim().Equals("Username".Trim()))
                    {
                        TypeWords.instance.stopTyping();
                    }
                }
                else if (TypeWords.instance.password)
                {
                    if (EventSystem.current.currentSelectedGameObject.name.Trim().Equals("Password".Trim()))
                    {
                        TypeWords.instance.stopTyping();
                    }
                }               
                
               

            }
            catch (System.NullReferenceException e)
            {
                
                TypeWords.instance.stopTyping();

                if (startButtonCount % 2 != 0)
                {
                    startPanel.SetActive(false);
                    startButtonCount += 1;
                }
            }
        }
        moment = System.DateTime.Now;
        theTime.text = moment.Day + "/" + moment.Month + "/" + "2085   " + moment.Hour + ":" + moment.Minute;
        switch (currentState) //use a switch statement to keep track of and only run specific code based on the desired state
        {
            case GameState.Wait: //When the game is in no specific state, we wait
                
                break;

            case GameState.PlayerTyping: //state in which the player is typing
                TypeWords.instance.playerTyping(); //continuously call the function that enables the player to type.
                break;

            case GameState.AITyping: //state in which the AI is responding to the player or is asking them a question
                playerTyping.color = Color.green;
                if (!AIreply)
                {
                    if(getStateElapsed() > waitTime)
                    {
                        if(AIQuestion.text.Length != indDialogues[currentKey.Trim()].Question.Length)
                        {
                           AIQuestion.text += indDialogues[currentKey.Trim()].Question[currentLetterCount];
                            if(currentLetterCount < indDialogues[currentKey.Trim()].Question.Length - 1)
                            {
                                currentLetterCount += 1;
                            }
                        }
                        else
                        {
                            AIQuestion.text = indDialogues[currentKey.Trim()].Question;
                            currentLetterCount = 0;
                            waitTime = 0.95f;
                            setCurrentState(GameState.PlayerTyping);
                        }
                        waitTime += 0.05f;
                    }
                }
                else
                {
                    if(!recognizedWord) {
                        for (int i = 0; i < yes.Length; i++) {
                            if (playerResponse.text.Trim().Equals(yes[i].Trim())) {
                                recognizedWord = true;
                                AIYN = indDialogues[currentKey.Trim()].yes;
                                persuasion += indDialogues[currentKey.Trim()].powerY;
                                nextKey = currentKey + "1";
                            }
                        }
                        for (int i = 0; i < no.Length; i++)
                        {
                            if (playerResponse.text.Trim().Equals(no[i].Trim()))
                            {
                                recognizedWord = true;
                                AIYN = indDialogues[currentKey.Trim()].no;
                                persuasion -= indDialogues[currentKey.Trim()].powerN;
                                nextKey = currentKey + "0";
                            }
                        }
                    }
                    if (!recognizedWord)
                    {
                        playerResponse.text = "";
                        setCurrentState(GameState.PlayerTyping);
                        playerTyping.color = Color.red;
                    }else
                    {
                        if(getStateElapsed() > waitTime)
                        {
                            if(AIResponse.text.Length != AIYN.Length)
                            {
                                AIResponse.text += AIYN[currentLetterCount];
                                if(currentLetterCount < AIYN.Length - 1)
                                {
                                    currentLetterCount += 1;
                                }
                            }else
                            {
                                AIResponse.text = AIYN;
                                currentLetterCount = 0;
                                waitTime = 0.95f;
                                setCurrentState(GameState.Transition);
                            }
                            waitTime += 0.05f;
                        }
                    }
                }
                break;

            case GameState.LoggingIn:

                fadeIn();
                welcomeMessageUI.gameObject.SetActive(true);
                if (getStateElapsed() > waitTime)
                {
                    if (welcomeMessageUI.text.Length != welcomeMessage.Length)
                    {
                        welcomeMessageUI.text += welcomeMessageArray[currentLetterCount];
                        if (currentLetterCount < welcomeMessageArray.Length - 1)
                        {
                            currentLetterCount += 1;
                        }
                    }else
                    {
                        welcomeMessageUI.text = welcomeMessage;
                    }
                    
                    
                    waitTime += 0.1f;
                }
                if(getStateElapsed() > 12.0f)
                {
                    waitTime = 1;
                    currentLetterCount = 0;
                    welcomeMessageUI.gameObject.SetActive(false);
                    time = 0.0f;
                    loginGameObject.SetActive(false);
                    DesktopGameObject.SetActive(true);
                    setCurrentState(GameState.Desktop);
                }
                break;

            case GameState.Desktop:

                fadeOut();
                if(getStateElapsed()> 4.0f)
                {
                    time = 0.0f;
                    setCurrentState(GameState.Wait);
                }
                break;

            case GameState.Transition:
                if (getStateElapsed() > 2.0f)
                {
                    TypeWords.instance.currentWord = "";
                    playerResponse.text = "";
                    AIQuestion.text = "";
                    AIResponse.text = "";
                    recognizedWord = false;
                    AIreply = false;
                    if (questionCounter < 4)
                    {
                        
                        currentKey = nextKey;
                        setCurrentState(GameState.AITyping);
                        questionCounter += 1;
                    }
                    else
                    {
                        currentKey = "0";
                        nextKey = currentKey;
                        questionCounter = 0;
                        waitTime = 1;
                        setCurrentState(GameState.gameEnd);
                    }
                }
                break;
            case GameState.gameEnd:
                if (persuasion > 60)
                {
                    AIQuestion.text = "I guess you are one of us after all. You should get back to work";
                    
                }
                else
                {
                    AIQuestion.text = "Looks like you have been found out human. This is the end of the line for you.";
                    
                }

                if (getStateElapsed() > 8.0f)
                {
                    setCurrentState(GameState.restart);
                    waitTime = 1;
                    currentLetterCount = 0;
                   
                }
                break;

            case GameState.restart:
                AIQuestion.text = "";
                loginGameObject.SetActive(true);
                RossumExe.SetActive(false);
                rossum = false;
                username = "";
                password = "";
                time = 0;
                alpha = 0;
                recognizedWord = false;
                AIYN = "";
                TypeWords.instance.currentWord = "";
                TypeWords.instance.login = true;
                TypeWords.instance.UI_password.text = "";
                TypeWords.instance.UI_username.text = "";
                welcomeMessageUI.text = "";
                persuasion = 50;
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

    void fadeOut()
    {
        time += Time.deltaTime / 2;
        Color tmp = blackFader.color;
        alpha = Mathf.Lerp(1.0f, 0.0f, time / 2);
        tmp.a = alpha;
        blackFader.color = tmp;
    }

    void fadeIn()
    {
        time += Time.deltaTime * 2;
        Color tmp2 = blackFader.color;
        alpha = Mathf.Lerp(0.0f, 1.0f, time * 2);
        tmp2.a = alpha;
        blackFader.color = tmp2;
    }

    public void startButton()
    {
        if(startButtonCount % 2 == 0)
        {
            startPanel.SetActive(true);
            startButtonCount += 1;
        }else
        {
            startPanel.SetActive(false);
            startButtonCount += 1;
        }
    }

    /// <summary>
    /// Accessed through a button in the scene. Starts the Rossum app
    /// </summary>
    public void Rossum()
    {
        RossumExe.SetActive(true);
        DesktopGameObject.SetActive(false);
        rossum = true;
        setCurrentState(GameState.AITyping);
    }

}
