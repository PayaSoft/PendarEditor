using System;

namespace Paya.Automation.Editor.Models
{
    public sealed class HameshNode
    {
        #region Public Properties

        public long? Id { get; set; }
        public HameshNodeType NodeType { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public HameshNodeReceived ReceivedBy { get; set; }
        public string Remark { get; set; }
        public HameshNodeReceived SentBy { get; set; }

        #endregion
    }
}
