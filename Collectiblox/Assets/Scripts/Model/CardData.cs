using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox
{
    [CreateAssetMenu(fileName = "Card")]
    public class CardData : ScriptableObject
    {
        public CardType Type { get { return type; } }
        [SerializeField] CardType type;

        public int Cost { get { return cost; } }
        [SerializeField] int cost;

        //serializable custom data for each card.
        public Dictionary<string, string> Attributes;
    }
}