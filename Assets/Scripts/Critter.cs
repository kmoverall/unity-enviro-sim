using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct CritterData {
    public Vector2 position;
    public float health;
    public float maxConsumption;
    public float timeDrain;

    public float consumptionThisFrame;
}

public class Critter : MonoBehaviour {

    [HideInInspector]
    public int index = -1;

    static float BaseHealth = 100;
    public static float MaxHealth = 200;

    public CritterData data;

	void Start () {
        SimManager.RegisterCritter(this);
	}
	
	public void Update () {
        data = SimManager.Critters[index];
        transform.position = data.position;
	}
}
