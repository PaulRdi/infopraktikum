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
    public float speed;
    public float angularVelocity; //velocity around the y axis.
}
public class CrowdSimulator : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] RenderTexture obstaclesTexture;
    [SerializeField] RenderTexture dynamicObstaclesRenderTex;
    [SerializeField] Camera obstacleProjectionCamera;
    [SerializeField] ComputeShader crowdSimulationShader;

    AgentController[] agents;

    Dictionary<int, AgentController> idToController;
    Dictionary<int, Walker> idToData;
    List<int> ids;


    void Start()
    {
        idToController = new Dictionary<int, AgentController>();
        idToData = new Dictionary<int, Walker>();
        ids = new List<int>();
        agents = FindObjectsOfType<AgentController>();
        dynamicObstaclesRenderTex = new RenderTexture(1024, 1024, 24);
        dynamicObstaclesRenderTex.enableRandomWrite = true;
        dynamicObstaclesRenderTex.Create();

        for (int id = 0; id < agents.Length; id++)
        {
            agents[id].Init(id);
            idToController.Add(id, agents[id]);
            idToData.Add(id, new Walker());
            ids.Add(id);
        }
        UpdateAgents();

        obstaclesTexture = new RenderTexture(1024, 1024, 8);
        obstacleProjectionCamera.targetTexture = obstaclesTexture;
        obstacleProjectionCamera.Render();
        obstacleProjectionCamera.gameObject.SetActive(false);


        crowdSimulationShader.SetMatrix("INV_VP", GetInvVP());
        int initTextureKernelIndex = crowdSimulationShader.FindKernel("SetPositionsTex");
        ComputeBuffer buffer = new ComputeBuffer(ids.Count, sizeof(float)*8);
        buffer.SetData<Walker>(idToData.Values.ToList());
        crowdSimulationShader.SetBuffer(initTextureKernelIndex, "Agents", buffer);
        crowdSimulationShader.SetTexture(initTextureKernelIndex, "Result", dynamicObstaclesRenderTex);
        crowdSimulationShader.Dispatch(initTextureKernelIndex, 32, 32, 1);
        
        buffer.Dispose();

    }
    //https://answers.unity.com/questions/12713/how-do-i-reproduce-the-mvp-matrix.html
    private Matrix4x4 GetInvVP()
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
        return (P * V).inverse;
    }

    void UpdateAgents()
    {
        foreach(int id in ids)
        {
            Walker walker = new Walker();
            walker.position = idToController[id].transform.position;
            idToData[id] = walker;
        }
    }
}
