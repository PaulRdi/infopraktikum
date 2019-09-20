using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collectiblox.Model.Commands;

namespace Collectiblox.Model.Rules
{
    /// <summary>
    /// Rule to check mana spending when a card gets played.
    /// </summary>
    public class ManaRule : Rule
    {
        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo(true);
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            PlayCardCommandData data = command.GetData<PlayCardCommandData>();

            if (data != null)
            {
                if (gm.match.playerDatas[data.sender].currentMana < data.cardInstance.cost)
                    return new RuleEvaluationInfo(false, "Not enough Mana!");

                gm.match.playerDatas[data.sender].currentMana -= data.cardInstance.cost;
            }
            return new RuleEvaluationInfo(true);
        }
    }
}
