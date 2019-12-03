using System.Collections;
using System.Collections.Generic;
using Collectiblox.Model.Commands;
using UnityEngine;

namespace Collectiblox.Model.Rules
{
    [CreateAssetMenu(fileName = "win_game_rule.asset", menuName = "Rules/Win Game Rule")]
    public class WinGameRule : Rule
    {
        public override void Init()
        {
        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo(!IsGameOver(gm.match, command));
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo(true);

        }

        bool IsGameOver(MatchData match, ICommand command)
        {
            int p1Strength = match.GetStrengthForPlayerAtPosition(match.crystalPosition, match.player1Key);
            int p2Strength = match.GetStrengthForPlayerAtPosition(match.crystalPosition, match.player2Key);

            if (p1Strength > match.crystal.strength)
            {
                Debug.Log("player 1 wins");
                return true;
            }
            else if (p2Strength > match.crystal.strength)
            {
                Debug.Log("player 2 wins");
                return true;
            }
            return false;
        }
    }
}