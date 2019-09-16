using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collectiblox.Controller
{
    public class GridCellController : MonoBehaviour
    {
        public Vector2Int gridPosition
        {
            get; private set;
        }

        public void Init(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;
        }
    }
}