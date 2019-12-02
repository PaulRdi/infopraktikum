using System.Collections;
using System.Collections.Generic;
using Collectiblox.Model.Commands;
using UnityEngine;

namespace Collectiblox.Model.Rules
{
    public class WinGameRule : Rule
    {
        public override void Init()
        {
        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo();
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo();

        }

        bool IsGameOver(GameManager gm, ICommand command)
        {
            return false;
        }
    }
}