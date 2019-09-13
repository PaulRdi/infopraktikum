using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox
{
    public class CardInstance<T> : ICardInstance where T : ICardData
    {
        CardData<T> data;
        public ICardData baseData
        {
            get;
            private set;
        }

        public CardInstance(ICardData cardData)
        {
            baseData = Cards.DB[cardData.cardName];
        }
    }

    public interface ICardInstance
    {
        ICardData baseData { get; }
    }
}