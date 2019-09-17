using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Collectiblox.Model.DB;

namespace Collectiblox.Model {
    public class Cards
    {
        Dictionary<string, ICardData> cards;

        public static Cards DB
        {
            get
            {
                if (instance == null)
                    throw new System.Exception("DB not loaded!");
                return instance;
            }
        }
        static Cards instance;
        public ICardData this[string cardName]
        {
            get
            {
                return cards[cardName];
            }
        }

        public static void LoadDB(CardDB db)
        {
            instance = new Cards();
            instance.cards = new Dictionary<string, ICardData>();
            foreach(ICardData data in db.cards)
            {
                instance.cards.Add(data.cardName, data);
            }
        }

        internal static ICardInstance CreateCardInstance(ICardData cardData, PlayerKey player1Key)
        {
            Type cardType = cardData.type;
            ICardInstance cardInstance = null;

            if (cardType == typeof(Monster))
            {
                cardInstance = new CardInstance<Monster>(cardData);
            }
            else if (cardType == typeof(Spell))
            {
                cardInstance = new CardInstance<Spell>(cardData);
            }

            if (cardInstance == null)
                throw new System.Exception("Invalid Card Type");

            return cardInstance;
        }
    }
}