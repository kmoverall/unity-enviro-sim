using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyField : MonoBehaviour {

    public float maxEnergy = 50;
    public float maxEnergyPerSecond = 1;

    public Texture2D seedTexture;

    [HideInInspector]
    public RenderTexture currentEnergy;

    public Shader energyShader;
    Material energyMat;

    void Awake () {
        if (seedTexture == null)
            return;

        energyMat = new Material(energyShader);
        energyMat.hideFlags = HideFlags.HideAndDontSave;
        
        currentEnergy = new RenderTexture(seedTexture.width, seedTexture.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        currentEnergy.antiAliasing = 1;
        currentEnergy.enableRandomWrite = true;
        currentEnergy.wrapMode = TextureWrapMode.Clamp;
        currentEnergy.Create();

        //Set size of quad being rendered to
        transform.localScale = new Vector3(seedTexture.width, seedTexture.height, 1);

        GetComponent<MeshRenderer>().material.mainTexture = currentEnergy;
        Shader.SetGlobalFloat("_MaxEnergy", maxEnergy);
        Shader.SetGlobalFloat("_MaxEnergyRate", maxEnergyPerSecond);
        Shader.SetGlobalTexture("_EnergySeed", seedTexture);
    }

    void Update() {
        Graphics.Blit(currentEnergy, currentEnergy, energyMat);
    }

    void OnApplicationQuit()
    {
        currentEnergy.Release();
        Object.DestroyImmediate(energyMat);
    }
}
