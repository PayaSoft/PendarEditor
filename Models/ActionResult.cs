using System;

namespace Paya.Automation.Editor.Models
{
    [Serializable]
    public sealed class ActionResult
    {
        #region Static Fields

        public static readonly ActionResult Success = new ActionResult();

        #endregion

        #region Fields

        private readonly bool _isSuccess;
        private readonly string _message;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResult"/> class.
        /// </summary>
        public ActionResult()
            : this(true, null)
        {
        }

        public ActionResult(bool isSuccess, string message)
        {
            this._isSuccess = isSuccess;
            this._message = message;
        }

        #endregion

        #region Public Properties

        public bool IsSuccess
        {
            get { return this._isSuccess; }
        }

        public string Message
        {
            get { return this._message; }
        }

        #endregion
    }
}
