using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DisplayCritterCount : MonoBehaviour {

    Text display;

	void Start () {
        display = GetComponent<Text>();
	}
	
	void Update () {
        display.text = "# of Critters: " + SimManager.critterCount;
	}
}
