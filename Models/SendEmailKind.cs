using System;

namespace Paya.Automation.Editor.Models
{
    using System.ComponentModel;

    [Serializable]
    public enum SendEmailKind
    {
        [Description("تکی")]
        Single,

        [Description("گروهی")]
        Group
    }
}