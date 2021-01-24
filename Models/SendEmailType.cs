using System;

namespace Paya.Automation.Editor.Models
{
    using System.ComponentModel;

    [Serializable]
    public enum SendEmailType
    {
        [Description("دلخواه")]
        Custom,

        [Description("انتخاب از دفترچه تلفن")]
        PhoneBook,

        [Description("گروه")]
        Group,

        [Description("انتخاب از مشتریان")]
        Customer,

        [Description("انتخاب از مشتریان بالقوه")]
        Defacto
    }
}
 