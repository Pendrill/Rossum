using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TxtApps : MonoBehaviour
{

    bool hasApp = false;
    public GameObject windowPrefab;
    public TextAsset theText;
    public GameObject canvas;
    //public TxtApps StartBarButton;
    //public Button exit;
    //public Text textUI;
    Apps thisApp;

    public void displayWindow()
    {
        if( GameManager.instance.startButtonCount %2 != 0)
        {
            GameManager.instance.startPanel.SetActive(false);
            GameManager.instance.startButtonCount += 1;
        }

        if (!hasApp)
        {
            thisApp = new Apps(windowPrefab, theText, canvas);
            hasApp = true;
            //StartBarButton.thisApp = this.thisApp;
            //StartBarButton.hasApp = this.hasApp;
        }
        else
        {
            Debug.Log(thisApp);
            Debug.Log("test");
            if (!thisApp.isDisplayed)
            {
                Debug.Log("is this getting accessed?");
                thisApp.window.SetActive(true);
                thisApp.isDisplayed = true;
            }
        }
    }

}
