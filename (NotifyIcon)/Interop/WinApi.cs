namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Win32 API imports.
	/// </summary>
	internal static class NativeMethods
	{
		#region Public Methods and Operators

		/// <summary>
		///     Creates the helper window that receives messages from the taskar icon.
		/// </summary>
		[DllImport("USER32.DLL", EntryPoint = "CreateWindowExW", SetLastError = true)]
		public static extern IntPtr CreateWindowEx(int dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		/// <summary>
		///     Processes a default windows procedure.
		/// </summary>
		[DllImport("USER32.DLL", EntryPoint = "DefWindowProc")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

		/// <summary>
		///     Used to destroy the hidden helper window that receives messages from the
		///     taskbar icon.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		[DllImport("USER32.DLL", SetLastError = true, EntryPoint = "DestroyWindow")]
        public static extern bool DestroyWindow(IntPtr hWnd);

		[DllImport("USER32.DLL", SetLastError = true, EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(ref Point lpPoint);

		/// <summary>
		///     Gets the maximum number of milliseconds that can elapse between a
		///     first click and a second click for the OS to consider the
		///     mouse action a double-click.
		/// </summary>
		/// <returns>
		///     The maximum amount of time, in milliseconds, that can
		///     elapse between a first click and a second click for the OS to
		///     consider the mouse action a double-click.
		/// </returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "GetDoubleClickTime")]
        public static extern int GetDoubleClickTime();

		/// <summary>
		///     Gets the screen coordinates of the current mouse position.
		/// </summary>
		[DllImport("USER32.DLL", SetLastError = true, EntryPoint = "GetPhysicalCursorPos")]
        public static extern bool GetPhysicalCursorPos(ref Point lpPoint);

		/// <summary>
		///     Registers the helper window class.
		/// </summary>
		[DllImport("USER32.DLL", EntryPoint = "RegisterClassW", SetLastError = true)]
		public static extern short RegisterClass(ref WindowClass lpWndClass);

		/// <summary>
		///     Registers a listener for a window message.
		/// </summary>
		/// <param name="lpString"></param>
		/// <returns></returns>
		[DllImport("User32.Dll", EntryPoint = "RegisterWindowMessageW")]
		public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

		/// <summary>
		///     Gives focus to a given window.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		[DllImport("USER32.DLL", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		///     Creates, updates or deletes the taskbar icon.
		/// </summary>
		[DllImport("shell32.Dll", CharSet = CharSet.Unicode, EntryPoint = "Shell_NotifyIcon")]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);

		#endregion
	}
}