using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Collectiblox.Controller;
using Collectiblox.Model;

[CustomEditor(typeof(BoardEntity))]
public class BoardEntityInspector : Editor
{
    BoardEntity data;
    private void OnEnable()
    {
        data = (BoardEntity)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.TextArea(data.ToString());

    }
}
