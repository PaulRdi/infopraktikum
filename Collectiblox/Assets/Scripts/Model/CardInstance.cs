﻿using System.Collections;
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
        public bool TryGetData<J>(out J data) where J : CardData
        {
            data = null;
            if (this.data is J)
            {
                data = this.data as J;
                return true;
            }
            return false;
        }
        public CardInstance<J> Get<J>() where J : CardData
        {
            if (this is CardInstance<J>)
                return this as CardInstance<J>;
            return null;
        }
        public bool TryGetStrongType<J>(out CardInstance<J> data) where J : CardData
        {
            data = null;
            if (this is CardInstance<J>)
            {
                data = this as CardInstance<J>;
                return true;
            }
            return false;
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

        public override string ToString()
        {
            //todo: implement to string once actual instance data exists.
            return data.ToString();
        }
    }

    public interface ICardInstance
    {
        ICardData baseData { get; }
        J GetData<J>() where J : CardData;
        bool TryGetData<J>(out J data) where J : CardData;
        bool TryGetStrongType<J>(out CardInstance<J> data) where J : CardData;
        CardInstance<J> Get<J>() where J : CardData;
        int cost { get; }
    }
}