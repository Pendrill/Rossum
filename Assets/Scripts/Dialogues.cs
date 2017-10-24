using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogues  {

    private string p_key; //reference to the key the instance of this object is associated with
    private string p_Question; //string that contains the question the ai is asking
    private string p_yes; //string that contains the ai response if the player agrees
    private string p_no; //string that contains the ai response if the player disagrees
    private int p_powerY=0, p_powerN=0; //persuasion values associated to the players choice to answer yes or no

    /// <summary>
    /// create the instance of dialogue with the specific text and values associated to it
    /// </summary>
    /// <param name="key"></param>
    /// <param name="question"></param>
    /// <param name="yes"></param>
    /// <param name="powerY"></param>
    /// <param name="no"></param>
    /// <param name="powerN"></param>
    public Dialogues(string key, string question, string yes, string powerY, string no, string powerN)
    {
        p_key = key; //assign the key
        p_Question = question; //assign the Ai question
        p_yes = yes; //assign the Ai response to yes
        p_no = no; //assign the Ai response to no
        p_powerY = System.Int32.Parse(powerY); //set the persuasion value associated to yes
        p_powerN = System.Int32.Parse(powerN); //set the persuasion value associated to no
    }
    
    /// <summary>
    /// getter for the key associated to this instance of dialogue
    /// </summary>
    public string key
    {
        get
        {
            return p_key;
        }
    }
    /// <summary>
    /// getter for the specific question asked by the ai
    /// </summary>
    public string Question
    {
        get
        {
            return p_Question;
        }
    }
    /// <summary>
    /// getter for the yes response of the ai
    /// </summary>
    public string yes
    {
        get
        {
            return p_yes;
        }
    }
    /// <summary>
    /// getter for the no response of the ai
    /// </summary>
    public string no
    {
        get
        {
            return p_no;
        }
    }
    /// <summary>
    /// getter for the yes persuasion value
    /// </summary>
    public int powerY
    {
        get
        {
            return p_powerY;
        }
    }
    /// <summary>
    /// getter for the no persuasion value
    /// </summary>
    public int powerN
    {
        get
        {
            return p_powerN;
        }
    }
}
