using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Behaviors
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;

    public sealed class DisableWindowMaximizeBehavior : Behavior<Window>
    {
        #region Methods

        protected override void OnAttached()
        {
            this.AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
        }

        private static void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            NativeMethods.WindowDisableMaximize((Window) sender);
        }

        #endregion

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static class NativeMethods
        {
            [DllImport(DllName, EntryPoint = "GetWindowLong")]
            private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport(DllName, EntryPoint = "SetWindowLong")]
            private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            private const int GWL_STYLE = -16;
            private const int WS_MAXIMIZEBOX = 0x10000;
            private const string DllName = "user32.dll";

            internal static void WindowDisableMaximize(Window w)
            {
                var hwnd = new WindowInteropHelper(w).Handle;
                var value = GetWindowLong(hwnd, GWL_STYLE);
                SetWindowLong(hwnd, GWL_STYLE, value & ~WS_MAXIMIZEBOX);
            }
        }
    }
}
