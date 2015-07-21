//-----------------------------------------------------------------------
// <copyright file="OptionExtension.cs" company="Open Source">
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
    /// Option Extension Methods.
    /// </summary>
    public static class OptionExtension
    {
        /// <summary>
        /// Checks whether the specified option exists, is Some.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="option">The option.</param>
        /// <returns>A <c>bool</c></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists<TValue>(this Option<TValue> option)
        {
            Contract.Requires(option != null);

            return (option is Option<TValue>.None) == false;
        }

        /// <summary>
        /// Maps the specified option.
        /// </summary>
        /// <typeparam name="TInValue">The type of the in value.</typeparam>
        /// <typeparam name="TOutValue">The type of the out value.</typeparam>
        /// <param name="option">The option.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>An option of TOutValue</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
            Justification = "Validated by code contracts")]
        public static Option<TOutValue> Map<TInValue, TOutValue>(this Option<TInValue> option, Func<TInValue, TOutValue> mapper)
        {
            Contract.Requires(option != null);
            Contract.Requires(mapper != null);

            if (option.Exists())
            {
                var some = (Option<TInValue>.Some)option;
                var result = mapper(some.Value);

                return Option.OfValue(result);
            }
            else
            {
                return Option.None<TOutValue>();
            }
        }

        /// <summary>
        /// Defaults an option to the supplied value, when None.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="option">The option.</param>
        /// <param name="value">The value.</param>
        /// <returns>The value</returns>
        public static TValue WithDefault<TValue>(this Option<TValue> option, TValue value)
        {
            Contract.Requires(option != null);
            Contract.Requires(value != null);

            if (option.Exists())
            {
                return (option as Option<TValue>.Some).Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Evaluates the callback when the option is Some.
        /// Use for chaining multiple options together.
        /// </summary>
        /// <typeparam name="TInValue">The type of the in value.</typeparam>
        /// <typeparam name="TOutValue">The type of the out value.</typeparam>
        /// <param name="option">The option.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>An option of TOutValue.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
            Justification = "Validated by code contracts")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design warrants it")]
        public static Option<TOutValue> AndThen<TInValue, TOutValue>(
            this Option<TInValue> option, Func<TInValue, Option<TOutValue>> callback)
        {
            Contract.Requires(option != null);
            Contract.Requires(callback != null);

            if (option.Exists())
            {
                var value = (option as Option<TInValue>.Some).Value;
                return callback(value);
            }
            else
            {
                return Option.None<TOutValue>();
            }
        }

        /// <summary>
        /// Evaluates the callback when the option is Some.
        /// Use for chaining multiple options together.
        /// This overload is intended for use with TryParse style functions.
        /// </summary>
        /// <typeparam name="TInValue">The type of the in value.</typeparam>
        /// <typeparam name="TOutValue">The type of the out value.</typeparam>
        /// <param name="option">The option.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>An option of TOutValue.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
            Justification = "Validated by code contracts")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design warrants it")]
        public static Option<TOutValue> AndThen<TInValue, TOutValue>(
            this Option<TInValue> option, Func<TInValue, OutputCollector<TOutValue>, bool> callback)
        {
            Contract.Requires(option != null);
            Contract.Requires(callback != null);

            if (option.Exists())
            {
                var collector = new OutputCollector<TOutValue>();
                var value = (option as Option<TInValue>.Some).Value;

                if (callback(value, collector))
                {
                    return Option.OfValue(collector.Value);
                }
                else
                {
                    return Option.None<TOutValue>();
                }
            }
            else
            {
                return Option.None<TOutValue>();
            }
        }

        /// <summary>
        /// Substitutes for the option value using the substitution functions..
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TOutcome">The type of the outcome.</typeparam>
        /// <param name="option">The option.</param>
        /// <param name="forNone">Substitution function for none.</param>
        /// <param name="forSome">Substitution function for some.</param>
        /// <returns>The outcome.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
            Justification = "Validated by code contracts")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2",
            Justification = "Validated by code contracts")]
        public static TOutcome Substitute<TValue, TOutcome>(
            this Option<TValue> option,
            Func<Unit, TOutcome> forNone,
            Func<TValue, TOutcome> forSome)
        {
            Contract.Requires(option != null);
            Contract.Requires(forNone != null);
            Contract.Requires(forSome != null);

            if (option.Exists())
            {
                var some = (Option<TValue>.Some)option;

                return forSome(some.Value);
            }
            else
            {
                return forNone(new Unit());
            }
        }

        /// <summary>
        /// Converts an <c>Option</c> to a <c>Result</c>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="option">The option.</param>
        /// <returns>The Result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit, TValue> ToResult<TValue>(this Option<TValue> option)
        {
            Contract.Requires(option != null);

            return option.Substitute<TValue, Result<Unit, TValue>>(
                x => new Result<Unit, TValue>.Error(new Unit()),
                y => new Result<Unit, TValue>.Success(y));
        }

        /// <summary>
        /// Converts an <c>Option</c> to a <c>Nullable</c>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="option">The option.</param>
        /// <returns>A TValue?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? ToNullable<TValue>(this Option<TValue> option)
            where TValue : struct
        {
            Contract.Requires(option != null);

            return option.Substitute(x => new Nullable<TValue>(), y => new Nullable<TValue>(y));
        }
    }
}
