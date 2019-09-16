using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Collectiblox
{
    public class PlayCardCommandData : CommandData
    {
        public override PlayerKey sender => _sender;
        PlayerKey _sender;

        public ICardInstance cardInstance { get; private set; }
        public Vector2Int targetTile { get; private set; }

        public PlayCardCommandData(
            PlayerKey sender, 
            ICardInstance cardInstance, 
            Vector2Int targetTile)
        {
            this._sender = sender;
            this.cardInstance = cardInstance;
            this.targetTile = targetTile;
        }
    }
}
