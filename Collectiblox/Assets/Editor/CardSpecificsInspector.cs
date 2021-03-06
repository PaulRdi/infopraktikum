﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Newtonsoft.Json;
using Collectiblox.Model;
namespace Collectiblox.EditorExtensions
{
    [CustomEditor(typeof(ICardData))]
    public class CardDataInspector : Editor
    {
        static AttributeFormats attributeFormats;

        ICardData data;

        public void OnEnable()
        {
            if (attributeFormats == null)
                attributeFormats = AttributeFormats.Load();
            data = (ICardData)target;
            foreach (AttributeFormat format in attributeFormats)
            {
                string className = char.ToUpper(format.type[0]) + format.type.Substring(1);
                //displayed name bei m ist Collectiblox.Monster
                //warum ist t null?
                //Funktioniert in Unity wegen Serialisierungs-Magie nicht...
                //Type t = Type.GetType("Collectiblox.Monster");
                //Type m = typeof(Monster);

                //https://answers.unity.com/questions/206665/typegettypestring-does-not-work-in-unity.html
                if (!System.AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.GetType("Collectiblox."+className) != null))
                {
                    ClassFileCreator.CreateClassFile(format, null);
                }
            }

        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label(data.type.ToString());

        }
    }
}