using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyNotifier : MonoBehaviour
{
    public event Action<GameObject> Destroyed;

    private void OnDestroy()
    {
        Destroyed?.Invoke(gameObject);
    }
}
