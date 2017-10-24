using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Apps { 
	
    private GameObject p_Window; //reference to the window gameobject
    private TextAsset p_theText; //reference to the text file that contains the text that will be displayed
    private string p_TextArray; //reference to the string of text that will contain the contents of the textfile
    private Text p_Text; //the text object in the scene
    private Button p_Exit; //the button to close the window
    private bool p_isDisplayed; //is the window currently being displayed

    /// <summary>
    /// creates a window that will display text within it
    /// </summary>
    /// <param name="window"></param>
    /// <param name="theText"></param>
    /// <param name="canvas"></param>
    public Apps(GameObject window, TextAsset theText, GameObject canvas)
    {
        p_Window = GameObject.Instantiate(window, canvas.transform); //create the window from the prefab object, have it spawn under the canvas
        p_Window.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-394, 471), Random.Range(-187, 244)); // randomly set the location of the window in the scene
        p_theText = theText; //get a reference to the test file
        p_Text = p_Window.gameObject.GetComponentInChildren<Text>(); //Find the relevant text object in the scene
        p_Exit = p_Window.gameObject.GetComponentInChildren<Button>(); //find th relevant exit button in the scene
        p_TextArray = p_theText.text; //store the text in the string
        p_Text.text = p_TextArray; //have the string of text display on the screen
        p_isDisplayed = true; //the window is being displayed
        p_Exit.onClick.AddListener(setWindowOff); //the buton can now close itself
    }

    /// <summary>
    /// function that is called when the player wants to close a window
    /// </summary>
    void setWindowOff()
    {
        p_Window.SetActive(false); //set the window to inactive
        p_isDisplayed = false; //update the bool to reflect this
    }

    /// <summary>
    /// getter and setter for the bool that checks if the window is being displayed
    /// </summary>
    public bool isDisplayed
    {
        get
        {
            return p_isDisplayed;
        }
        set
        {
            this.p_isDisplayed = value;
        }
    }
    /// <summary>
    /// getter that returns the window object
    /// </summary>
    public GameObject window
    {
        get
        {
            return p_Window;
        }
        
    }
}
