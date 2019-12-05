using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collectiblox.Model.Commands;
using UnityEngine;
namespace Collectiblox.Model.Rules
{
    /// <summary>
    /// Rule to check mana spending when a card gets played.
    /// </summary>
    [CreateAssetMenu(fileName = "mana_rule.asset", menuName = "Rules/Mana Rule")]
    public class ManaRule : Rule
    {
        public override void Init()
        {

        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo(true);
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            if (command.TryGetData<PlayCardCommandData>(out PlayCardCommandData data))
            {
                if (gm.match.playerDatas[data.sender].currentMana < data.cardInstance.cost)
                    return new RuleEvaluationInfo(false, "Not enough Mana!");
            }
            return new RuleEvaluationInfo(true);
        }
    }
}
