using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor
{
    using System.Collections;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;

    [Serializable]
    public sealed class PayaCommandLineParser : ILookup<string, string>
    {
        #region Static Fields

        private static readonly Regex Unix1CmdRegex = new Regex(@"\s*\-(?<switch>\w)+\s*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex Unix2CmdRegex = new Regex(@"\s*\-\-(?<switch>\w+)([\=\:]\b(?<value>\w+)\b)?\s*\s*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex WindowsCmdRegex = new Regex(@"\s*\/(?<switch>\b\w+\b)([\=\:]\b(?<value>\w+)\b)?\s*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        #endregion

        #region Fields

        private readonly ILookup<string, string> _Switches;

        #endregion

        #region Constructors and Destructors

        public PayaCommandLineParser([NotNull] IEnumerable<string> args, [NotNull] StringComparer stringComparer)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (stringComparer == null) throw new ArgumentNullException("stringComparer");

            this._Switches = ParseCommands(args, stringComparer);
        }

        public PayaCommandLineParser([NotNull] IEnumerable<string> args)
            : this(args, StringComparer.OrdinalIgnoreCase)
        {
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get { return this._Switches.Count; }
        }

        #endregion

        #region Public Indexers

        public IEnumerable<string> this[string key]
        {
            get { return this._Switches[key]; }
        }

        #endregion

        #region Public Methods and Operators

        public bool ContainsAny([NotNull] params string[] keys)
        {
            if (keys == null) throw new ArgumentNullException("keys");

            return keys.Any(k => this._Switches.Contains(k));
        }

        public bool Contains(string key)
        {
            return this._Switches.Contains(key);
        }

        public IEnumerator<IGrouping<string, string>> GetEnumerator()
        {
            return this._Switches.GetEnumerator();
        }

        #endregion

        #region Explicit Interface Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Methods

        private static ILookup<string, string> ParseCommands([NotNull] IEnumerable<string> args, [NotNull] StringComparer comparer)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var items = from matches in args.Select(arg => WindowsCmdRegex.Matches(arg).Cast<Match>().Concat(Unix1CmdRegex.Matches(arg).Cast<Match>()).Concat(Unix2CmdRegex.Matches(arg).Cast<Match>()))
                        from m in matches
                        where m.Success
                        let sw = m.Groups["switch"]
                        where sw.Success
                        let val = m.Groups["value"]
                        select new KeyValuePair<string, string>(sw.Value, val.Success ? val.Value : string.Empty);

            return items.ToLookup(x => x.Key, y => y.Value, comparer);
        }

        #endregion
    }
}
