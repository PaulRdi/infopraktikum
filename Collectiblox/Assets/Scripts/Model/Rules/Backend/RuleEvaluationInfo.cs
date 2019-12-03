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
        public ICardInstance cardInstance { get; private set; }

        public RuleEvaluationInfo(bool actionAllowed, string deniedMessage, ICardInstance cardInstance = null)
        {
            this.actionAllowed = actionAllowed;
            this.ruleDeniedMessage = deniedMessage;
            this.cardInstance = cardInstance;
        }

        public RuleEvaluationInfo(bool actionAllowed, ICardInstance cardInstance = null)
        {
            this.actionAllowed = actionAllowed;
            this.ruleDeniedMessage = "";
            this.cardInstance = cardInstance;
        }
    }
}
