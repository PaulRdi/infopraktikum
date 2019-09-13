using UnityEngine;
using System;

namespace Collectiblox {

    [Serializable]
    [CreateAssetMenu(fileName = "Spell.asset")]
    public class Spell : CardData<Spell>
    {
		public String spellEffect{
			get { return _spellEffect;}
		}
		[SerializeField]
		private String _spellEffect;

    }
}