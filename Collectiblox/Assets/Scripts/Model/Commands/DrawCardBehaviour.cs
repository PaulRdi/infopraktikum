using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    public class DrawCardBehaviour : CommandBehaviour
    {
        public override List<CommandType> targetCommandTypes => new List<CommandType>
        {
            CommandType.DrawCardFromDeck,
        };

        public override CommandBehaviourPriority priority => CommandBehaviourPriority.First;

        public override void OnExecute(GameManager gm, ICommand command)
        {
        }

        public override void OnSend(GameManager gm, ICommand command)
        {
            if (command.TryGetData<DrawCardCommandData>(out DrawCardCommandData data))
            {
                if (gm.match.playerDatas[data.sender].drawOrder.TryDraw(out ICardInstance inst))
                {
                    gm.match.playerDatas[data.sender].hand.Add(inst);
                }
            }
        }

    }
}
