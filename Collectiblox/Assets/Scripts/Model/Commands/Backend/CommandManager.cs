using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Collectiblox.Model.Commands
{
    public class CommandManager
    {
        public Stack<ICommand> commandStack;
        List<ICommandListener> commandListeners;
        Dictionary<CommandType, List<CommandBehaviour>> registeredBehaviours;

        public CommandManager()
        {
            commandStack = new Stack<ICommand>();
            commandListeners = new List<ICommandListener>();
            registeredBehaviours = new Dictionary<CommandType, List<CommandBehaviour>>();

            foreach (CommandBehaviour behaviour in Util.GetEnumerableOfType<CommandBehaviour>())
            {
                foreach (CommandType type in behaviour.targetCommandTypes)
                    TryAddCommandBehaviour(type, behaviour);
            }
            for (int i =0; i < registeredBehaviours.Count; i++)
            {
                CommandType key = registeredBehaviours.ElementAt(i).Key;
                registeredBehaviours[key] = registeredBehaviours[key].OrderBy(b => (int)b.priority).ToList();
            }
        }

        /// <summary>
        /// Calls CommandPushed on each ICommandListener.
        /// Handle queue locking if needed.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool TrySendCommand(ICommand command)
        {
            commandStack.Push(command);
            foreach(ICommandListener commandListener in commandListeners)
            {
                commandListener.CommandPushed(command);
            }
            return true;
        }
        /// <summary>
        /// Executes each CommandBehaviour registired for the next Command in the Queue.
        /// </summary>
        /// <param name="gm"></param>
        /// <returns></returns>
        public bool TryExecuteNextCommand (GameManager gm)
        {
            if (commandStack.Count == 0)
                return false;

            ICommand command = commandStack.Pop();
            foreach(CommandBehaviour behaviour in registeredBehaviours[command.type])
            {
                behaviour.Execute(gm, command);
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
