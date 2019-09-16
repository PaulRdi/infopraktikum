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
        J GetEntity<J>() where J : class
        {
            return entity as J;
        }

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
        T GetEntity<T>() where T : class;
    }
}
