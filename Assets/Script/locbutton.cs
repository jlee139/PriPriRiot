using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class locbutton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string LocationName;
    [SerializeField] Text locmessage;

    // Use this for initialization
    void Start () {
		
	}

    public string getLocation()
    {
        return LocationName;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name + " GameObject");
        locmessage.text = LocationName;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name + " GameObject");
        locmessage.text = "Click on a location";
    }
}
