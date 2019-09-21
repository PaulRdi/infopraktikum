﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collectiblox.Model.Commands;

namespace Collectiblox.Model.Rules
{
    public class CreaturePlayRule : Rule
    {
        public override RulePriority priority => RulePriority.Early;

        public override void Init()
        {
        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            PlayCardCommandData commandData = command.GetData<PlayCardCommandData>();

            if (commandData != null)
            {
                CardInstance<Monster> monster = commandData.cardInstance.Get<Monster>();
                if (monster != null &&
                    gm.match.fieldEntities.ContainsKey(commandData.targetTile))
                {
                    return new RuleEvaluationInfo(false, "Field filled");
                }
            }
            return new RuleEvaluationInfo(true);
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            return IsCommandExecutable(gm, command);
        }
    }
}
