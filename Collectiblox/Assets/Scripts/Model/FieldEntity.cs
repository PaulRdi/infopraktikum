using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Collectiblox.Model
{

    public class FieldEntity<T> : IFieldEntity
    {
        public T entity;
        public J GetEntity<J>() where J : class
        {
            if (entity is J)
                return entity as J;
            return null;
        }

        public bool TryGetEntity<J>(out J entity) where J : class
        {
            entity = null;
            if (this.entity is J)
            {
                entity = this.entity as J;
                return true;
            }
            return false;
        }

        public Vector2Int gridPos { get; private set; }

        public Type type => typeof(T);

        public FieldEntity(T entity, Vector2Int position)
        {
            this.entity = entity;
            this.gridPos = position;
        }

        public override string ToString()
        {
            return "Position: " + gridPos + "\n" + entity.ToString();
        }
    }
    public interface IFieldEntity
    {
        Vector2Int gridPos { get; }
        Type type { get; }
        T GetEntity<T>() where T : class;
        bool TryGetEntity<T>(out T entity) where T : class;
        string ToString();
    }
}
