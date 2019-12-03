using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collectiblox.Model.Commands;
using UnityEngine;
namespace Collectiblox.Model.Rules
{
    [CreateAssetMenu(fileName = "rule.asset", menuName = "Rules/Creature Play Rule")]
    public class CreaturePlayRule : Rule
    {
        public override RulePriority priority => RulePriority.Early;

        public override void Init()
        {
        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            PlayCardCommandData commandData = command.GetData<PlayCardCommandData>();
            CardInstance<Monster> monster = null;

            if (commandData != null)
            {
                monster = commandData.cardInstance.Get<Monster>();
                if (monster != null &&
                    gm.match.fieldEntities.ContainsKey(commandData.targetTile))
                {
                    return new RuleEvaluationInfo(false, "Field filled");
                }
                else
                {
                    return new RuleEvaluationInfo(true, monster);
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
