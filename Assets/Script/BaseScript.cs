using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.EventSystems;

public class BaseScript : MonoBehaviour {

    //Many thanks to http://www.indiana.edu/~gamedev/2015/09/27/creating-a-visual-novel-in-unity/ for their VN tutorial for Unity

    struct EachLine
    {
        public string content;
        public string location;
        public int newpage;
    }
    
   public struct Prisoner
    {
        public string name;
        public int difficulty;
        public bool converted;
        public string crime;
        public List<DialogueChoices> diachoices;
    }

   public struct DialogueChoices
    {
        public string question;
        public string choiceRight;
        public string choiceWrong;
        public string responseRight;
        public string responseWrong;
    }

   public struct Crime
    {
        public string crimename;
        public DialogueChoices crimedia;
    }


    List<EachLine> alllines = new List<EachLine>();
    List<DialogueChoices> alldialogues = new List<DialogueChoices>();
    List<Prisoner> allprisoners = new List<Prisoner>();
    List<Prisoner> convertedpri = new List<Prisoner>();
    List<string> Allnames = new List<string>();
    List<Crime> Allcrimes = new List<Crime>();
    List<Text> curlocpriobj = new List<Text>();

    int curline, curDay;
    bool vnmode, diamode;

    [SerializeField] Canvas mapCanvas;
    [SerializeField] Canvas messageCanvas;
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] Text messageText;
    [SerializeField] Text subText;
    [SerializeField] Text HeadText;
    [SerializeField] Text DiaText;
    [SerializeField] Text DiaTextExpo;
    [SerializeField] Text ClickText;
    [SerializeField] Button EndDay;

    // Use this for initialization
    void Start() {
        //We start in vn mode
        vnmode = true;
        diamode = false;
        EndDay.enabled = false;
        dialogueCanvas.enabled = false;
        mapCanvas.enabled = false;

        for (int n = 1; n <= 7; n++)
        {
            //Text preparing
            string file = "Assets/Text/Day" + n + ".txt";
            LoadDialogue(file);
        }

        curline = 0;
        curDay = 1;

        //Game preparing
        PopulateName();
        PopulateCrime();
        PopulateDialogue();
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
            if (convertedpri.Count < 3)
            {
                BadEndActivate();
            }
            else
            {
                GoodEndActivate();
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

        List<Prisoner> curlocpri = new List<Prisoner>();

        //Set up the new diamode by getting rid of the map and turning the other shit back on
        mapCanvas.enabled = false;
        dialogueCanvas.enabled = true;
        EndDay.enabled = true;

        DiaText.text = loc+"\n\nWhich prisoner would you like to talk to?";

        //Randomly pick random people to show up here allprisoners
        List<Prisoner> deleted = new List<Prisoner>();
        int random = Random.Range(3, 6);

        for(int i = 0; i < random; i++)
        {
            int randperson = Random.Range(0, allprisoners.Count);
            deleted.Add(allprisoners[randperson]);
            curlocpri.Add(allprisoners[randperson]);

            Prisoner temppri = allprisoners[randperson];

            //Turn each prisoner into a clickable ClickText
            Text ourtext = Instantiate(ClickText, new Vector3(0,i,0), Quaternion.identity) as Text;
            ourtext.transform.SetParent(dialogueCanvas.transform);
            ourtext.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y/2+30+(i * 30),0);
            ourtext.text = "Name: "+temppri.name+"\t\tCrime: "+temppri.crime + "\t\tDifficulty: " + temppri.difficulty;

            EventTrigger trigger = ourtext.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { PrisonerNameClicked((PointerEventData)data, temppri, loc, curlocpri); });
            trigger.triggers.Add(entry);

            curlocpriobj.Add(ourtext);

            allprisoners.RemoveAt(randperson);
        }

        //Put them back
        for(int i = 0; i < deleted.Count; i++)
        {
            allprisoners.Add(deleted[i]);
        }
        
    }

    //When a prisoner is clicked on
    public void PrisonerNameClicked(PointerEventData data2, Prisoner ourpri, string loc, List<Prisoner> curlocpri)
    {
      
        //Get rid of the previous prisoner name buttons
        for (int i = 0; i < curlocpriobj.Count; i++)
        {
            Destroy(curlocpriobj[i].gameObject);
        }
        curlocpriobj.Clear();

        //Display name + Question
        int random = Random.Range(0, ourpri.diachoices.Count);

        DiaText.text = ourpri.name + " says: "+ ourpri.diachoices[random].question;

        int flip = Random.Range(0, 2);

        if (flip == 0)
        {
            //Create a button text for what the player can choose
            //Right Question
            Text righttext = Instantiate(ClickText, new Vector3(0, 1, 0), Quaternion.identity) as Text;
            righttext.transform.SetParent(dialogueCanvas.transform);
            righttext.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y, 0);
            righttext.text = ourpri.diachoices[random].choiceRight;
            curlocpriobj.Add(righttext);

            //Wrong Question
            Text wrongtext = Instantiate(ClickText, new Vector3(0, 1, 0), Quaternion.identity) as Text;
            wrongtext.transform.SetParent(dialogueCanvas.transform);
            wrongtext.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y - 30, 0);
            wrongtext.text = ourpri.diachoices[random].choiceWrong;
            curlocpriobj.Add(wrongtext);

            EventTrigger trigger = righttext.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { TalkToPrisoner((PointerEventData)data, ourpri, curlocpri, "right", random, loc); });
            trigger.triggers.Add(entry);

            EventTrigger trigger2 = wrongtext.GetComponent<EventTrigger>();
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerDown;
            entry2.callback.AddListener((data) => { TalkToPrisoner((PointerEventData)data, ourpri, curlocpri, "wrong", random, loc); });
            trigger2.triggers.Add(entry2);
        }
        else
        {
            //Create a button text for what the player can choose
            //Right Question
            Text righttext = Instantiate(ClickText, new Vector3(0, 1, 0), Quaternion.identity) as Text;
            righttext.transform.SetParent(dialogueCanvas.transform);
            righttext.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y, 0);
            righttext.text = ourpri.diachoices[random].choiceWrong;
            curlocpriobj.Add(righttext);

            //Wrong Question
            Text wrongtext = Instantiate(ClickText, new Vector3(0, 1, 0), Quaternion.identity) as Text;
            wrongtext.transform.SetParent(dialogueCanvas.transform);
            wrongtext.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y - 30, 0);
            wrongtext.text = ourpri.diachoices[random].choiceRight;
            curlocpriobj.Add(wrongtext);

            EventTrigger trigger = righttext.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { TalkToPrisoner((PointerEventData)data, ourpri, curlocpri, "wrong", random, loc); });
            trigger.triggers.Add(entry);

            EventTrigger trigger2 = wrongtext.GetComponent<EventTrigger>();
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerDown;
            entry2.callback.AddListener((data) => { TalkToPrisoner((PointerEventData)data, ourpri, curlocpri, "right", random, loc); });
            trigger2.triggers.Add(entry2);
        }

    }

    //When you talk to a prisoner you do this
    public void TalkToPrisoner(PointerEventData data2, Prisoner ourpri, List<Prisoner> curlocpri, string chosen, int index, string loc)
    {
        //Get rid of the previous prisoner name buttons
        for (int i = 0; i < curlocpriobj.Count; i++)
        {
            Destroy(curlocpriobj[i].gameObject);
        }
        curlocpriobj.Clear();

        //If the choice was correct
        if (chosen.Equals("right"))
        {
            DiaText.text = ourpri.name + " says: " + ourpri.diachoices[index].responseRight;

            DiaTextExpo.text = ourpri.name +" looks happy. You said the correct thing.";

            ourpri.difficulty -= 1;

            if (ourpri.difficulty < 1 && !ourpri.converted)
            {
                ourpri.converted = true;
                convertedpri.Add(ourpri);
                DiaTextExpo.text += "\n\nThis person will help you for the riot!";
            }

        }
        //Otherwise
        else
        {
            DiaText.text = ourpri.name + " says: " + ourpri.diachoices[index].responseWrong;
            DiaTextExpo.text = ourpri.name + " looks upset. You said the wrong thing.";
        }

        //Remove this prisoner from the current location list
        bool check = curlocpri.Remove(ourpri);
      //  Debug.Log("Was "+ourpri.name+" deleted? " + check);


        //Now make us a button to return to location 
        //Text returntxt = Instantiate(ClickText, new Vector3(0, 1, 0), Quaternion.identity) as Text;
        //returntxt.transform.SetParent(dialogueCanvas.transform);
        //returntxt.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y, 0);
        //returntxt.text = "Return to " + loc;

        //EventTrigger trigger = returntxt.GetComponent<EventTrigger>();
        //EventTrigger.Entry entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerDown;
        //entry.callback.AddListener((data) => { ReturnToLoc(loc, returntxt, curlocpri); });
        //trigger.triggers.Add(entry);

    }

    //When you want to return to prev location, do this
    public void ReturnToLoc(string loc, Text deleteme, List<Prisoner> curlocpri)
    {
        Destroy(deleteme.gameObject);
        
        DiaText.text = loc + "\n\nWhich prisoner would you like to talk to?";
        DiaTextExpo.text = "";

            //Go through curlocpri and list them
            for (int i = 0; i < curlocpri.Count; i++)
            {
                //Turn each prisoner into a clickable ClickText
                Text ourtext = Instantiate(ClickText, new Vector3(0, i, 0), Quaternion.identity) as Text;
                ourtext.transform.SetParent(dialogueCanvas.transform);
                ourtext.transform.position = new Vector3(dialogueCanvas.transform.position.x, dialogueCanvas.transform.position.y / 2 + 30 + (i * 30), 0);
            ourtext.text = "Name: " + curlocpri[i].name + "\t\tCrime: " + curlocpri[i].crime+"\t\tDifficulty: "+curlocpri[i].difficulty;
            curlocpriobj.Add(ourtext);

                //Debug.Log(curlocpri[i].name);

                EventTrigger trigger = ourtext.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => { PrisonerNameClicked((PointerEventData)data, curlocpri[i], loc, curlocpri); });
                trigger.triggers.Add(entry);
            }
        
    }

    public void EndDayClicked()
    {
        //Time to increment day!
        curDay++;
        //  Debug.Log("Today is "+curDay);

        //Get rid of the previous prisoner name buttons
        for (int i = 0; i < curlocpriobj.Count; i++)
        {
            Destroy(curlocpriobj[i].gameObject);
        }
        curlocpriobj.Clear();

        messageText.text = "";
        DiaTextExpo.text = "";

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
            if(convertedpri.Count > 3)
            {
                report += "\nYou can succesfully start a riot!";
            }
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

                //Set up Dialogue!!!!!
                DialogueChoices tempdia = new DialogueChoices();
                tempdia.question = "Ah. You.";
                tempdia.choiceRight = "Y-yes?";
                tempdia.choiceWrong = "What?";
                tempdia.responseRight = "Good luck.";
                tempdia.responseWrong = "Nothing.";

                temp.diachoices = new List<DialogueChoices>();
                temp.diachoices.Add(tempdia);
            }
            else
            {
                //Set a random difficulty for this guy
                temp.difficulty = Random.Range(1, 5);

                //Set a random crime
                random = Random.Range(0, Allcrimes.Count);
                temp.crime = Allcrimes[random].crimename;

                //Set converted to false unless difficulty is 0
                if (temp.difficulty == 0)
                {
                    temp.converted = true;
                    convertedpri.Add(temp);
                }
                else temp.converted = false;
                // Debug.Log("Name: "+temp.name);

                //Now set up the Dialogue for them
                temp.diachoices = new List<DialogueChoices>();
                List<DialogueChoices> deleted = new List<DialogueChoices>();
                for (int j = 0; j < 4; j++)
                {
                    int randdia = Random.Range(0, alldialogues.Count);
                    DialogueChoices holdme = alldialogues[randdia];
                    deleted.Add(holdme);                   
                    temp.diachoices.Add(holdme);
                    alldialogues.RemoveAt(randdia);

                    //Debug.Log(temp.diachoices[j].question);

                }
                //Put the removed dialogues back in
                for (int j = 0; j < 4; j++)
                {
                    alldialogues.Add(deleted[j]);
                }

                //Crime-specific dialogue
                DialogueChoices tempdia = new DialogueChoices();

              //  Debug.Log("Criminal count: "+Allcrimes.Count+"\nRandom Number: "+random);

                tempdia.question = Allcrimes[random].crimedia.question;
                tempdia.choiceRight = Allcrimes[random].crimedia.choiceRight;
                tempdia.choiceWrong = Allcrimes[random].crimedia.choiceWrong;
                tempdia.responseRight = Allcrimes[random].crimedia.responseRight;
                tempdia.responseWrong = Allcrimes[random].crimedia.responseWrong;
                temp.diachoices.Add(tempdia);

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
                    string[] lineData = line.Split(';');
                    Crime tempcrime = new Crime();
                    tempcrime.crimename = lineData[0];
                    tempcrime.crimedia = new DialogueChoices();
                    tempcrime.crimedia.question = lineData[1];
                    tempcrime.crimedia.choiceRight = lineData[2];
                    tempcrime.crimedia.choiceWrong = lineData[3];
                    tempcrime.crimedia.responseRight = lineData[4];
                    tempcrime.crimedia.responseWrong = lineData[5];

                    Allcrimes.Add(tempcrime);
                }
            } while (line != null);
            r.Close();
        }

    }

    //Read from file "Dialogue.text"
    void PopulateDialogue()
    {
        string line;
        StreamReader r = new StreamReader("Assets/Text/Dialogue.txt");

        using (r)
        {
            do
            {
                line = r.ReadLine();
                if (line != null)
                {
                    string[] lineData = line.Split(';');
                    DialogueChoices tempchoice = new DialogueChoices();
                    tempchoice.question = lineData[0];
                    tempchoice.choiceRight = lineData[1];
                    tempchoice.choiceWrong = lineData[2];
                    tempchoice.responseRight = lineData[3];
                    tempchoice.responseWrong = lineData[4];
                    

                    alldialogues.Add(tempchoice);

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

    void GoodEndActivate()
    {
        //Clear up the previous lines stored in alllines
        alllines.Clear();
        curline = 0;
        //Then load dialogue from our BadEnd text
        LoadDialogue("Assets/Text/EndNormal.txt");
        vnmode = true;

        mapCanvas.enabled = false;
        dialogueCanvas.enabled = false;
        messageCanvas.enabled = true;

    }


}
