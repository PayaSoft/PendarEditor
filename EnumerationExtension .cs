namespace Paya.Automation.Editor
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Markup;
    using JetBrains.Annotations;

    public sealed class EnumerationExtension : MarkupExtension
    {
        #region Fields

        private Type _enumType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumerationExtension" /> class.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <exception cref="System.ArgumentNullException">enumType</exception>
        public EnumerationExtension([NotNull] Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            this.EnumType = enumType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the type of the enum.
        /// </summary>
        /// <value>
        ///     The type of the enum.
        /// </value>
        /// <exception cref="System.ArgumentException">Type must be an Enum.</exception>
        [NotNull]
        public Type EnumType
        {
            get { return this._enumType; }
            private set
            {
                if (this._enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                this._enumType = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(this.EnumType);

            return Array.AsReadOnly((
                from object enumValue in enumValues
                select new EnumerationMember(enumValue, this.GetDescription(enumValue))).ToArray());
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        [NotNull]
        private string GetDescription([NotNull] object enumValue)
        {
            var stringValue = Convert.ToString(enumValue);
            var descriptionAttribute = this.EnumType
                                           .GetField(stringValue)
                                           .GetCustomAttributes(typeof (DescriptionAttribute), false)
                                           .FirstOrDefault() as DescriptionAttribute;


            return descriptionAttribute != null
                ? descriptionAttribute.Description
                : stringValue;
        }

        #endregion

        /// <summary>
        ///     The <see cref="EnumerationMember" /> sealed class.
        /// </summary>
        [Serializable]
        [DebuggerDisplay("{Value}: {Description}")]
        public sealed class EnumerationMember : IEquatable<EnumerationMember>, ICloneable
        {
            #region Fields

            private readonly string _description;
            private readonly object _value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="EnumerationMember" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="description">The description.</param>
            internal EnumerationMember(object value, string description)
            {
                this._value = value;
                this._description = description;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the description.
            /// </summary>
            /// <value>
            ///     The description.
            /// </value>
            public string Description
            {
                get { return this._description; }
            }

            /// <summary>
            ///     Gets the value.
            /// </summary>
            /// <value>
            ///     The value.
            /// </value>
            public object Value
            {
                get { return this._value; }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Implements the operator ==.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>
            ///     The result of the operator.
            /// </returns>
            public static bool operator ==(EnumerationMember left, EnumerationMember right)
            {
                if (ReferenceEquals(left, right))
                    return true;
                if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                    return false;
                return left.Equals(right);
            }

            /// <summary>
            ///     Implements the operator !=.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>
            ///     The result of the operator.
            /// </returns>
            public static bool operator !=(EnumerationMember left, EnumerationMember right)
            {
                return !(left == right);
            }

            /// <summary>
            ///     Determines whether the specified <see cref="EnumerationMember" />, is equal to this instance.
            /// </summary>
            public bool Equals(EnumerationMember other)
            {
                if (ReferenceEquals(this, other))
                    return true;
                return !ReferenceEquals(other, null) && Equals(this.Value, other.Value);
            }

            /// <summary>
            ///     Determines whether the specified <see cref="object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
            /// <returns>
            ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                return this.Equals(obj as EnumerationMember);
            }

            /// <summary>
            ///     Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                return this.Value != null ? this.Value.GetHashCode() : 0;
            }

            /// <summary>
            ///     Returns a <see cref="string" /> that represents this instance.
            /// </summary>
            /// <returns>
            ///     A <see cref="string" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0}: {1}", this.Value, this.Description);
            }

            #endregion

            #region Explicit Interface Methods

            /// <summary>
            ///     Creates a new object that is a copy of the current instance.
            /// </summary>
            /// <returns>
            ///     A new object that is a copy of this instance.
            /// </returns>
            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }

            #endregion
        }
    }
}
