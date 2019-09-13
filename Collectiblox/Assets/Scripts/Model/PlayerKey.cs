using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
{
    public class PlayerKey 
    {
        public long uid => _uid;
        private long _uid;

        public PlayerKey()
        {
            _uid = (long)new Random().NextDouble();
        }

        public bool Equals(PlayerKey other)
        {
            return _uid == other._uid;
        }

        public static bool operator ==(PlayerKey lhs, PlayerKey rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PlayerKey lhs, PlayerKey rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
