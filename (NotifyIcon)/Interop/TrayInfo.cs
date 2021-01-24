// Some interop code taken from Mike Marshall's AnyForm


namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
	using System.Drawing;

	/// <summary>
	///     Resolves the current tray position.
	/// </summary>
	internal static class TrayInfo
	{
		#region Public Methods and Operators

		/// <summary>
		///     Gets the position of the system tray.
		/// </summary>
		/// <returns>Tray coordinates.</returns>
		public static Point GetTrayLocation()
		{
			var info = new AppBarInfo();
			info.GetSystemTaskBarPosition();

			Rectangle rcWorkArea = info.WorkArea;

			int x = 0, y = 0;
			switch (info.Edge)
			{
			    case AppBarInfo.ScreenEdge.Left:
			        x = rcWorkArea.Left + 2;
			        y = rcWorkArea.Bottom;
			        break;
			    case AppBarInfo.ScreenEdge.Bottom:
			        x = rcWorkArea.Right;
			        y = rcWorkArea.Bottom;
			        break;
			    case AppBarInfo.ScreenEdge.Top:
			        x = rcWorkArea.Right;
			        y = rcWorkArea.Top;
			        break;
			    case AppBarInfo.ScreenEdge.Right:
			        x = rcWorkArea.Right;
			        y = rcWorkArea.Bottom;
			        break;
			}

			return new Point { X = x, Y = y };
		}

		#endregion
	}
}

// ReSharper restore FieldCanBeMadeReadOnly.Local