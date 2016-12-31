using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AdjustTimeScale : MonoBehaviour {

    Text timeText;
    Slider timeSlider;

	void Start () {
        timeText = GetComponentInChildren<Text>();
        timeSlider = GetComponent<Slider>();
    }
	
	void Update () {
        Time.timeScale = Mathf.Pow(2, timeSlider.value);
        if (Time.timeScale >= 1) 
        {
            timeText.text = Time.timeScale + "x";
        }
        else
        {
            timeText.text = "1/" + 1 / Time.timeScale + "x";
        }
	}
}
