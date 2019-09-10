using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Collectiblox.DB;

namespace Collectiblox {
    public class Cards
    {
        Dictionary<string, CardData> cards;

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
        public CardData this[string cardName]
        {
            get
            {
                return cards[cardName];
            }
        }

        public static void LoadDB(CardDB db)
        {
            instance = new Cards();
            instance.cards = new Dictionary<string, CardData>();
            foreach(CardData data in db.cards)
            {
                instance.cards.Add(data.name, data);
            }
        }
    }
}