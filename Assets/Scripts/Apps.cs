using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Apps { 
	
    private GameObject p_Window;
    private TextAsset p_theText;
    private string p_TextArray;
    private Text p_Text;
    private Button p_Exit;
    private bool p_isDisplayed;


    public Apps(GameObject window, TextAsset theText, GameObject canvas)
    {
        p_Window = GameObject.Instantiate(window, canvas.transform);
        p_Window.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-394, 471), Random.Range(-187, 244));
        p_theText = theText;
        p_Text = p_Window.gameObject.GetComponentInChildren<Text>();
        p_Exit = p_Window.gameObject.GetComponentInChildren<Button>();
        p_TextArray = p_theText.text;
        p_Text.text = p_TextArray;
        p_isDisplayed = true;
        p_Exit.onClick.AddListener(setWindowOff);
    }
    void setWindowOff()
    {
        p_Window.SetActive(false);
        p_isDisplayed = false;
    }

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
    public GameObject window
    {
        get
        {
            return p_Window;
        }
        
    }
}
