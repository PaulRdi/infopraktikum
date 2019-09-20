using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model
{
    public class PlayerData
    {
        public DrawOrder drawOrder;
        public List<IFieldEntity> fieldEntities;
        public List<ICardInstance> hand;
        public PlayerKey key;
        public int currentMana;
        public int maxMana;

        public PlayerData(
            DrawOrder drawOrder, 
            PlayerKey key,
            int currentMana,
            int maxMana)
        {
            this.drawOrder = drawOrder;
            this.fieldEntities = new List<IFieldEntity>();
            this.hand = new List<ICardInstance>();
            this.key = key;
            this.currentMana = currentMana;
            this.maxMana = maxMana;
        }
    }
}
