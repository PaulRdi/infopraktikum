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
    [CreateAssetMenu(fileName = "cards_in_deck_rule.asset", menuName = "Rules/Cards in Deck Rule")]
    public class CardsInDeckRule : Rule
    {
        public override void Init()
        {

        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return AreCardsInDeck(gm, command);
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            return AreCardsInDeck(gm, command);
        }

        RuleEvaluationInfo AreCardsInDeck(GameManager gm, ICommand command)
        {
            if (command.TryGetData<DrawCardCommandData>(out DrawCardCommandData data))
            {
                bool output = gm.match.playerDatas[data.sender].drawOrder.Count > 0;
                if (!output)
                    return new RuleEvaluationInfo(output, "No cards in Deck");
                return new RuleEvaluationInfo(output);
            }
            return new RuleEvaluationInfo(true);
        }
    }
}