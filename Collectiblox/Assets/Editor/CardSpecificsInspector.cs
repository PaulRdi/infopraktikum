using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
namespace Collectiblox
{
    [CustomEditor(typeof(CardData))]
    public class CardDataInspector : Editor
    {
        static AttributeFormats attributeFormats;


        CardData data;

        public void OnEnable()
        {
            if (attributeFormats == null)
                attributeFormats = AttributeFormats.Load();
            data = (CardData)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
}