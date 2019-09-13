using UnityEngine;
using System;

namespace Collectiblox {

    [Serializable]
    [CreateAssetMenu(fileName = "Trap.asset")]
    public class Trap : CardData<Trap>
    {
		public String spellEffect{
			get { return _spellEffect;}
		}
		[SerializeField]
		private String _spellEffect;

    }
}