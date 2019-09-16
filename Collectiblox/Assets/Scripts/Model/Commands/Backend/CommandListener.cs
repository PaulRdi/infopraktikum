using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Commands
{
    public interface ICommandListener
    {
        void CommandPushed(ICommand command);
    }
}
