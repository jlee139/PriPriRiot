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
    List<Prisoner> convertedpri;
    List<string> Allnames;
    List<string> Allcrimes;

    int curline, curDay;
    bool vnmode, diamode;

    [SerializeField] Canvas mapCanvas;
    [SerializeField] Canvas messageCanvas;
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] Text messageText;
    [SerializeField] Text subText;
    [SerializeField] Text HeadText;
    [SerializeField] Text DiaText;
    [SerializeField] Button EndDay;

    // Use this for initialization
    void Start() {
        //We start in vn mode
        vnmode = true;
        diamode = false;
        EndDay.enabled = false;
        dialogueCanvas.enabled = false;
        mapCanvas.enabled = false;

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
        convertedpri = new List<Prisoner>();
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

        //If we're out of vn mode and dia mode, and player presses down, open us up to the map
        else if (Input.GetMouseButtonDown(0) && !vnmode && !diamode)
        {
            messageCanvas.enabled = false;
            mapCanvas.enabled = true;
        }

        //If we've hit the end of our deadline, it's time to activate the endings
        if (curDay > 7)
        {
            if (convertedpri.Count < 5)
            {
                BadEndActivate();
            }
        }

    }

    public void ShowDialogue()
    {
        //Just make sure that we're not in dialogue mode
        diamode = false;

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

    //This function only runs when a location on the map has been clicked
    public void TimeforDialogue(string loc)
    {
        diamode = true;

        //Set up the new diamode by getting rid of the map and turning the other shit back on
        mapCanvas.enabled = false;
        dialogueCanvas.enabled = true;
        EndDay.enabled = true;

        DiaText.text = loc;

    }

    public void EndDayClicked()
    {
        //Time to increment day!
        curDay++;
      //  Debug.Log("Today is "+curDay);

        messageText.text = "";

        vnmode = true;
        dialogueCanvas.enabled = false;
        mapCanvas.enabled = false;
        messageCanvas.enabled = true;
    }

    void ShowReport()
    {
        //Use messageText.text to show the current state of the prison
        string report = "Day " +curDay+ " Report:\n\n";

        //check to see if we converted anyone and print them
        if (convertedpri.Count > 0)
        {
            report += "You have convinced:\n\n";
            for (int i=0; i< convertedpri.Count; i++)
            {
                report += convertedpri[i].name+ "\n";
            }
            report += "\n";
        }
        else
        {
            report += "You have convinced no one. Get to work!";
        }
       
        //display message
        messageText.text = report;
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

            if (temp.name.Equals("Harada"))
            {
                temp.difficulty = 5;
                temp.crime = "???";
                temp.converted = false;
            }
            else
            {
                //Set a random difficulty for this guy
                temp.difficulty = Random.Range(1, 5);

                //Set a random crime
                random = Random.Range(0, Allcrimes.Count);
                temp.crime = Allcrimes[random];

                //Set converted to false unless difficulty is 0
                if (temp.difficulty == 0)
                {
                    temp.converted = true;
                    convertedpri.Add(temp);
                }
                else temp.converted = false;
            }

           // Debug.Log("Name: "+temp.name+"\tCrime: "+temp.crime+"\tDifficulty: "+temp.difficulty);

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

   void BadEndActivate()
    {
        //Clear up the previous lines stored in alllines
        alllines.Clear();
        curline = 0;
        //Then load dialogue from our BadEnd text
        LoadDialogue("Assets/Text/EndBad.txt");
        vnmode = true;

        mapCanvas.enabled = false;
        dialogueCanvas.enabled = false;
        messageCanvas.enabled = true;

    }



}
