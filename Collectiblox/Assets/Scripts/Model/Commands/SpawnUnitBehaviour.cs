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

        public override void OnExecute(GameManager gm, ICommand command)
        {
            PlayCardCommandData data = command.GetData<PlayCardCommandData>();
            if (data == null)
                return;
            CardInstance<Monster> monster = data.cardInstance.Get<Monster>();
            if (monster != null)
            {
                gm.match.SpawnMonster(monster, data.targetTile);
            }  
        }

        public override void OnSend(GameManager gm, ICommand command)
        {
        }
    }
}
