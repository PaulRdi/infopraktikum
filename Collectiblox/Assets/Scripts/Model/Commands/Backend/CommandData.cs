using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
{
    /// <summary>
    /// would contain interface to possible serialization logic!
    /// </summary>
    public abstract class CommandData
    {
        public abstract PlayerKey sender { get; }
    }
}
