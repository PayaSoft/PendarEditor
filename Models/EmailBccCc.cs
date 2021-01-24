using System;

namespace Paya.Automation.Editor.Models
{
    using System.ComponentModel;

    [Serializable]
    public enum EmailBccCc
    {
        [Description("عادی")]
        None = 0,

        [Description("CC")]
        Cc,

        [Description("BCC")]
        Bcc
    }
}