using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox
{
    public class CardInstance
    {
        CardData data;

        public CardInstance(string cardName)
        {
            data = Cards.DB[cardName];
        }
    }
}