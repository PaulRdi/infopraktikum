using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox.Model
{
    public class CardInstance<T> : ICardInstance where T : CardData
    {
        public T data
        {
            get;
            private set;
        }
        public J GetData<J>() where J : CardData
        {
            if (data is J)
                return data as J;
            return null;
        }
        public CardInstance<J> Get<J>() where J : CardData
        {
            if (this is CardInstance<J>)
                return this as CardInstance<J>;
            return null;
        }
        /// <summary>
        /// Modified cost for this card instance.
        /// Todo: Implement interface for modifying cost
        /// </summary>
        public int cost
        {
            get
            {
                return data.cost;
            }
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
        J GetData<J>() where J : CardData;
        CardInstance<J> Get<J>() where J : CardData;
        int cost { get; }
    }
}