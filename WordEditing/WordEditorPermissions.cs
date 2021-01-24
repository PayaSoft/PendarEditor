namespace Paya.Automation.Editor.WordEditing
{
    using System;

    [Flags]
    [Serializable]
    public enum WordEditorPermissions
    {
        None = 0,

        Change = 1,

        Print = 2
    }
}