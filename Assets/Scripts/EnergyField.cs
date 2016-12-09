using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyField : MonoBehaviour {

    public float maxEnergy = 50;
    public float maxEnergyPerSecond = 1;

    public Texture2D seedTexture;

    RenderTexture currentEnergy;
    Texture2D energySample;
    Texture2D energyDrain;
    
    const int threadsX = 32;
    const int threadsY = 32;

    void Awake () {
        if (seedTexture == null)
            return;

        currentEnergy = new RenderTexture(seedTexture.width, seedTexture.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        currentEnergy.antiAliasing = 1;
        currentEnergy.enableRandomWrite = true;
        currentEnergy.wrapMode = TextureWrapMode.Clamp;
        currentEnergy.Create();

        //Set size of quad being rendered to
        transform.localScale = new Vector3(seedTexture.width, seedTexture.height, 1);

        //Generate the initial state of the energy field texture
        Shader.SetGlobalFloat("_MaxEnergy", maxEnergy);
        SimManager.simShader.SetInt("tex_Width", seedTexture.width);
        SimManager.simShader.SetInt("tex_Height", seedTexture.height);
        SimManager.simShader.SetFloat("Sim_MaxEnergy", maxEnergy);
        
        SimManager.simShader.SetTexture(SimManager.GenKernel, "Seed", seedTexture);
        SimManager.simShader.SetTexture(SimManager.GenKernel, "Energy", currentEnergy);
        SimManager.simShader.Dispatch(SimManager.GenKernel, (currentEnergy.width + threadsX - 1) / threadsX, (currentEnergy.height + threadsY - 1) / threadsY, 1);

        //Set the initial state of the texture used for sampling
        energySample = new Texture2D(seedTexture.width, seedTexture.height, TextureFormat.RGBAFloat, false, true);

        RenderTexture.active = currentEnergy;
        energySample.ReadPixels(new Rect(0, 0, currentEnergy.width, currentEnergy.height), 0, 0);
        energySample.Apply();
        RenderTexture.active = null;

        GetComponent<MeshRenderer>().material.mainTexture = currentEnergy;
    }
	
	void LateUpdate ()
    {
        //Update the energy texture each frame
        //Red = current energy
        //Green = energy added / second
        SimManager.simShader.SetFloat("Unity_DeltaTime", Time.deltaTime * maxEnergyPerSecond);
        
        SimManager.simShader.SetTexture(SimManager.SimKernel, "Energy", currentEnergy);

        SimManager.simShader.Dispatch(SimManager.SimKernel, (currentEnergy.width + threadsX - 1) / threadsX, (currentEnergy.height + threadsY - 1) / threadsY, 1);

        RenderTexture.active = currentEnergy;
        energySample.ReadPixels(new Rect(0, 0, currentEnergy.width, currentEnergy.height), 0, 0);
        energySample.Apply();
        RenderTexture.active = null;
    }

    void OnApplicationQuit()
    {
        currentEnergy.Release();
    }

    public float GetEnergyAtPoint(Vector3 position) 
    {
        position.x = position.x / energySample.width + 0.5f;
        position.y = position.y / energySample.height + 0.5f;
        return energySample.GetPixelBilinear(position.x, position.y).r;
    }
}
