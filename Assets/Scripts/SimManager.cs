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

    int _simKernel;
    public static int SimKernel { get { return Instance._simKernel; } }

    public EnergyField _energyField;
    public static EnergyField Energy { get { return Instance._energyField; } }

    CritterData[] _critters;
    public static CritterData[] Critters { get { return Instance._critters; } }


    const int threadsX = 32;
    const int threadsY = 32;

    int _critterCount = 0;


    void Awake() 
    {
        if (_energyField == null) {
            _energyField = FindObjectOfType<EnergyField>();
        }

        _critters = new CritterData[64];
        
        _simKernel = _simShader.FindKernel("TimeStep");

        //Set compute shader globals
        simShader.SetInt("tex_Width", Energy.seedTexture.width);
        simShader.SetInt("tex_Height", Energy.seedTexture.height);
        simShader.SetFloat("Sim_MaxEnergy", Energy.maxEnergy);
        simShader.SetFloat("Sim_MaxEnergyRate", Energy.maxEnergyPerSecond);
        simShader.SetFloat("Sim_MaxHealth", Critter.MaxHealth);
    }

    void LateUpdate() 
    {
        simShader.SetFloat("Unity_DeltaTime", Time.deltaTime);

        //Update the energy texture each frame
        //Red = current energy
        //Green = energy added / second
        //Blue = energy removed / second
        if (Critters.Length > 0) {
            CritterData[] output = new CritterData[_critters.Length];

            simShader.SetTexture(SimKernel, "Energy", Energy.currentEnergy);
            ComputeBuffer critterBuffer = new ComputeBuffer(Critters.Length, 24);
            critterBuffer.SetData(Instance._critters);
            simShader.SetBuffer(SimKernel, "CritterData", critterBuffer);
            simShader.Dispatch(SimKernel, Critters.Length / (threadsX * threadsY) + 1, 1, 1);
            critterBuffer.GetData(output);
            Instance._critters = output;
            critterBuffer.Release();
        }
    }

    public static void RegisterCritter(Critter c) 
    {
        c.index = Instance._critterCount;
        if (Instance._critterCount >= Instance._critters.Length) 
        {
            Array.Resize(ref Instance._critters, Instance._critters.Length + 1024);
        }
        Instance._critters[Instance._critterCount] = c.data;
        Instance._critterCount++;
    }
}