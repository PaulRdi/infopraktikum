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
    [CreateAssetMenu(fileName = "card_in_hand_rule.asset", menuName = "Rules/Card in hand rule")]
    public class CardInHandRule : Rule
    {
        public override void Init()
        {

        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return IsCardInHand(gm, command);
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            return IsCardInHand(gm, command);
        }

        RuleEvaluationInfo IsCardInHand(GameManager gm, ICommand command)
        {
            if (command.TryGetData<PlayCardCommandData>(out PlayCardCommandData data))
            {
                bool output = gm.match.playerDatas[data.sender].hand.Contains(data.cardInstance);
                if (!output)
                    return new RuleEvaluationInfo(output, "Card was not in hand", data.cardInstance);
                return new RuleEvaluationInfo(output, data.cardInstance);
            }
            return new RuleEvaluationInfo(true);
        }
    }
}