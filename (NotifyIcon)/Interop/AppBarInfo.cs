// Some interop code taken from Mike Marshall's AnyForm
// ReSharper disable FieldCanBeMadeReadOnly.Local
namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Runtime.InteropServices;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal sealed class AppBarInfo
	{
		#region Constants

		private const int ABE_BOTTOM = 3;

		private const int ABE_LEFT = 0;

		private const int ABE_RIGHT = 2;

		private const int ABE_TOP = 1;

		private const int ABM_GETTASKBARPOS = 0x00000005;

		// SystemParametersInfo constants
		private const uint SPI_GETWORKAREA = 0x0030;

		#endregion

		#region Fields

		private APPBARDATA m_data;

		#endregion

		#region Enums

		public enum ScreenEdge
		{
			Undefined = -1,

			Left = ABE_LEFT,

			Top = ABE_TOP,

			Right = ABE_RIGHT,

			Bottom = ABE_BOTTOM
		}

		#endregion

		#region Public Properties

		public ScreenEdge Edge
		{
			get
			{
				return (ScreenEdge)this.m_data.uEdge;
			}
		}

		public Rectangle WorkArea
		{
			get
			{
				var rc = new RECT();
				IntPtr rawRect = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
				int bResult = NativeMethods.SystemParametersInfo(SPI_GETWORKAREA, 0, rawRect, 0);
				rc = (RECT)Marshal.PtrToStructure(rawRect, rc.GetType());

				if (bResult == 1)
				{
					Marshal.FreeHGlobal(rawRect);
					return new Rectangle(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
				}

				return new Rectangle(0, 0, 0, 0);
			}
		}

		#endregion

		#region Public Methods and Operators

		public void GetPosition(string strClassName, string strWindowName)
		{
			this.m_data = new APPBARDATA();
			this.m_data.cbSize = (uint)Marshal.SizeOf(this.m_data.GetType());

			IntPtr hWnd = NativeMethods.FindWindow(strClassName, strWindowName);

			if (hWnd != IntPtr.Zero)
			{
				uint uResult = NativeMethods.SHAppBarMessage(ABM_GETTASKBARPOS, ref this.m_data);

				if (uResult != 1)
				{
					throw new InvalidOperationException("Failed to communicate with the given AppBar");
				}
			}
			else
			{
				throw new InvalidOperationException("Failed to find an AppBar that matched the given criteria");
			}
		}

		public void GetSystemTaskBarPosition()
		{
			this.GetPosition("Shell_TrayWnd", null);
		}

		#endregion

		/// <summary>
		/// Specifies the <see cref="NativeMethods"/> static sclass. 
		/// </summary>
		private static class NativeMethods
		{
			#region Methods

			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return")]
			[DllImport("shell32.dll")]
			internal static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA data);

			[DllImport("user32.dll")]
			internal static extern int SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

			#endregion
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct APPBARDATA
		{
			public uint cbSize;

		    private IntPtr hWnd;

		    private uint uCallbackMessage;

			public uint uEdge;

		    private RECT rc;

		    private int lParam;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left;

			public int top;

			public int right;

			public int bottom;
		}
	}
}

// ReSharper restore FieldCanBeMadeReadOnly.Local