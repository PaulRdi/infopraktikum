﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collectiblox.Model.DB
{
    [CreateAssetMenu(fileName = "CardDatabase")]
    public class CardDB : ScriptableObject
    {
        public List<ScriptableObject> cards;
    }
}