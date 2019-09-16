using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Collectiblox
{

    public class FieldEntity<T> : IFieldEntity
    {
        public T entity;

        public Vector2Int gridPos { get; private set; }

        public Type type => typeof(T);

        public FieldEntity(T entity, Vector2Int position)
        {
            this.entity = entity;
            this.gridPos = position;
        }
    }
    public interface IFieldEntity
    {
        Vector2Int gridPos { get; }
        Type type { get; }
    }
}
