using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Collectiblox
{
    public static class Convert
    {

        public static Type ToType(AttributeType type)
        {
            switch (type)
            {
                case AttributeType.Integer:
                    return typeof(int);
                case AttributeType.String:
                    return typeof(string);
            }
            return null;
        }
    }
}
