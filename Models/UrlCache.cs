using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Models
{
    using System.Collections.Concurrent;

    [Serializable]
    internal sealed class UrlCache<T>
    {
        #region Fields

        private readonly IDictionary<UrlKeyTuple, T> _Data = new ConcurrentDictionary<UrlKeyTuple, T>();

        #endregion

        #region Public Indexers

        public T this[string url, string key]
        {
            get
            {
                var k = new UrlKeyTuple(url, key);
                T d;
                return this._Data.TryGetValue(k, out d) ? d : default(T);
            }

            set
            {
                var k = new UrlKeyTuple(url, key);
                if (Equals(value, default(T)))
                {
                    this._Data.Remove(k);
                }
                else
                {
                    this._Data[k] = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool ContainsKey(string url, string key)
        {
            var k = new UrlKeyTuple(url, key);

            return this._Data.ContainsKey(k);
        }

        #endregion

        [Serializable]
        private struct UrlKeyTuple : IEquatable<UrlKeyTuple>
        {
            #region Fields

            private readonly string _key;
            private readonly string _url;

            #endregion

            #region Constructors and Destructors

            public UrlKeyTuple(string url, string key) : this()
            {
                this._key = key;
                this._url = Utility.NormalizeUrl(url);
            }

            #endregion

            #region Public Methods and Operators

            public bool Equals(UrlKeyTuple other)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(this._key, other._key) && StringComparer.OrdinalIgnoreCase.Equals(this._url, other._url);
            }

            public override bool Equals(object obj)
            {
                if (obj is UrlKeyTuple)
                    return this.Equals((UrlKeyTuple) obj);
                return false;
            }

            public override int GetHashCode()
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(this._key) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this._url);
            }

            #endregion
        }
    }
}
