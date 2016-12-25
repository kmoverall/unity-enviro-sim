/// Note: that the more namespaces we use the more loading this screen has to do
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct CritterData {
    public Vector2 position;
    public float health;
    public float maxConsumption;
    public float timeDrain;
}

public class SimManager : Singleton<SimManager> {
	protected SimManager() { }

    public Texture2D critterSprite;
    public float critterSize;
    public Color critterColor;

    public Shader critterShader;
    Material critterMat;

    public ComputeShader critterManager;
    int appendKernel;

    public EnergyField _energyField;
    public static EnergyField Energy { get { return Instance._energyField; } }
    
    private ComputeBuffer critterDrawArgs;
    private ComputeBuffer critterPoints;

    Vector2[] debugArray;

    const int THREADS = 1024;
    const int THREADS_X = 32;
    const int THREADS_Y = 32;
    const int MAX_CRITTERS = 20;

    void Awake() 
    {
        if (_energyField == null) {
            _energyField = FindObjectOfType<EnergyField>();
        }

        appendKernel = critterManager.FindKernel("AppendCritter");

        debugArray = new Vector2[MAX_CRITTERS];
    }

    void CreateResources() 
    {
        if (critterDrawArgs == null) {
            critterDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
            var args = new int[4];
            args[0] = 0;
            args[1] = 1;
            args[2] = 0;
            args[3] = 0;
            critterDrawArgs.SetData(args);
        }
        if (critterPoints == null) {
            critterPoints = new ComputeBuffer(MAX_CRITTERS, sizeof(float)*2, ComputeBufferType.Append);
            critterPoints.ClearAppendBuffer();
        }
        if (critterMat == null) {
            critterMat = new Material(critterShader);
            critterMat.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst) 
    {
        if (!critterShader)
            return;
        CreateResources();

        critterMat.SetTexture("_Sprite", critterSprite);
        critterMat.SetFloat("_Size", critterSize);
        critterMat.SetColor("_Color", critterColor);
        critterMat.SetBuffer("_CritterPoints", critterPoints);

        ComputeBuffer.CopyCount(critterPoints, critterDrawArgs, 0);
        critterMat.SetPass(0);
        Graphics.DrawProceduralIndirect(MeshTopology.Points, critterDrawArgs, 0);
    }


    private void ReleaseResources() 
    {
        if (critterDrawArgs != null) critterDrawArgs.Release(); critterDrawArgs = null;
        if (critterPoints != null) critterPoints.Release(); critterPoints = null;
        Object.DestroyImmediate(critterMat);
    }

    void OnDisable() {
        ReleaseResources();
    }

    public void AddCritter(Vector2 position)
    {
        CreateResources();
        critterManager.SetFloats("NewPosition", position.x, position.y);
        critterManager.SetBuffer(appendKernel, "Positions", critterPoints);
        critterManager.Dispatch(appendKernel, 1, 1, 1);
    }

    public void AddRandomCritter() 
    {
        AddCritter(new Vector2(Random.Range(-40.0f, 40.0f), Random.Range(-40.0f, 40.0f)));
    }
}