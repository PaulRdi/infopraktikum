using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collectiblox.Model;

namespace Collectiblox.Controller
{
    public class BoardEntity : MonoBehaviour
    {
        IFieldEntity data;

        public void Init(FieldEntity<CardInstance<Monster>> cardInstance)
        {
            data = cardInstance;
        }

        public override string ToString()
        {
            if (data != null)
                return data.ToString();
            return "No data.";
        }
    }
}