using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TxtApps : MonoBehaviour
{

    bool hasApp = false; //checks if the associated app has been created already
    public GameObject windowPrefab; // keeps a reference to the window prefab
    public TextAsset theText;//keeps a reference to the specific assoicated text file
    public GameObject canvas; //keeps reference to the parent of the window/app we will spawn
    //public TxtApps StartBarButton;
    //public Button exit;
    //public Text textUI;
    Apps thisApp; //keeps a reference to the app associated to this button


    /// <summary>
    /// when the icon is clicked it will create and display the relavant window
    /// </summary>
    public void displayWindow()
    {
        if( GameManager.instance.startButtonCount %2 != 0) //we check if the start bar is currently being displayed
        {
            GameManager.instance.startPanel.SetActive(false); //if so we need to minimize it
            GameManager.instance.startButtonCount += 1; //and increase the counter to remember that it is now not being displayed
        }

        if (!hasApp) //we check if the specific icon being pressed already has an app associated to it
        {
            thisApp = new Apps(windowPrefab, theText, canvas); //if not we need to create the app
            hasApp = true;//it now has an app
            //StartBarButton.thisApp = this.thisApp;
            //StartBarButton.hasApp = this.hasApp;
        }
        else//it already created an app
        {
            //Debug.Log(thisApp);
            //Debug.Log("test");
            if (!thisApp.isDisplayed) //if the app's window is not being displayed, we need to turn it on
            {
                //Debug.Log("is this getting accessed?");
                thisApp.window.SetActive(true); //set the window to true
                thisApp.isDisplayed = true; //the window is being displayed
            }
        }
    }

}
