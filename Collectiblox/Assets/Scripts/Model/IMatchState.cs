using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Collectiblox.Model
{
    public interface IMatchState
    {
        void Enter(
            MatchState state, 
            MatchData match);

        void Exit(
            MatchState state, 
            MatchData match);

    }
}
