namespace Paya.Automation.Editor
{
	using System;
	using System.Windows.Threading;

	/// <summary>Specifies the <see cref="UIHelper" /> static class.</summary>
	public static class UIHelper
	{
		#region Public Methods and Operators

		public static void SetTimeout(double milliseconds, Action func)
		{
			SetTimeout(TimeSpan.FromMilliseconds(milliseconds), func);
		}

		public static void SetTimeout(double milliseconds, Action<object> func, object data)
		{
			SetTimeout(TimeSpan.FromMilliseconds(milliseconds), func, data);
		}

		public static void SetTimeout<T>(double milliseconds, Action<T> func, T data)
		{
			SetTimeout(TimeSpan.FromMilliseconds(milliseconds), func, data);
		}

		public static void SetTimeout(TimeSpan time, Action func)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}

			var timer = new DispatcherTimerContainingAction { Interval = time, Action = func };
			StartTimer(timer);
		}

		public static void SetTimeout(TimeSpan time, Action<object> func, object data)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}

			var timer = new DispatcherTimerContainingAction { Interval = time, ActionWithData = func, Data = data };
			StartTimer(timer);
		}

		public static void SetTimeout<T>(TimeSpan time, Action<T> func, T data)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}

			var timer = new DispatcherTimerContainingAction { Interval = time, ActionWithData = d => { if (d is T) func((T)d); }, Data = data };
			StartTimer(timer);
		}

		#endregion

		#region Methods

		private static void StartTimer(DispatcherTimer timer)
		{
			timer.Tick += OnTimeout;
			timer.Start();
		}

		private static void OnTimeout(object sender, EventArgs arg)
		{
			var t = (DispatcherTimerContainingAction)sender;
			t.Stop();
			t.Tick -= OnTimeout;
			if (t.Action != null)
				t.Action();
			if (t.ActionWithData != null)
				t.ActionWithData(t.Data);
			t.Data = null;
			t.ActionWithData = null;
			t.Action = null;
		}

		#endregion

		private sealed class DispatcherTimerContainingAction : DispatcherTimer
		{
            #region Properties

            internal Action Action { get; set; }

			internal Action<object> ActionWithData { get; set; }

			internal object Data { get; set; }

			#endregion
		}
	}
}