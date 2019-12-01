using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collectiblox.Model.Rules;

namespace Collectiblox
{
    [CreateAssetMenu(fileName = "GameConfig.asset")]
    public class GameConfig : ScriptableObject
    {
        public Vector2Int gridSize => _gridSize;
        [SerializeField] Vector2Int _gridSize;

        public Vector2Int crystalPosition => _crystalPosition;
        [SerializeField] Vector2Int _crystalPosition;

        public int crystalStrength => _crystalStrength;
        [SerializeField] int _crystalStrength;

        public Rule[] rules => _rules;
        [SerializeField] Rule[] _rules;

        public static GameConfig current;
    }
}