/// Note: that the more namespaces we use the more loading this screen has to do
using UnityEngine;

[System.Serializable]
public struct CritterData {
    public float health;
    public float consumption;
    public float timeDrain;
    public float isAlive;
}

public class SimManager : Singleton<SimManager> {
	protected SimManager() { }

    public Texture2D critterSprite;
    public float critterSize;
    public Color healthyCritterColor;
    public Color unhealthyCritterColor;
    public float maxCritterHealth;

    public Shader critterShader;
    Material critterMat;

    public ComputeShader critterManager;
    int appendKernel;

    public ComputeShader aiManager;
    int processKernel;

    public EnergyField _energyField;
    public static EnergyField Energy { get { return Instance._energyField; } }
    
    private ComputeBuffer critterDrawArgs;
    private ComputeBuffer critterPoints;
    private ComputeBuffer critterData;

    const int THREADS = 1024;
    const int THREADS_X = 32;
    const int THREADS_Y = 32;
    const int MAX_CRITTERS = 10000;

    void Awake() 
    {
        if (_energyField == null) {
            _energyField = FindObjectOfType<EnergyField>();
        }

        appendKernel = critterManager.FindKernel("AppendCritter");
        processKernel = aiManager.FindKernel("ProcessCritters");
        Shader.SetGlobalVector("Sim_EnergyCaps", new Vector4(Energy.maxEnergy, maxCritterHealth,0,0));
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
            critterPoints = new ComputeBuffer(MAX_CRITTERS, 8, ComputeBufferType.Counter);
            critterPoints.ClearAppendBuffer();
            critterPoints.SetCounterValue(0);
            Shader.SetGlobalBuffer("_CritterPoints", critterPoints);
        }
        if (critterData == null) {
            critterData = new ComputeBuffer(MAX_CRITTERS, 16, ComputeBufferType.Counter);
            critterData.ClearAppendBuffer();
            critterData.SetCounterValue(0);
            Shader.SetGlobalBuffer("_CritterData", critterData);
        }
        if (critterMat == null) {
            critterMat = new Material(critterShader);
            critterMat.hideFlags = HideFlags.HideAndDontSave;
        }

    }

    void Update()
    {
        if (!aiManager)
            return;
        CreateResources();

        Shader.SetGlobalVector("Sim_EnergyCaps", new Vector4(Energy.maxEnergy, maxCritterHealth, 0, 0));

        aiManager.SetInts("Sim_EnergyFieldSize", Energy.seedTexture.width, Energy.seedTexture.height);
        aiManager.SetFloats("Sim_EnergyCaps", Energy.maxEnergy, maxCritterHealth);
        aiManager.SetFloat("Sim_MaxCritterHealth", maxCritterHealth);
        aiManager.SetFloat("Unity_DeltaTime", Time.deltaTime);

        aiManager.SetTexture(processKernel, "EnergyField", Energy.currentEnergy);
        aiManager.SetBuffer(processKernel, "Positions", critterPoints);
        aiManager.SetBuffer(processKernel, "Data", critterData);
        aiManager.SetBuffer(processKernel, "DrawArgs", critterDrawArgs);
        
        aiManager.Dispatch(processKernel, THREADS, 1, 1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst) 
    {
        if (!critterShader)
            return;
        CreateResources();

        critterMat.SetPass(0);
        Shader.SetGlobalBuffer("_CritterData", critterData);
        critterMat.SetTexture("_Sprite", critterSprite);
        critterMat.SetFloat("_Size", critterSize);
        critterMat.SetColor("_HealthyColor", healthyCritterColor);
        critterMat.SetColor("_UnhealthyColor", unhealthyCritterColor);

        Graphics.DrawProceduralIndirect(MeshTopology.Points, critterDrawArgs, 0);
    }


    private void ReleaseResources() 
    {
        if (critterDrawArgs != null) critterDrawArgs.Release(); critterDrawArgs = null;
        if (critterPoints != null) critterPoints.Release(); critterPoints = null;
        if (critterData != null) critterData.Release(); critterData = null;
        Object.DestroyImmediate(critterMat);
    }

    void OnDisable() {
        ReleaseResources();
    }

    public void AddCritter(Vector2 position)
    {
        if (!critterManager)
            return;
        CreateResources();
        critterManager.SetFloats("NewPosition", position.x, position.y);
        critterManager.SetBuffer(appendKernel, "Positions", critterPoints);
        critterManager.SetBuffer(appendKernel, "Data", critterData);
        critterManager.Dispatch(appendKernel, 1, 1, 1);
        
        ComputeBuffer.CopyCount(critterPoints, critterDrawArgs, 0);
    }

    public void AddRandomCritter() 
    {
        AddCritter(new Vector2(
                        Random.Range(-Energy.seedTexture.width / 2.0f, Energy.seedTexture.width / 2.0f), 
                        Random.Range(-Energy.seedTexture.height / 2.0f, Energy.seedTexture.height / 2.0f)
                  ));
    }
}