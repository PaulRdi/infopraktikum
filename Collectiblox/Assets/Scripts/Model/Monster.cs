using UnityEngine;
using System;

namespace Collectiblox.Model {

    [Serializable]
    [CreateAssetMenu(fileName = "Monster.asset")]
    public class Monster : CardData
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