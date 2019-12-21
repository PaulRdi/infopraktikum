using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model
{
    public class HandRequest : Request<List<ICardInstance>>
    {
        PlayerKey sender;

        public HandRequest(PlayerKey sender)
        {
            this.sender = sender;
        }
        public override List<ICardInstance> RetrieveData(MatchData model)
        {
            return model.playerDatas[sender].hand;
        }
    }
}
