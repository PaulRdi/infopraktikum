using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

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

public struct Walker
{
    public Vector3 position;
    public Vector3 orientation;
    public Matrix4x4 localToWorld;
    public float speed;
    public float angularVelocity; //velocity around the y axis.
    public int id;
}
public class CrowdSimulator : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] RenderTexture obstaclesTexture;
    [SerializeField] RenderTexture dynamicObstaclesRenderTex;
    [SerializeField] Camera obstacleProjectionCamera;
    [SerializeField] ComputeShader crowdSimulationShader;
    [SerializeField] int resolutionScale = 2;
    [SerializeField] Shader obstaclesShader;
    [SerializeField] GameObject agentPrefab;
    List<Walker> data;
    List<AgentController> agents;

    Dictionary<int, AgentController> idToController;

    void Start()
    {
        agents = new List<AgentController>();
        CreateRandomAgents();

        idToController = new Dictionary<int, AgentController>();
        data = new List<Walker>();
        dynamicObstaclesRenderTex = new RenderTexture(obstacleProjectionCamera.pixelWidth, obstacleProjectionCamera.pixelHeight, 24);
        dynamicObstaclesRenderTex.enableRandomWrite = true;
        dynamicObstaclesRenderTex.Create();
        Matrix4x4 vp = GetVPMatrix();
        crowdSimulationShader.SetMatrix("INVERSE_MATRIX_VP", vp.inverse);
        for (int id = 0; id < agents.Count; id++)
        {
            agents[id].Init(id);
            Walker walker = new Walker();
            walker.orientation = new Vector3(UnityEngine.Random.Range(-1f,1f), 0, UnityEngine.Random.Range(-1f, 1f));
            walker.id = id;
            walker.localToWorld = agents[id].transform.localToWorldMatrix;
            walker.position = agents[id].transform.position;
            walker.speed = 1.0f;
            idToController.Add(id, agents[id]);
            data.Add(walker);
        }

        obstaclesTexture = new RenderTexture(
            obstacleProjectionCamera.pixelWidth * resolutionScale, 
            obstacleProjectionCamera.pixelHeight * resolutionScale, 8);
        obstacleProjectionCamera.gameObject.SetActive(false);

        obstaclesTexture.enableRandomWrite = true;
        obstacleProjectionCamera.targetTexture = obstaclesTexture;


        //DoAgentProjectionCompute();
        Debug.Log(obstacleProjectionCamera.projectionMatrix.MultiplyPoint(agents[0].transform.position));
    }

    private void CreateRandomAgents()
    {
        for (int i = 0; i < 10000; i++)
        {
            agents.Add(Instantiate(agentPrefab, new Vector3(
                UnityEngine.Random.Range(-50f, 50f),
                0f,
                UnityEngine.Random.Range(-50f, 50f)), Quaternion.identity).GetComponent<AgentController>());
        }
    }

    private void Update()
    {
        Matrix4x4 vp = GetVPMatrix();
        crowdSimulationShader.SetMatrix("MATRIX_VP", vp);
        RenderWorldStateTex();
        //TestMove();
        Move();
    }

    private void Move()
    {
        int moveKernelIndex = crowdSimulationShader.FindKernel("Move");
        crowdSimulationShader.SetTexture(moveKernelIndex, "WorldState", obstaclesTexture);
        ComputeBuffer agentsBuffer = new ComputeBuffer(data.Count, sizeof(float) * 25);
        agentsBuffer.SetData<Walker>(data);
        crowdSimulationShader.SetBuffer(moveKernelIndex, "Agents", agentsBuffer);
        crowdSimulationShader.Dispatch(moveKernelIndex, data.Count / 1024, 1, 1);
        Walker[] resultData = new Walker[data.Count];
        agentsBuffer.GetData(resultData);
        data = resultData.ToList();
        UpdateAgents();
        agentsBuffer.Dispose();
    }

    private void RenderWorldStateTex()
    {
        int clearKernelIndex = crowdSimulationShader.FindKernel("ResetTex");
        crowdSimulationShader.SetTexture(clearKernelIndex, "WorldState", obstaclesTexture);
        crowdSimulationShader.Dispatch(clearKernelIndex, obstaclesTexture.width / 32, obstaclesTexture.height / 18, 1);
        obstacleProjectionCamera.RenderWithShader(obstaclesShader, "RenderType");
    }

    private void TestMove()
    {
        ComputeBuffer agentsBuffer = new ComputeBuffer(data.Count, sizeof(float) * 25);
        agentsBuffer.SetData<Walker>(data);
        int moveKernelID = crowdSimulationShader.FindKernel("TestMove");
        crowdSimulationShader.SetBuffer(moveKernelID, "Agents", agentsBuffer);
        crowdSimulationShader.Dispatch(moveKernelID, 1024, 1, 1);
        Walker[] resultData = new Walker[data.Count];
        agentsBuffer.GetData(resultData);
        data = resultData.ToList();
        UpdateAgents();
        agentsBuffer.Dispose();
    }

    //project the agents colliders on to a texture.
    //doesnt work, dont know why :(
    //would be more efficient just to project a sphere for the collider than actually render a model to the texture
    private void DoAgentProjectionCompute()
    {
        
        int initTextureKernelIndex = crowdSimulationShader.FindKernel("SetPositionsTex");
        ComputeBuffer buffer = new ComputeBuffer(data.Count, sizeof(float) * 25);

        buffer.SetData<Walker>(data);
        crowdSimulationShader.SetVector("screenSize", new Vector2(
            dynamicObstaclesRenderTex.width,
            dynamicObstaclesRenderTex.height));
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
        foreach(Walker w in data)
        {

            idToController[w.id].transform.position = w.position;
        }
    }
}
