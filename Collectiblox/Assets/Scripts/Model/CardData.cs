using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Collectiblox
{
    public class CardData<T> : ScriptableObject, ICardData
    {
        public int cost => _cost;
        [SerializeField] int _cost;

        public Type type => typeof(T);

        public string cardName => name;
    }
    public interface ICardData
    {
        int cost { get; }
        string cardName { get; }
        Type type { get; }
    }
}

