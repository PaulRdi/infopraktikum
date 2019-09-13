using UnityEngine;
using System;

namespace Collectiblox {

    [Serializable]
    [CreateAssetMenu(fileName = "Spell.asset")]
    public class Spell : CardData
    {
		public String spellEffect{
			get { return _spellEffect;}
		}
		[SerializeField]
		private String _spellEffect;

    }
}