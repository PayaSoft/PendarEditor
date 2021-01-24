namespace Paya.Automation.Editor.WordEditing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    /// Specifies the <see cref="IWordEditor"/> interface.
    /// </summary>
    public interface IWordEditor
    {
        Task LaunchAsync([CanBeNull] CancellationTokenSource cancellationTokenSouce = null);

        Task PrintAsync(bool withPreview, [CanBeNull] CancellationTokenSource cancellationTokenSouce = null);

        bool CanChange { get; }

        bool CanPrint { get; }

        byte[] GetContents();

        event EventHandler Changed;
        event EventHandler Closed;
        event EventHandler Finished;
        event EventHandler NewDocument;
        event EventHandler Opened;
        event EventHandler Opening;
        event EventHandler Printing;
        event EventHandler Quit;
        event EventHandler Saving;
        event EventHandler Startup;
    }
}