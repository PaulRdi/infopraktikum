using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Collectiblox.Model
{
    public class CardData : ScriptableObject, ICardData
    {
        public int cost => _cost;
        [SerializeField] int _cost;

        public GameObject prefab => _prefab;
        [SerializeField] GameObject _prefab;

        public Type type => GetType();

        public string cardName => name;

        public override string ToString()
        {
            return "-- " + cardName + " --\n" +
                   "Cost: " + cost.ToString() + "\n" +
                   "Prefab: " + prefab.ToString() + "\n" +
                   "Type: " + type.ToString() + "\n";
        }
    }
    public interface ICardData
    {
        int cost { get; }
        string cardName { get; }
        Type type { get; }
        GameObject prefab { get; }
    }
}

