using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model.Rules
{
    public enum RulePriority
    {
        Default = 0,
        Early = 1000,
        Late = -1000
    }
}
