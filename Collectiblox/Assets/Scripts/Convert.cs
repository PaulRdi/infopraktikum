using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Collectiblox.Model;


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
        /// <summary>
        /// Converts grid coordinates to world coordinates in the XZ-Plane  
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public static Vector3 GridToWorld(Transform transform, Vector2Int gridPos)
        {
            return transform.TransformPoint(gridPos.x, 0, gridPos.y);
        }


    }
}
