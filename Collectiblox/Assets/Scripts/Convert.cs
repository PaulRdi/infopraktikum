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

        public static Vector3 GridToWorld(Matrix4x4 matrix, Vector2Int gridPos)
        {
            return matrix * new Vector3(gridPos.x, 0, gridPos.y);
        }
    }
}
