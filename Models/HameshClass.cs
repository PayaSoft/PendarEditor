using System;

namespace Paya.Automation.Editor.Models
{
    public sealed class HameshClass
    {
        #region Public Properties

        public bool HasAnyNode { get; set; }
        public HameshNode[] Nodes { get; set; }

        #endregion

        //new { HasAnyNode = true, Nodes = new[] { new { Id = 1, ReceivedBy = new { ID = 0, Title = "" }, SentBy = new { ID = 0, Title = "" }, ReceivedAt = DateTime.MinValue, Remark = "", NodeType = new { Title = "" } } } 
    }
}
