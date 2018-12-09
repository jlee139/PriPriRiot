using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BaseScript : MonoBehaviour {

    DialogueParser Parser;
    int curline, curDay;
    string curExpoMes, curHeader, curSubT;

    [SerializeField] Canvas messageCanvas;
    [SerializeField] Text messageText;
    [SerializeField] Text subText;
    [SerializeField] Text HeadText;

    // Use this for initialization
    void Start () {
        //Get our dialogue
        Parser = GameObject.Find("DialogueParser").GetComponent<DialogueParser>();
        curline = 0;
        curDay = 1;

        //Game preparing
    }
    
    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0) )
        {
            ShowDialogue();
            curline++;
        }
    }

    public void ShowDialogue()
    {
       
    }
    


}
