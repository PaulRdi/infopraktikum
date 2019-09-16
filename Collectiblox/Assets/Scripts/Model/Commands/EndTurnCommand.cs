﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox
{
    public class EndTurnCommandBehaviour : CommandBehaviour
    {
        public override List<CommandType> targetCommandTypes => 
            new List<CommandType>
            {
                CommandType.EndPhase
            };

        public override CommandBehaviourPriority priority => CommandBehaviourPriority.Last;

        public override void Execute(GameManager gm, ICommand command)
        {
            if (gm.currentStateOrderIndex >= gm.stateOrder.Count)
                gm.currentStateOrderIndex = 0;
            else
                gm.currentStateOrderIndex++;

            gm.match.currentState = gm.stateOrder[gm.currentStateOrderIndex];
        }
    }
}
