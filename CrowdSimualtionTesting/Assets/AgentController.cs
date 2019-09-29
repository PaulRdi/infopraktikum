using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    int id;
    //private void Update()
    //{
    //    transform.Translate(new Vector3(1.0f, 0f, 0f) * Time.deltaTime);
    //}

    public void Init(int id)
    {
        this.id = id;
    }
}
