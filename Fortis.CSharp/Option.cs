//-----------------------------------------------------------------------
// <copyright file="Option.cs" company="Open Source">
//     Defensive coding libaries
//     Code defensively
// </copyright>
// <author>Huw Simpson</author>
//-----------------------------------------------------------------------

namespace Fortis.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Option type helper functions.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Option",
        Justification = "Keywords in C# are lowercase, not sure why it's complaining")]
    public static class Option
    {
        /// <summary>
        /// None, represents the absence of a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value, when Some.</typeparam>
        /// <returns>An instance of None</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The design requires it")]
        public static Option<TValue>.None None<TValue>()
        {
            return new Option<TValue>.None();
        }

        /// <summary>
        /// Some of TValue, wraps a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An instance of Some of TValue</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue>.Some Some<TValue>(TValue value)
        {
            Contract.Requires(value != null);

            return new Option<TValue>.Some(value);
        }

        /// <summary>
        /// Creates an option from a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>None when the value is null, otherwise Some of TValue</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> OfValue<TValue>(TValue value)
        {
            return (value == null) ? None<TValue>() as Option<TValue> : Some(value);
        }

        /// <summary>
        /// Create an option from a boolean value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <returns>Some of Unit when value <c>true</c>, otherwise None.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<Unit> OfBool(bool value)
        {
            if (value)
            {
                return new Option<Unit>.Some(new Unit());
            }
            else
            {
                return new Option<Unit>.None();
            }
        }

        /// <summary>
        /// Guards the specified callback.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="callback">The callback to guard.</param>
        /// <returns>An option of TValue.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The design warrants it"),
         SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
             Justification = "Code contracts validates it")]
        public static Option<TValue> Guard<TValue>(Func<TValue> callback)
        {
            Contract.Requires(callback != null);

            try
            {
                return OfValue(callback());
            }
            catch
            {
                return new Option<TValue>.None();
            }
        }
    }

    /// <summary>
    /// Option Type.
    /// Use as a replacement for null, to represent the presence or absence of a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Option",
        Justification = "Keywords in C# are lowercase, not sure why it's complaining")]
    public abstract class Option<TValue>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="optionA">The option a.</param>
        /// <param name="optionB">The option b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Option<TValue> optionA, Option<TValue> optionB)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(optionA, optionB))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)optionA == null) || ((object)optionB == null))
            {
                return false;
            }

            // Return true if the fields match:
            return optionA.Equals(optionB);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="optionA">The option a.</param>
        /// <param name="optionB">The option b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Option<TValue> optionA, Option<TValue> optionB)
        {
            return !(optionA == optionB);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var some = this as Some;
            return ((object)some == null) ? 0 : some.Value.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Option<TValue>)
            {
                var thisSome = this as Some;
                var otherSome = obj as Some;

                if ((object)thisSome != null && (object)otherSome != null)
                {
                    return object.Equals(thisSome.Value, otherSome.Value);
                }
                else
                {
                    return this is None && obj is None;
                }
            }

            return false;
        }

        /// <summary>
        /// The None Option.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "The design warrants it")]
        public sealed class None : Option<TValue>
        {
        }

        /// <summary>
        /// The Some Option.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "The design warrants it")]
        public sealed class Some : Option<TValue>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Some"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Some(TValue value)
            {
                Contract.Requires(value != null);

                this.Value = value;
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public TValue Value
            {
                get;
                private set;
            }
        }
    }
}
