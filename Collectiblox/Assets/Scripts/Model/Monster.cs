using UnityEngine;
using System;

namespace Collectiblox {

    [Serializable]
    [CreateAssetMenu(fileName = "Monster.asset")]
    public class Monster : CardData<Monster>
    {
			public int combatValue{
			get { return _combatValue;}
		}
		[SerializeField]
		private int _combatValue;
		public int otherStuff{
			get { return _otherStuff;}
		}
		[SerializeField]
		private int _otherStuff;

    }
}