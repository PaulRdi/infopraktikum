using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    public class SpawnUnitBehaviour : CommandBehaviour
    {
        public override List<CommandType> targetCommandTypes =>
            new List<CommandType>
            {
                CommandType.PlayCard
            };

        public override CommandBehaviourPriority priority => CommandBehaviourPriority.Default;

        public override void Execute(GameManager gm, ICommand command)
        {
            PlayCardCommandData data = command.GetData<PlayCardCommandData>();
            CardInstance<Monster> monster = data.cardInstance.Get<Monster>();
            if (monster != null)
            {
                gm.match.SpawnMonster(monster, data.targetTile);
            }  
        }
    }
}
