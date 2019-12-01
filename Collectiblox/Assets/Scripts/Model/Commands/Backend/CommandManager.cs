using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Collectiblox.Model.Rules;

namespace Collectiblox.Model.Commands
{
    public class CommandManager
    {
        public Stack<ICommand> commandStack;
        
        List<ICommandListener> commandListeners;
        List<Rule> rules;
        Dictionary<CommandType, List<CommandBehaviour>> registeredBehaviours;

        public CommandManager(Rule[] rules)
        {
            commandStack = new Stack<ICommand>();
            commandListeners = new List<ICommandListener>();
            this.rules = new List<Rule>();
            registeredBehaviours = new Dictionary<CommandType, List<CommandBehaviour>>();

            foreach (CommandBehaviour behaviour in Util.GetEnumerableOfType<CommandBehaviour>())
            {
                foreach (CommandType type in behaviour.targetCommandTypes)
                    TryAddCommandBehaviour(type, behaviour);
            }
            foreach (Rule rule in rules)
            {
                this.rules.Add(rule);
                rule.Init();
            }
            this.rules = this.rules.OrderBy(rule => (int)rule.priority).ToList();
            for (int i =0; i < registeredBehaviours.Count; i++)
            {
                CommandType key = registeredBehaviours.ElementAt(i).Key;
                registeredBehaviours[key] = registeredBehaviours[key].OrderBy(b => (int)b.priority).ToList();
            }
        }

        /// <summary>
        /// Checks if any rules forbid the command from being executed.
        /// 
        /// Calls CommandPushed on each ICommandListener.
        /// Handle queue locking if needed.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public RuleEvaluationInfo TrySendCommand(GameManager gm, ICommand command)
        {
            foreach(Rule rule in rules)
            {
                if (!rule.active) continue;
                RuleEvaluationInfo evaluationInfo = rule.IsCommandSendable(gm, command);
                if (!evaluationInfo.actionAllowed)
                    return evaluationInfo;
            }
            commandStack.Push(command);
            foreach(CommandBehaviour behaviour in registeredBehaviours[command.type])
            {
                behaviour.OnSend(gm, command);
            }
            foreach(ICommandListener commandListener in commandListeners)
            {
                commandListener.CommandPushed(command);
            }
            return new RuleEvaluationInfo(true);
        }
        /// <summary>
        /// Executes each CommandBehaviour registired for the next Command in the Queue.
        /// </summary>
        /// <param name="gm"></param>
        /// <returns></returns>
        public bool TryExecuteNextCommand (GameManager gm, out ICommand command)
        {
            command = null;
            if (commandStack.Count == 0)
                return false;

            command = commandStack.Pop();
            foreach(Rule rule in rules)
            {
                if (!rule.active) continue;
                if (!rule.IsCommandExecutable(gm, command).actionAllowed)
                    return false;
            }
            foreach(CommandBehaviour behaviour in registeredBehaviours[command.type])
            {
                behaviour.OnExecute(gm, command);
            }
            return true;
        }

        private void TryAddCommandBehaviour(CommandType type, CommandBehaviour behaviour)
        {
            if (!registeredBehaviours.ContainsKey(type))
                registeredBehaviours.Add(type, new List<CommandBehaviour>());

            if (!registeredBehaviours[type].Contains(behaviour))
                registeredBehaviours[type].Add(behaviour);
        }


    }
}
