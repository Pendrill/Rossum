using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TypeWords : MonoBehaviour
{

    public static TypeWords instance = new TypeWords(); //create a singleton of this scripts so the words that are being typed can be accessed by different scripts.
    public string currentWord = ""; //variable that stores the word (or phrase) the player is typing.
    int currentMaxLength  = 10; //keeps track of the current max amount of characters the player can type
    public string[] possibleLetters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "space" }; //all possible characters that the player can type
    public bool username, password; //bools that check wether we are currently editing the username or the password
    public bool login = true; //checks if we are in the login screen
    public Text UI_username, UI_password; //reference to the UI text object for both the username and the password
    void Start()
    {
        instance = this; //apply the singleton
    }

    
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Return) && login) //checks if the player presses enter 
        {
            if ( !GameManager.instance.username.Trim().Equals("".Trim()) && !GameManager.instance.password.Trim().Equals("".Trim())) //if something was entered for both the username and password
            {
                login = false; //the player is not longer login in
                username = false;
                password = false;
                currentWord = "";
                GameManager.instance.welcomeMessage = "Welcome " + GameManager.instance.username.Substring(0,1).ToUpper() + GameManager.instance.username.Substring(1).ToLower() + "\nPlease wait a few seconds while we log you in"; //Set up the welcome message by using the username entered by the player.
                GameManager.instance.welcomeMessageArray = GameManager.instance.welcomeMessage.ToCharArray();
                GameManager.instance.setCurrentState(GameManager.GameState.LoggingIn);
                Debug.Log("entered username and password, now go to next screen"); //we proceed to the loading screen for the desktop
            }else //if there are no inputs for either the password of the username
            {
                Debug.Log("either password or username was not entered, redo accordingly"); //don't let the player proceed to the desktop's loading screen
                GameManager.instance.setCurrentState(GameManager.GameState.Wait); //set the state to wait
                if (GameManager.instance.username.Trim().Equals("".Trim())) //checks if the username is still an empty string
                {
                    UI_username.text = "New Username"; //if it is, we have it display the "new username"
                    UI_username.fontStyle = FontStyle.Italic; //and return to italic
                    UI_username.color = Color.red; //and the color red
                }
                if (GameManager.instance.password.Trim().Equals("".Trim())) //we check the same thing for the password
                {
                    UI_password.text = "New Password"; //if it is we have it display the "new password" text
                    UI_password.fontStyle = FontStyle.Italic; // have it be in italics
                    UI_password.color = Color.red; // and be in red
                }
            }
        }
    }

    public void playerTyping()
    {
       if(currentWord.Length < currentMaxLength) //if the content of what the player is typing contains fewer characters than the current max then let the player keep typing.
        {
            for( int i = 0; i < possibleLetters.Length; i++) //go through all the letters that the player could possibly time
            {
                
                if (Input.GetKeyDown(possibleLetters[i])) //check if the player is pressing that specific key
                {
                    if (possibleLetters[i].Trim().Equals("space")) //checks if the player pressed the space button
                    {
                        currentWord += " "; //add a space to the current word
                        break;
                    }
                    else //if the key pressed is not space
                    {
                        currentWord += possibleLetters[i]; // add the character associated to the key that was pressed to the end of the word
                        break; //break out of the for loop
                    }                 
                    
                }
            }
        }
        if (currentWord.Length > 0) //if the word contains at least one character then let the player remove one character from the end of the word by playing backspace.
        {
            if (Input.GetKeyDown(KeyCode.Backspace)) //if the player presses backspace
            {
                currentWord = currentWord.Substring(0, currentWord.Length - 1); //then we update the word by removing the last character of what the player is typing.
            }
        }
        if (username) //Is the player wanting to edit the username
        {
            GameManager.instance.username = currentWord; //if so we need to update the unsername in the scene and the gamemanager
            UI_username.text = currentWord;
        }else if (password)//if the player is editing the password,
        {
            GameManager.instance.password = currentWord; // we need to update the password in the scene and in the gamemanager
            UI_password.text = currentWord;
        }else // if the player is neither editing the password nor the username, that means they are responding to the AI's questions
        {
            GameManager.instance.playerTyping.text = currentWord;
        }
    }

    /// <summary>
    /// This is accessed by pressing a button in the scene. It lets the player enter their desired username
    /// </summary>
    public void TypingUsername()
    {
        if (!GameManager.instance.username.Trim().Equals("".Trim())) //check if the player already typed in something for their username
        {
            currentWord = GameManager.instance.username; //if so, they should be editing what they already had typed
        }else
        {
            currentWord = ""; //otherwise reset the word to be empty
            UI_username.text = currentWord; //updates the username UI text object to be the current word which is an empty string (removes the new username text) 
            UI_username.fontStyle = FontStyle.Normal; //set the fonstyle to normal (rather than the original italic)
            UI_username.color = Color.black; //have the text be in black (rather than the original grey)
        }
        password = false; //we don't want to be editing the password
        username = true; //We want to be editing the username
        GameManager.instance.setCurrentState(GameManager.GameState.PlayerTyping); //set the game state to player typing
    }

    /// <summary>
    /// This is accessed by pressing a button in the scene. It lets the player enter their desired password associated to the username.
    /// </summary>
    public void TypingPassword()
    {
        if (!GameManager.instance.password.Trim().Equals("".Trim())) //we check to see if the player had already started typing a password
        {
            currentWord = GameManager.instance.password; //if yes, we need to let them edit that word
        }
        else
        {
            currentWord = ""; //Otherwise we just reset the current word
            UI_password.text = currentWord; //updates the password UI text object to be the current word which is an empty string (removes the new password text)
            UI_password.fontStyle = FontStyle.Normal; //set the fonstyle to normal (rather than the original italic)
            UI_password.color = Color.black; //have the text be in black (rather than the original grey)
        }
        username = false; //we don't want to be editing the username
        password = true; //we want to be editing the password
        GameManager.instance.setCurrentState(GameManager.GameState.PlayerTyping); //set the game state to player typing
    }

    /// <summary>
    /// This is accessed by pressing a button in the login scene. By pressing outside of the username and password buttons, the player returns to the 
    /// wait state and can no longer type.
    /// </summary>
    public void stopTyping()
    {
        GameManager.instance.setCurrentState(GameManager.GameState.Wait); //set the state to wait
        if (GameManager.instance.username.Trim().Equals("".Trim())) //checks if the username is still an empty string
        {
            UI_username.text = "New Username"; //if it is, we have it display the "new username"
            UI_username.fontStyle = FontStyle.Italic; //and return to italic
            UI_username.color = Color.grey; //and the color grey
        }
        if (GameManager.instance.password.Trim().Equals("".Trim())) //we check the same thing for the password
        {
            UI_password.text = "New Password"; //if it is we have it display the "new password" text
            UI_password.fontStyle = FontStyle.Italic; // have it be in italics
            UI_password.color = Color.grey; // and be in grey
        }
    }
}