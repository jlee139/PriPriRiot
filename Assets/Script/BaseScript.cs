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

    struct Prisoner
    {
        public string name;
        public int difficulty;
        public bool converted;
        public string crime;
    }

    List<EachLine> alllines;
    List<Prisoner> allprisoners;
    List<string> Allnames;
    List<string> Allcrimes;

    int curline, curDay;
    bool vnmode;

    [SerializeField] Canvas messageCanvas;
    [SerializeField] Text messageText;
    [SerializeField] Text subText;
    [SerializeField] Text HeadText;

    // Use this for initialization
    void Start() {
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
        allprisoners = new List<Prisoner>();
        Allnames = new List<string>();
        Allcrimes = new List<string>();

        PopulateName();
        PopulateCrime();
        CreatePrisoners();

    }

    // Update is called once per frame
    void Update() {
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
            messageText.text = "";
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
                messageText.text = tempholder + "\n\n" + alllines[curline].content;
            }
        }
    }

    void ShowReport()
    {
        //Use messageText.text to show the current state of the prison


    }

    //Load Data from file just once
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

    //Create 15 prisoners to interact with
    void CreatePrisoners()
    {
        for(int i=0; i < 15; i++)
        {
            Prisoner temp = new Prisoner();

            //Set a random name
            int random = Random.Range(0, Allnames.Count);
            temp.name = Allnames[random];
            //delete name from Allnames so we don't get a repeat
            Allnames.RemoveAt(random);

            //Set a random difficulty for this guy
            temp.difficulty = Random.Range(0,5);

            //Set a random crime
            random = Random.Range(0, Allcrimes.Count);
            temp.crime = Allcrimes[random];

            //Set converted to false unless difficulty is 0
            if (temp.difficulty == 0) temp.converted = true;
            else temp.converted = false;

            Debug.Log("Name: "+temp.name+"\tCrime: "+temp.crime+"\tDifficulty: "+temp.difficulty);

            allprisoners.Add(temp);
        }
     }

    //Read from the file "Name.text" 
    void PopulateName()
    {
        string line;
        StreamReader r = new StreamReader("Assets/Text/Name.txt");

        using (r)
        {
            do
            {
                line = r.ReadLine();
                if (line != null)
                {
                    Allnames.Add(line);
                }
            } while (line != null);
            r.Close();
        }

    }

    //Read from file "Crime.text"
    void PopulateCrime()
    {
         string line;
        StreamReader r = new StreamReader("Assets/Text/Crime.txt");

        using (r)
        {
            do
            {
                line = r.ReadLine();
                if (line != null)
                {
                    //string[] lineData = line.Split(';');
                    //Allcrimes.Add(lineData[0]);
                    Allcrimes.Add(line);
                }
            } while (line != null);
            r.Close();
        }

    }

}
