using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataTest
{
    public float r, g, b;
    public DataTest(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    public override string ToString()
    {
        return r + "|" + g + "|" + b;
    }
}

public class ComputeTester : MonoBehaviour
{
    List<DataTest> dataTest;
    [SerializeField] int count = 10000;
    [SerializeField] ComputeShader shader;    

    // Start is called before the first frame update
    void Start()
    {
        Populate(ref dataTest);
        ComputeBuffer buffer = new ComputeBuffer(dataTest.Count, sizeof(float) * 3);
        buffer.SetData<DataTest>(dataTest);
        int kernelIndex = shader.FindKernel("CSMain");
        shader.SetBuffer(kernelIndex, "dataTest", buffer);
        Debug.Log(dataTest[0].ToString());
        shader.Dispatch(kernelIndex, count/10, 1, 1);
        Debug.Log(dataTest[0].ToString());

    }

    private void Populate(ref List<DataTest> dataTest)
    {
        //could obviously also do this via compute shader but I want to have a familiar environment to get started.
        dataTest = new List<DataTest>();
        for (int i = 0; i < count; i++)
        {
            dataTest.Add(new DataTest(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

