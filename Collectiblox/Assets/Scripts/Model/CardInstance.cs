using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox
{
    public class CardInstance<T> : ICardInstance
    {
        public T data
        {
            get;
            private set;
        }
        public ICardData baseData
        {
            get;
            private set;
        }

        public CardInstance(ICardData cardData)
        {
            baseData = Cards.DB[cardData.cardName];
            data = (T)Cards.DB[cardData.cardName];
        }
    }

    public interface ICardInstance
    {
        ICardData baseData { get; }
    }
}