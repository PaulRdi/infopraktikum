using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
{
    public enum MatchStateType
    {
        Idle,
        Draw,
        Main,
        Evaluate,
        IncreaseMana,

    }
    public class MatchState
    {
        //does match state keep info (we keep class instance for each state behaviour)
        public MatchStateType name { get; private set; }
        public PlayerKey controllingPlayer { get; private set; }
        public PlayerKey otherPlayer { get; private set; }

        public MatchState(
            MatchStateType name, 
            PlayerKey controllingPlayer, 
            PlayerKey otherPlayer)
        {
            this.controllingPlayer = controllingPlayer;
            this.otherPlayer = otherPlayer;
            this.name = name;
        }        
    }
}
