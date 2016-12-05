using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyField : MonoBehaviour {

    public float maxEnergy = 50;
    public float pixelsPerUnit = 100;
    public float maxEnergyPerSecond = 1;

    public Texture2D seedTexture;
    [HideInInspector]
    public Texture2D currentEnergy;

    void Start () {
        if (seedTexture == null)
            return;

        currentEnergy = new Texture2D(seedTexture.width, seedTexture.height, TextureFormat.RGBAFloat, false, true);
        currentEnergy.wrapMode = TextureWrapMode.Clamp;

        //Start current energy at nothing
        Color[] colors = currentEnergy.GetPixels();
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = new Color(0,0,0,0);
        }

        //Set size of quad being rendered to
        transform.localScale = new Vector3(seedTexture.width / pixelsPerUnit, seedTexture.height / pixelsPerUnit, 1);

        Shader.SetGlobalFloat("_MaxEnergy", maxEnergy);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (currentEnergy == null)
            return;

        RenderTexture tempRT = RenderTexture.GetTemporary(currentEnergy.width, currentEnergy.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear, 1);
        tempRT.enableRandomWrite = true;

        SimManager.simShader.SetFloat("Unity_DeltaTime", Time.deltaTime * maxEnergyPerSecond);
        SimManager.simShader.SetFloat("Sim_MaxEnergy", maxEnergy);
        SimManager.simShader.SetTexture(SimManager.SimKernel, "EnergyInput", seedTexture);
        SimManager.simShader.SetTexture(SimManager.SimKernel, "Energy", currentEnergy);
        SimManager.simShader.SetTexture(SimManager.SimKernel, "Output", tempRT);

        SimManager.simShader.Dispatch(SimManager.SimKernel, tempRT.width / 32, tempRT.height / 32, 1);

        RenderTexture.active = tempRT;
        currentEnergy.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        currentEnergy.Apply();

        GetComponent<MeshRenderer>().material.mainTexture = currentEnergy;
        RenderTexture.ReleaseTemporary(tempRT);
    }
}
