using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class BaseScript : MonoBehaviour {

    //Many thanks to http://www.indiana.edu/~gamedev/2015/09/27/creating-a-visual-novel-in-unity/ for their VN tutorial for Unity

    struct EachLine
    {
        public string content;
        public string location;
        public int newpage;
    }

    List<EachLine> alllines;
    
    int curline, curDay;
    bool vnmode;

    [SerializeField] Canvas messageCanvas;
    [SerializeField] Text messageText;
    [SerializeField] Text subText;
    [SerializeField] Text HeadText;

    // Use this for initialization
    void Start () {
        //We start in vn mode
        vnmode = true;

        //Get our dialogue
        alllines = new List<EachLine>();

        for (int n = 1; n <= 7; n++)
        {
            //Text preparing
            string file = "Assets/Text/Day" + n + ".txt";
            LoadDialogue(file);
        }

        curline = 0;
        curDay = 1;

        //Game preparing


    }
    
    // Update is called once per frame
    void Update () {
        //If we're in vn mode and the player presses down
        if (Input.GetMouseButtonDown(0) && vnmode)
        {
            ShowDialogue();
            curline++;
        }
    }

    public void ShowDialogue()
    {
        //If the "new page" is 1, then we need to clear the information that's alerady been in there
        if (alllines[curline].newpage == 1)
        {
            messageText.text="";
            subText.text = "";
            HeadText.text = "";

            //If we're at END, time to break out of vn mode
            if (alllines[curline].content.Equals("END"))
            {
                vnmode = false;
                //Show report for Today and get us started on the game
                ShowReport();
            }

        }
        else //Otherwise, just add the value to wherever it's supposed to go
        {
            //For "h"
            if (alllines[curline].location.Equals("H"))
            {
                HeadText.text = alllines[curline].content;
            }

            //for "S"
            if (alllines[curline].location.Equals("S"))
            {
                subText.text = alllines[curline].content;
            }

            //for "T"
            if (alllines[curline].location.Equals("T"))
            {
                string tempholder = messageText.text;
               messageText.text = tempholder+ "\n\n" + alllines[curline].content;
            }
        }
    }

    void ShowReport()
    {
        //Use messageText.text to show the current state of the prison

    }

    void LoadDialogue(string filename)
    {
        string line;
        StreamReader r = new StreamReader(filename);

        using (r)
        {
            do
            {
                line = r.ReadLine();
                if (line != null)
                {
                    string[] lineData = line.Split(';');
                    EachLine lineEntry = new EachLine();
                    lineEntry.content = lineData[0];
                    lineEntry.location = lineData[1];
                    if (lineData[2] == "0") lineEntry.newpage = 0;
                    else lineEntry.newpage = 1;
                    alllines.Add(lineEntry);
                }
            } while (line != null);
            r.Close();
        }
    }





}
