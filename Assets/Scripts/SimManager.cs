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

    void Start() 
    {
        if (_energyField == null) {
            _energyField = FindObjectOfType<EnergyField>();
        }

        _simKernel = _simShader.FindKernel("TimeStep");
    }
}