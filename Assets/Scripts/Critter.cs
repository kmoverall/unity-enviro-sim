using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Critter : MonoBehaviour {

    public float health;
    public float consumptionRate;
    static float BaseHealth = 100;

	void Start () {
        SimManager.RegisterCritter(this);
	}
	
	public void SimUpdate () {
	    
	}
}
