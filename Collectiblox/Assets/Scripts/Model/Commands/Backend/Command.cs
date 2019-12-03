using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    /// <summary>
    /// Send commands via the GameManager. All changes to the model must be done via Commands.
    /// Commands may carry custom data.
    /// </summary>
    /// <typeparam name="T">The type of CommandData bundled with this command.</typeparam>
    public class Command<T> : ICommand where T : CommandData
    {
        /// <summary>
        /// The command type denotes how the command is processed.
        /// </summary>
        public CommandType type { get; private set; }
        public Type dataType { get { return typeof(T); } }
        public T data { get; private set; }
        CommandData ICommand.data => data;
        public PlayerKey sender
        {
            get { return data.sender; }
        }


        public Command(CommandType type, T data)
        {
            this.type = type;
            this.data = data;
        }

        public J GetData<J>() where J : CommandData
        {
            if (data is J)
            {
                return data as J;
            }
            return null;
        }
        public bool TryGetData<J>(out J data) where J : CommandData
        {
            data = null;
            if (this.data is J)
            {
                data = this.data as J;
                return true;
            }
            return false;
        }
    }

    public interface ICommand
    {
        CommandData data { get; }
        CommandType type { get; }
        Type dataType { get; }
        T GetData<T>() where T : CommandData;
        bool TryGetData<T>(out T data) where T : CommandData;
        PlayerKey sender { get; }
    }
}
