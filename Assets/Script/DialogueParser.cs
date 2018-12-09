using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{

    struct EachLine
    {
        public string content;
        public string location;
        public int newpage;
    }

    List<EachLine> alllines;


    // Use this for initialization
    void Start()
    {
        alllines = new List<EachLine>();

        for (int n=1;n<=7;n++) {
            //Text preparing
            string file = "Assets/Text/Day" + n + ".txt";
            LoadDialogue(file);
        }
        
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

    public string GetLocation(int linenumber)
    {
        if (linenumber < alllines.Count)
        {
            return alllines[linenumber].location;
        }
        return "";
    }

    public int GetNewPage(int linenumber)
    {
        if (linenumber < alllines.Count)
        {
            return alllines[linenumber].newpage;
        }
        return 0;
    }

    public string GetContent(int linenumber)
    {
        if (linenumber < alllines.Count)
        {
            return alllines[linenumber].content;
        }
        return "";
    }


   
}
