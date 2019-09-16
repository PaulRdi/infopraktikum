using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Collectiblox.Model
{

    [CreateAssetMenu(fileName ="Decklist.asset")]
    public class Decklist : ScriptableObject
    {
        public List<DecklistEntry> cards;
    }

    [Serializable]
    public class DecklistEntry
    {
        public string cardName;
        public int amount;
    }
}
