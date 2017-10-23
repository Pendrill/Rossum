using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {
    //create a singleton of the GameManager, in case other scripts need to know stuff like what state we are in.
    public static GameManager instance = new GameManager();
    //The player will enter a name and a password at the beginning of the game. Mostly a game feel thing
    public string username, password, playerResponse, AIResponse;

    public enum GameState { Wait, PlayerTyping, AITyping, LoggingIn, Desktop, InApp}; //list of the different states the game can be in
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

    // Use this for initialization
    void Start () {
        //apply the singleton
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
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
                    waitTime = 0;
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
    }

}
