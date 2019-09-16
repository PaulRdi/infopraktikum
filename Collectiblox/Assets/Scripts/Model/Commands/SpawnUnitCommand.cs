using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
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

            if (data.cardInstance.baseData.type == typeof(Monster))
            {
                CardInstance<Monster> monster = (CardInstance<Monster>)data.cardInstance;
                gm.match.SpawnMonster(monster, data.targetTile);
            }
        }
    }
}
