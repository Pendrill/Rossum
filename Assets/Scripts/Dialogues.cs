using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogues  {

    private string p_key;
    private string p_Question;
    private string p_yes;
    private string p_no;
    private int p_powerY=0, p_powerN=0;

    public Dialogues(string key, string question, string yes, string powerY, string no, string powerN)
    {
        p_key = key;
        p_Question = question;
        p_yes = yes;
        p_no = no;
        p_powerY = System.Int32.Parse(powerY);
        p_powerN = System.Int32.Parse(powerN);
    }
    
    public string key
    {
        get
        {
            return p_key;
        }
    }
    public string Question
    {
        get
        {
            return p_Question;
        }
    }
    public string yes
    {
        get
        {
            return p_yes;
        }
    }
    public string no
    {
        get
        {
            return p_no;
        }
    }
    public int powerY
    {
        get
        {
            return p_powerY;
        }
    }
    public int powerN
    {
        get
        {
            return p_powerN;
        }
    }
}
