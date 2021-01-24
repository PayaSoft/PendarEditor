namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
	using System.Runtime.InteropServices;

	/// <summary>
	///     Win API struct providing coordinates for a single point.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct Point
	{
		/// <summary>
		///     X coordinate.
		/// </summary>
		public int X;

		/// <summary>
		///     Y coordinate.
		/// </summary>
		public int Y;
	}
}