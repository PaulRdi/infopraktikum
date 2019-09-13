using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox.DB
{
    public class DBCreator : MonoBehaviour
    {
        public CardDB DB;
        private static bool __DBLOADED;
        public  void Awake()
        {
            if (!__DBLOADED)
                Cards.LoadDB(DB);

            Destroy(this.gameObject);
        }
    }
}