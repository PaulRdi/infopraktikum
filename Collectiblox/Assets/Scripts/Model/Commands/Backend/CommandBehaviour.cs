using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    /// <summary>
    /// Implement Behaviour for specific commands.
    /// A Command Behaviour is how the world state changes when a certain command is executed.
    /// </summary>
    public abstract class CommandBehaviour
    {
        /// <summary>
        /// Logic which happens when the Command is sent to the stack.
        /// </summary>
        /// <param name="gm">The GameManager contains all information about the current world state. 
        ///                  Access gm.worldData to find data.</param>
        /// <param name="command">The command being executed. Use command.GetData to get specific CommandData.</param>
        public abstract void OnSend(GameManager gm, ICommand command);
        /// <summary>
        /// Logic happening when executing the behaviour goes here.
        /// </summary>
        /// <param name="gm">The GameManager contains all information about the current world state. 
        ///                  Access gm.worldData to find data.</param>
        ///                  
        /// <param name="command"> The command being executed. Use command.GetData to get specific CommandData.</param>
        public abstract void OnExecute(GameManager gm, ICommand command);
        /// <summary>
        /// Define the command types for which this behaviour is executed.
        /// </summary>
        public abstract List<CommandType> targetCommandTypes { get; }
        /// <summary>
        /// Define the priority for which this behaviour is executed.
        /// Behaviours with a higher priority are executed earlier.s
        /// </summary>
        public abstract CommandBehaviourPriority priority { get; }
    }
}
