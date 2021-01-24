using System;

namespace Paya.Automation.Editor.Data
{
    public interface ICustomTypeProvider
    {
        #region Public Methods and Operators

        // Methods
        Type GetCustomType();

        #endregion
    }
}