using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectiblox.Model
{
    public abstract class Request<T> : IRequest where T : class
    {
        protected T _data;
        public T data => _data;

        protected event Action<T> _complete;

        /// <summary>
        /// If a Request ist saved as an IRequest we need this to retrieve the data.
        /// </summary>
        /// <typeparam name="J"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetData<J>(out J data) where J : class
        {
            data = null;
            if (this._data != null &&
                this._data is J)
            {
                data = this._data as J;
                return true;
            }
            return false;
        }

        public bool TryGetData(out T data)
        {
            data = null;
            if (this._data != null)
            {
                data = this._data;
                return true;
            }
            return false;
        }

        private void Success()
        {
            _complete?.Invoke(data as T);
        }

        public virtual void Start(Action<T> onComplete)
        {
            _complete += onComplete;
#if NETWORKING_SERVER
#else
            GameManager.instance.requestManager.TryProcessRequest<T>(this, Success);
#endif
        }
        public abstract T RetrieveData(MatchData model);
        public void RetrieveData(MatchData model, bool internalCall)
        {
            _data = RetrieveData(model);
        }
    }


    public interface IRequest
    {
        bool TryGetData<J>(out J data) where J : class;
    }
}
