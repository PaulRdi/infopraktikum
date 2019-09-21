using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collectiblox.Model.Commands;
namespace Collectiblox.Model.Rules
{
    /// <summary>
    /// Rules change game tate when Commands get sent or executed.
    /// Rules also evaluate game state.
    /// </summary>
    public abstract class Rule
    {
        /// <summary>
        /// Is executed before the command gets put on the stack.
        /// This means, the player hasn't commited to anything if IsCommandSendable returns false.
        /// 
        /// If the rule is irrelevant in any case (e.g. type of command does not match rule) this function must return true.
        /// </summary>
        /// <param name="gm">Contains all information about the current world state.
        /// Access gm.match for all info about the current world state.</param>
        /// <param name="command"></param>
        /// <returns>True if sending the Command is legal with the current board state.
        /// False if sending the Command is not legal with the current board state.</returns>
        public abstract RuleEvaluationInfo IsCommandSendable(GameManager gm, ICommand command);
        /// <summary>
        /// Is executed after the command gets popped from the stack but before the Command gets executed.
        /// CommandBehaviour execution will be skipped if this returns false.
        /// 
        /// If the rule is irrelevant in any case (e.g. type of command does not match rule) this function must return true.
        /// </summary>
        /// <param name="gm">Contains all information about the current world state.
        /// Access gm.match for all info about the current world state.</param>
        /// <param name="command"></param>
        /// <returns>True if executing the Command is legal with the current board state.
        /// False if executing the command is illegal with the current board state</returns>
        public abstract RuleEvaluationInfo IsCommandExecutable(GameManager gm, ICommand command);

        public abstract void Init();

        public bool active {
            get { return _active; }
            protected set { _active = value; }
        }
        protected bool _active = true;
        public void  SetActive(bool active)
        {
            this.active = active;
        }
        public virtual RulePriority priority => RulePriority.Default;

    }
}
