using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    public class PlayCardBehaviour : CommandBehaviour
    {
        public override List<CommandType> targetCommandTypes => new List<CommandType>
        {
            CommandType.PlayCard,
        };

        public override CommandBehaviourPriority priority => CommandBehaviourPriority.First;

        public override void OnExecute(GameManager gm, ICommand command)
        {
        }

        public override void OnSend(GameManager gm, ICommand command)
        {
            if (command.TryGetData<PlayCardCommandData>(out PlayCardCommandData data))
            {
                UnityEngine.Debug.Log(data.cardInstance.cost);
                gm.match.playerDatas[command.sender].currentMana -= data.cardInstance.cost;

                if (gm.match.playerDatas[command.sender].hand.Contains(data.cardInstance))
                {
                    gm.match.playerDatas[command.sender].hand.Remove(data.cardInstance);
                    PlayCard(gm.match, data);
                }
            }
        }

        private void PlayCard(MatchData match, PlayCardCommandData commandData)
        {
            if (commandData.cardInstance.TryGetStrongType<Monster>(out CardInstance<Monster> monster))
            {
                match.SpawnMonster(monster, commandData.targetTile, commandData.sender);
            }
        }
    }
}
