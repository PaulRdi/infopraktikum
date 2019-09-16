using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    public interface ICommandExecutionListener
    {
        void CommandExecuted(ICommand command, GameManager gm);
    }
}
