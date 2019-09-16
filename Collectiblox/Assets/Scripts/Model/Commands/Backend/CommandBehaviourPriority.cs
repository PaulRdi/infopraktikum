using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
{
    public enum CommandBehaviourPriority
    {
        First = int.MaxValue - 1,
        Filter = 1000,
        Default = 0,
        Late = -1000,
        Last = int.MinValue + 1
    }
}
