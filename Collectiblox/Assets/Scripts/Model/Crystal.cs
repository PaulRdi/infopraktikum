using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
{
    public class Crystal
    {
        public int strength { get; private set; }

        public Crystal(int strength)
        {
            this.strength = strength;
        }
    }
}
