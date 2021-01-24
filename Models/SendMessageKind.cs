using System;

namespace Paya.Automation.Editor.Models
{
    using System.ComponentModel;

    [Serializable]
    public enum SendMessageKind
    {
        [Description("تکی")]
        Single,

        [Description("گروهی")]
        Group
    }
}