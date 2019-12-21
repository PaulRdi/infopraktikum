#define NETWORKING_LOCAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model
{
    public class RequestManager
    {
        MatchData model;
        public RequestManager (MatchData model)
        {
            this.model = model;
        }

        public bool TryProcessRequest<T>(Request<T> request, Action success) where T : class
        {
            request.RetrieveData(model, true);
            if (request.TryGetData<T>(out T data))
            {
                success?.Invoke();
                return true;
            }
            return false;
        }
    }
}
