﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Paya.Automation.Editor.Updater
{
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using NLog;
    using Paya.Automation.Editor.Models;
    using Paya.Automation.Editor.Properties;

    public sealed class PayaClientUpdaterFactory : BaseInpc
    {
        #region Static Fields

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        private static readonly Random _Random = new Random();

        #endregion

        #region Fields

        [NotNull]
        private readonly ConcurrentDictionary<string, PayaClientUpdater> _Updaters = new ConcurrentDictionary<string, PayaClientUpdater>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PayaClientUpdaterFactory" /> class.
        /// </summary>
        internal PayaClientUpdaterFactory()
        {
            this.Load();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the updaters.</summary>
        /// <value>The updaters.</value>
        /// <autogeneratedoc />
        public ReadOnlyObservableCollection<PayaClientUpdater> Updaters
        {
            get { lock (this._Updaters) return new ReadOnlyObservableCollection<PayaClientUpdater>(new ObservableCollection<PayaClientUpdater>(this._Updaters.Values)); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Triggers the asynchronous.
        /// </summary>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <returns></returns>
        public async Task TriggerAsync(CancellationTokenSource cancellationTokenSource)
        {
            IEnumerable<PayaClientUpdater> updaters;

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Triggering program update.");

            lock (this._Updaters)
            {
                updaters = this._Updaters.Values.Where(x => x != null).ToArray();
            }

            foreach (var updater in updaters)
            {
                try
                {
                    if (_Logger.IsInfoEnabled)
                        _Logger.Info("Checking for update in {0}", updater.BaseUrl);

                    await updater.TriggerAsync(cancellationTokenSource);

                    if (_Logger.IsInfoEnabled)
                        _Logger.Info("Checked for update in {0}", updater.BaseUrl);
                }
                catch (Exception exp)
                {
                    if (_Logger.IsErrorEnabled)
                        _Logger.Error(exp, "Error while checking for update in {0}", updater.BaseUrl);
                }
            }
        }

        /// <summary>Creates the updater.</summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">baseUrl</exception>
        /// <autogeneratedoc />
        [NotNull]
        public PayaClientUpdater CreateUpdater([NotNull] string baseUrl)
        {
            if (baseUrl == null)
                throw new ArgumentNullException("baseUrl");

            Contract.EndContractBlock();

            baseUrl = Utility.NormalizeUrl(baseUrl);

            PayaClientUpdater current;

            lock (this._Updaters)
            {
                if (!this._Updaters.TryGetValue(baseUrl, out current))
                {
                    var randomInterval = TimeSpan.FromSeconds(_Random.Next(0, checked((int)(Settings.Default.UpdateCheckInterval.TotalSeconds / 2))));

                    current = new PayaClientUpdater(baseUrl, Settings.Default.UpdateCheckInterval + randomInterval);
                    this._Updaters[baseUrl] = current;

                    Save(this._Updaters.Keys);

                    this.OnPropertyChanged(@"Updaters");
                }
            }


            return current;
        }

        /// <summary>Deletes the updater.</summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <exception cref="System.ArgumentNullException">baseUrl</exception>
        /// <autogeneratedoc />
        public void DeleteUpdater([NotNull] string baseUrl)
        {
            if (baseUrl == null)
                throw new ArgumentNullException("baseUrl");

            Contract.EndContractBlock();

            baseUrl = Utility.NormalizeUrl(baseUrl);
            lock (this._Updaters)
            {
                PayaClientUpdater current;
                if (this._Updaters.TryRemove(baseUrl, out current) && current != null)
                {
                    this.OnPropertyChanged("Updaters");

                    current.Dispose();
                }
            }
        }

        /// <summary>Resets this instance.</summary>
        /// <autogeneratedoc />
        public void Reset()
        {
            lock (this._Updaters)
            {
                var updaters = this._Updaters.Values.ToArray();

                this._Updaters.Clear();

                foreach (var updater in updaters)
                    updater.Dispose();

                this.OnPropertyChanged("Updaters");
            }
        }

        #endregion

        #region Methods

        private static async void Save(IEnumerable<string> keys)
        {
            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Saving the updaters");

            try
            {
                var data = await TaskEx.Run(() => JsonConvert.SerializeObject(keys));

                IsolatedStorageSettings.ApplicationSettings["updaters"] = data;

                await IsolatedStorageSettings.SaveAsync();

                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Saved the updaters");
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error while saving updaters.");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Load()
        {
            try
            {
                var json = IsolatedStorageSettings.ApplicationSettings["updaters"];

                var baseUrls = JsonConvert.DeserializeObject<IEnumerable<string>>(json);

                if (baseUrls == null)
                    return;

                foreach (var baseUrl in baseUrls)
                {
                    this.CreateUpdater(baseUrl);
                }
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error while loading updaters.");
            }
        }

        #endregion
    }
}