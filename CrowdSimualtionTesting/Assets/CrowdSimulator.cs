using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.UI;

//https://hal.inria.fr/inria-00539572/document
/*
 * Bearing Angle -> wenn alpha über die Zeit konstant bleibt, wird eine Kollision vorher gesehen!
 * tti -> time to interaction: wie lange braucht es noch bis ich mit dem anderen Objekt interagiere?
 * 
 * "For each perceived point pi,
we compute the bearing angle αi, its time-derivative α˙ i, and the
remaining time-to-interaction relatively to the walker ttii. We deduce the risk of a future collision from α˙ i. We also deduce the
dangerousness of the situation from ttii."


    walker -> position & orientation

    "Step 1 Set camera position and orientation at the one of the considered walker (see details below).
Step 2 Render to texture environment obstacles using simplified
geometries. Compute values αi, α˙ i and distance to obstacle d
per vertex (Figures 4 and 6).
Step 3 Then, using a fragment shader, compute per pixel ttii, build
P+ and P− from τ1+ and τ1−.
Step 4 Copy the resulting texture to the CUDA space and make a
parallel reduction to compute φ+, φ−. Result is stored to an
array on the GPU." 

    -> camera for each walker is inefficient (especially with many obstacles / vertices)
    -> 2d plane of movement -> texture (for static environment) + radii for dynamic walkers
 * */
[Serializable]
public struct Walker
{
    public Vector3 position;
    public Vector3 orientation;
    public float colliderRadius;
    public Matrix4x4 localToWorld;
    public float speed;
    public float angularVelocity; //velocity around the y axis.
    public int id;
}
public class CrowdSimulator : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Material[] rndMats;
    [SerializeField] RenderTexture obstaclesTexture;
    [SerializeField] RenderTexture dynamicObstaclesRenderTex;
    [SerializeField] Camera obstacleProjectionCamera;
    [SerializeField] ComputeShader crowdSimulationShader;
    [SerializeField] int resolutionScale = 2;
    [SerializeField] Shader obstaclesShader;
    [SerializeField] GameObject agentPrefab;
    [SerializeField] RawImage debugImg;
    [SerializeField] int numAgents = 1000;
    [SerializeField] bool generateAgentsOnSpawn = true;

    List<Walker> data;
    List<AgentController> agents;

    Dictionary<int, AgentController> idToController;

    void Start()
    {
        agents = new List<AgentController>();

        if (generateAgentsOnSpawn)
            CreateRandomAgents();
        else
            agents = FindObjectsOfType<AgentController>().ToList();

        idToController = new Dictionary<int, AgentController>();
        data = new List<Walker>();
        dynamicObstaclesRenderTex = new RenderTexture(obstacleProjectionCamera.pixelWidth, obstacleProjectionCamera.pixelHeight, 24);
        dynamicObstaclesRenderTex.enableRandomWrite = true;
        dynamicObstaclesRenderTex.Create();
        Matrix4x4 vp = GetVPMatrix();
        crowdSimulationShader.SetFloat("RAY_RESOLUTION", 30.0f);
        crowdSimulationShader.SetFloat("RAY_ANGLE", 30.0f);
        for (int id = 0; id < agents.Count; id++)
        {
            agents[id].Init(id);
            Walker walker = new Walker();
            walker.id = id;
            walker.position = agents[id].transform.position;
            walker.colliderRadius = 0.4f;
            idToController.Add(id, agents[id]);

            if (generateAgentsOnSpawn)
            {
                walker.orientation = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
                walker.speed = 3.0f;
            }
            else
            {
                walker.orientation = agents[id].data.orientation;
                walker.speed = agents[id].data.speed;
            }
            data.Add(walker);

        }

        InitRendering();


        //DoAgentProjectionCompute();
    }


    private void Update()
    {
        Matrix4x4 vp = GetVPMatrix();
        Matrix4x4 p = obstacleProjectionCamera.projectionMatrix;
        crowdSimulationShader.SetMatrix("MATRIX_VP", vp);
        crowdSimulationShader.SetMatrix("INVERSE_MATRIX_VP", vp.inverse);
        crowdSimulationShader.SetMatrix("INVERSE_MATRIX_P", p.inverse);
        crowdSimulationShader.SetFloat("dt", Time.deltaTime);
        crowdSimulationShader.SetFloat("resolutionScale", resolutionScale);
        crowdSimulationShader.SetVector("screenSize", new Vector2(
            obstacleProjectionCamera.scaledPixelWidth,
            obstacleProjectionCamera.scaledPixelHeight));
        crowdSimulationShader.SetVector("textureSize", new Vector2(
            obstaclesTexture.width,
            obstaclesTexture.height));
        //RenderWorldStateTex();
        //Move();
        ComputeBuffer agentsBuffer = new ComputeBuffer(data.Count, sizeof(float) * 26);
        agentsBuffer.SetData<Walker>(data);
        Walker[] resultData = new Walker[data.Count];

        AvoidStaticObstales(ref agentsBuffer);
        MoveV2(ref agentsBuffer);
        //TestMove(ref agentsBuffer);


        agentsBuffer.GetData(resultData);
        data = resultData.ToList();
        UpdateAgents();
        agentsBuffer.Dispose();
    }

    private void AvoidStaticObstales(ref ComputeBuffer agentsBuffer)
    {
        int avoidObstaclesKernelIndex = crowdSimulationShader.FindKernel("AvoidStaticObstacles");
        crowdSimulationShader.SetTexture(avoidObstaclesKernelIndex, "WorldState", obstaclesTexture);
        crowdSimulationShader.SetBuffer(avoidObstaclesKernelIndex, "Agents", agentsBuffer);
        crowdSimulationShader.Dispatch(avoidObstaclesKernelIndex, data.Count / 1024 + data.Count % 1024, 1, 1);
    }

    private void InitRendering()
    {
        obstaclesTexture = new RenderTexture(
            obstacleProjectionCamera.pixelWidth * resolutionScale,
            obstacleProjectionCamera.pixelHeight * resolutionScale, 24);
        obstacleProjectionCamera.gameObject.SetActive(false);
        obstaclesTexture.enableRandomWrite = true;
        obstacleProjectionCamera.targetTexture = obstaclesTexture;
        //debugImg.texture = obstaclesTexture;
        obstacleProjectionCamera.Render();
    }

    private void CreateRandomAgents()
    {
        for (int i = 0; i < numAgents; i++)
        {
            AgentController Agent = Instantiate(agentPrefab, new Vector3(
                UnityEngine.Random.Range(-19f, 19f),
                0f,
                UnityEngine.Random.Range(-19f, 19f)), Quaternion.identity).GetComponent<AgentController>();
            agents.Add(Agent);
            Agent.GetComponent<MeshRenderer>().material = rndMats[UnityEngine.Random.Range(0, rndMats.Length)];
        }
    }

    //Texture Based... didnt really get this to execute properly :(
    private void Move(ref ComputeBuffer agentsBuffer)
    {
        int moveKernelIndex = crowdSimulationShader.FindKernel("Move");
        crowdSimulationShader.SetTexture(moveKernelIndex, "WorldState", obstaclesTexture);        
        crowdSimulationShader.SetBuffer(moveKernelIndex, "Agents", agentsBuffer);
        crowdSimulationShader.Dispatch(moveKernelIndex, data.Count / 1024 + data.Count % 1024, 1, 1);
    }

    //~1 Ray per agent & 600 agents = ~30fps
    //~24 Rays per agent -> Still ~30fps
    //~60 Rays per agent -> ~22fps
    private void MoveV2(ref ComputeBuffer agentsBuffer)
    {
        int moveKernelIndex = crowdSimulationShader.FindKernel("MoveV2");        
        crowdSimulationShader.SetBuffer(moveKernelIndex, "Agents", agentsBuffer);
        crowdSimulationShader.Dispatch(moveKernelIndex, data.Count / 128 + data.Count % 128, 1, 1);
        
    }
    private void RenderWorldStateTex()
    {
        int clearKernelIndex = crowdSimulationShader.FindKernel("ResetTex");
        crowdSimulationShader.SetTexture(clearKernelIndex, "WorldState", obstaclesTexture);
        crowdSimulationShader.Dispatch(clearKernelIndex, obstaclesTexture.width / 32, obstaclesTexture.height / 18, 1);
        obstacleProjectionCamera.RenderWithShader(obstaclesShader, "RenderType");
    }

    private void TestMove(ref ComputeBuffer agentsBuffer)
    {
        int moveKernelID = crowdSimulationShader.FindKernel("TestMove");
        crowdSimulationShader.SetBuffer(moveKernelID, "Agents", agentsBuffer);
        crowdSimulationShader.Dispatch(moveKernelID, 1024, 1, 1);
    }



    //project the agents colliders on to a texture.
    //doesnt work, dont know why :(
    //would be more efficient just to project a sphere for the collider than actually render a model to the texture
    private void DoAgentProjectionCompute()
    {
        
        int initTextureKernelIndex = crowdSimulationShader.FindKernel("SetPositionsTex");
        ComputeBuffer buffer = new ComputeBuffer(data.Count, sizeof(float) * 26);

        buffer.SetData<Walker>(data);
        crowdSimulationShader.SetVector("screenSize", new Vector2(
            obstacleProjectionCamera.scaledPixelWidth,
            obstacleProjectionCamera.scaledPixelHeight));
        crowdSimulationShader.SetVector("textureSize", new Vector2(
            obstaclesTexture.width,
            obstaclesTexture.height));
        crowdSimulationShader.SetBuffer(initTextureKernelIndex, "Agents", buffer);
        crowdSimulationShader.SetTexture(initTextureKernelIndex, "Result", dynamicObstaclesRenderTex);
        crowdSimulationShader.Dispatch(
            initTextureKernelIndex,
            dynamicObstaclesRenderTex.width / 32,
            dynamicObstaclesRenderTex.height / 18,
            1);
        buffer.Dispose();
    }

    //https://answers.unity.com/questions/12713/how-do-i-reproduce-the-mvp-matrix.html
    private Matrix4x4 GetVPMatrix()
    {
        bool d3d = SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D") > -1;
        Matrix4x4 V = obstacleProjectionCamera.worldToCameraMatrix;
        Matrix4x4 P = obstacleProjectionCamera.projectionMatrix;
        if (d3d)
        {
            // Invert Y for rendering to a render texture
            for (int i = 0; i < 4; i++)
            {
                P[1, i] = -P[1, i];
            }
            // Scale and bias from OpenGL -> D3D depth range
            for (int i = 0; i < 4; i++)
            {
                P[2, i] = P[2, i] * 0.5f + P[3, i] * 0.5f;
            }
        }
        return P * V;
    }
    void UpdateAgents()
    {
        for (int i = 0; i < data.Count; i++)
        {
            Walker w = data[i];
            idToController[w.id].transform.position = w.position;
            w.localToWorld = idToController[w.id].transform.localToWorldMatrix;
            w.position = idToController[w.id].transform.position;
        }
    }
}
