using System;

namespace Paya.Automation.Editor.Models
{
    using System.ComponentModel;

    [Serializable]
    public enum SendFaxKind
    {
        [Description("تکی")]
        Single,

        [Description("گروهی")]
        Group
    }
}