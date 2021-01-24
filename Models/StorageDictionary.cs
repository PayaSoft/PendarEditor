using System;

namespace Paya.Automation.Editor.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using NLog;

    [Serializable]
    public sealed class StorageDictionary : IDictionary<string, string>
    {
        #region Static Fields

        [NonSerialized]
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private readonly Dictionary<string, string> _Dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="StorageDictionary" /> class from being created externally.
        /// </summary>
        private StorageDictionary()
        {
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get { return this._Dic.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<string> Keys
        {
            get { return this._Dic.Keys; }
        }

        public ICollection<string> Values
        {
            get { return this._Dic.Values; }
        }

        #endregion

        #region Public Indexers

        public string this[string key]
        {
            get
            {
                string value;
                return this._Dic.TryGetValue(key, out value) ? value : null;
            }
            set { this._Dic[key] = value; }
        }

        #endregion

        #region Public Methods and Operators

        public void Add(KeyValuePair<string, string> item)
        {
            this._Dic[item.Key] = item.Value;
        }

        public void Add(string key, string value)
        {
            this._Dic[key] = value;
        }

        public void Clear()
        {
            this._Dic.Clear();
        }

        public bool Contains(string key)
        {
            return this._Dic.ContainsKey(key);
        }

        public bool ContainsKey(string key)
        {
            return this._Dic.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this._Dic.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>) this._Dic).Remove(item);
        }

        public bool Remove(string key)
        {
            return this._Dic.Remove(key);
        }

        public async Task SaveAsync(string filePath)
        {
            var json = JsonConvert.SerializeObject(this._Dic);

            var path = GetNonIsolatedApplicationPath(filePath);

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, true))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    await sw.WriteAsync(json);
                }
            }

        }

        public bool TryGetValue(string key, out string value)
        {
            return this._Dic.TryGetValue(key, out value);
        }

        #endregion

        #region Explicit Interface Methods

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>) this._Dic).Contains(item);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, string>>) this._Dic).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Methods

        [NotNull]
        internal static StorageDictionary Load(string filePath)
        {
            try
            {
                var path = GetNonIsolatedApplicationPath(filePath);

                if (!File.Exists(path))
                    return new StorageDictionary();

                string json;

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        json = sr.ReadToEnd();
                    }
                }

                var data = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);

                var s = new StorageDictionary();

                if (data != null)
                {
                    foreach (var item in data.Where(item => item.Key != null))
                    {
                        s._Dic[item.Key] = item.Value;
                    }
                }

                return s;
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error while loading storage dictionary: {0}", filePath);

                return new StorageDictionary();
            }
        }

        private static string GetNonIsolatedApplicationPath(string filePath)
        {
            var basePathHash = StringComparer.OrdinalIgnoreCase.GetHashCode(Assembly.GetExecutingAssembly().Location).ToString("X");

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Paya", "Pendar", "Editor", basePathHash, filePath);

            var directory = Path.GetDirectoryName(path);

            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return path;
        }

        #endregion
    }
}
