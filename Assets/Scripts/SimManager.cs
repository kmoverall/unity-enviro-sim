/// Note: that the more namespaces we use the more loading this screen has to do
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SimManager : Singleton<SimManager> {
	protected SimManager() { }

    public ComputeShader _simShader;
    public static ComputeShader simShader { get { return Instance._simShader; } }

    public EnergyField _energyField;
    public static EnergyField Energy { get { return Instance._energyField; } }

    int _simKernel;
    public static int SimKernel { get { return Instance._simKernel; } }

    int _genKernel;
    public static int GenKernel { get { return Instance._genKernel; } }

    List<CritterData> _critters;

    void Start() 
    {
        if (_energyField == null) {
            _energyField = FindObjectOfType<EnergyField>();
        }

        _genKernel = _simShader.FindKernel("Generate");
        _simKernel = _simShader.FindKernel("TimeStep");
    }

    void Update()
    {

    }

    public static void RegisterCritter(Critter c) {
        c.index = Instance._critters.Count;
        Instance._critters.Add(c.data);
    }
}