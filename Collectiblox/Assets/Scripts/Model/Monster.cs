using UnityEngine;
using System;

namespace Collectiblox {
    public class Monster 
    {
	
		public int CombatValue {
			get { return combatValue;}
		}
		[SerializeField]
		private int combatValue;

		public int OtherStuff {
			get { return otherStuff;}
		}
		[SerializeField]
		private int otherStuff;

    }
}