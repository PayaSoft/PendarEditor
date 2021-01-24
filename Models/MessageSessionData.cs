using System.Threading.Tasks;

namespace Paya.Automation.Editor.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using NLog;

    [Serializable]
    public sealed class MessageSessionData : IEquatable<MessageSessionData>
    {
        #region Static Fields

        [NonSerialized]
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private readonly string _baseUrl;
        private readonly string _folderId;
        private readonly int _messageId;
        private readonly int _messageSerial;
        private readonly int _storeIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageSessionData" /> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="storeIndex">Index of the store.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="messageSerial">The message serial.</param>
        /// <param name="cookies">The cookies.</param>
        public MessageSessionData(string baseUrl, int storeIndex, string folderId, int messageId, int messageSerial, IDictionary<string, string> cookies)
        {
            this._baseUrl = Utility.NormalizeUrl(baseUrl);
            this._messageId = messageId;
            this._folderId = folderId;
            this._storeIndex = storeIndex;
            this._messageSerial = messageSerial;
            this.Cookies = cookies;
        }

        #endregion

        #region Public Properties

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string BaseUrl
        {
            get { return this._baseUrl; }
        }

        public string FolderId
        {
            get { return this._folderId; }
        }

        public int MessageId
        {
            get { return this._messageId; }
        }

        public int MessageSerial
        {
            get { return this._messageSerial; }
        }

        public ISet<string> Roles { get; internal set; }

        public int StoreIndex
        {
            get { return this._storeIndex; }
        }

        #endregion

        #region Properties

        internal IDictionary<string, string> Cookies { get; set; }

        internal JObject MessageData { get; set; }

        #endregion

        #region Public Methods and Operators

        public bool Equals(MessageSessionData other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (ReferenceEquals(other, null))
                return false;
            return this.MessageSerial == other.MessageSerial && StringComparer.OrdinalIgnoreCase.Equals(this.BaseUrl, other.BaseUrl);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MessageSessionData);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.MessageSerial.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.BaseUrl);
        }

        #endregion

       
    }
}
