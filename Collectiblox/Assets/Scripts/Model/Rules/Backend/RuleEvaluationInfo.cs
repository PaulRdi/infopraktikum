using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Rules
{
    public struct RuleEvaluationInfo
    {
        public bool actionAllowed { get; private set; }
        public string ruleDeniedMessage { get; private set; }

        public RuleEvaluationInfo(bool actionAllowed, string deniedMessage)
        {
            this.actionAllowed = actionAllowed;
            this.ruleDeniedMessage = deniedMessage;
        }

        public RuleEvaluationInfo(bool actionAllowed)
        {
            this.actionAllowed = actionAllowed;
            this.ruleDeniedMessage = "";
        }
    }
}
