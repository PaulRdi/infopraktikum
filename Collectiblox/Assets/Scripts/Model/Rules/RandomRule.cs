using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collectiblox.Model.Commands;

namespace Collectiblox.Model.Rules
{
    public class RandomRule : Rule
    {
        public override void Init()
        {
            active = false;
        }

        public override RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command)
        {
            return new RuleEvaluationInfo(true);
        }

        public override RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command)
        {
            if (command.GetData<PlayCardCommandData>() != null)
            {
                float rnd = UnityEngine.Random.Range(0f, 1f);
                if (rnd < 0.5f)
                    return new RuleEvaluationInfo(false, "sad");
            }
            return new RuleEvaluationInfo(true);
        }
    }
}
