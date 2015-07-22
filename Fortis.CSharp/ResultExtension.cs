//-----------------------------------------------------------------------
// <copyright file="ResultExtension.cs" company="Open Source">
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
    /// Result Extension Methods
    /// </summary>
    public static class ResultExtension
    {
        /// <summary>
        /// Determines whether this is an instance of error.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A boolean.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsError<TError, TValue>(this Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return (result is Result<TError, TValue>.Error) == true;
        }

        /// <summary>
        /// Determines whether this ia an instance of success.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A boolean.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccess<TError, TValue>(this Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return (result is Result<TError, TValue>.Success) == true;
        }

        /// <summary>
        /// Maps the value, when an instance of Success.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValueIn">The type of the value in.</typeparam>
        /// <typeparam name="TValueOut">The type of the value out.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>A Result of {TError, TValueOut}.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static Result<TError, TValueOut> Map<TError, TValueIn, TValueOut>(
            this Result<TError, TValueIn> result,
            Func<TValueIn, TValueOut> mapper)
        {
            Contract.Requires(result != null);
            Contract.Requires(mapper != null);

            if (result.IsSuccess())
            {
                var success = (Result<TError, TValueIn>.Success)result;
                var value = mapper(success.Value);

                Contract.Assume(value != null);
                return new Result<TError, TValueOut>.Success(value);
            }
            else
            {
                var error = (Result<TError, TValueIn>.Error)result;

                Contract.Assume(error.Value != null);
                return new Result<TError, TValueOut>.Error(error.Value);
            }
        }

        /// <summary>
        /// Formats the error, when an instance of Error.
        /// </summary>
        /// <typeparam name="TErrorIn">The type of the error in.</typeparam>
        /// <typeparam name="TErrorOut">The type of the error out.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>A Result of {TErrorOut, TValue}.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static Result<TErrorOut, TValue> FormatError<TErrorIn, TErrorOut, TValue>(
            this Result<TErrorIn, TValue> result,
            Func<TErrorIn, TErrorOut> mapper)
        {
            Contract.Requires(result != null);
            Contract.Requires(mapper != null);

            if (result.IsError())
            {
                var error = (Result<TErrorIn, TValue>.Error)result;
                var value = mapper(error.Value);

                Contract.Assume(value != null);
                return new Result<TErrorOut, TValue>.Error(value);
            }
            else
            {
                var success = (Result<TErrorIn, TValue>.Success)result;

                Contract.Assume(success.Value != null);
                return new Result<TErrorOut, TValue>.Success(success.Value);
            }
        }

        /// <summary>
        /// Evaluates the callback when the result is Success.
        /// Use for chaining multiple results together.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValueIn">The type of the value in.</typeparam>
        /// <typeparam name="TValueOut">The type of the value out.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>A Result of {TError, TValueOut}.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"),
         SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static Result<TError, TValueOut> AndThen<TError, TValueIn, TValueOut>(
            this Result<TError, TValueIn> result,
            Func<TValueIn, Result<TError, TValueOut>> mapper)
        {
            Contract.Requires(result != null);
            Contract.Requires(mapper != null);

            if (result.IsSuccess())
            {
                var success = (Result<TError, TValueIn>.Success)result;
                var value = mapper(success.Value);

                Contract.Assume(value != null);
                return value;
            }
            else
            {
                var error = (Result<TError, TValueIn>.Error)result;

                Contract.Assume(error.Value != null);
                return new Result<TError, TValueOut>.Error(error.Value);
            }
        }

        /// <summary>
        /// Substitutes for success or error using the substitution functions..
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TOutcome">The type of the outcome.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="forError">For error.</param>
        /// <param name="forSuccess">For success.</param>
        /// <returns>A value of TOutcome.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2"),
         SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOutcome Substitute<TError, TValue, TOutcome>(
            this Result<TError, TValue> result,
            Func<TError, TOutcome> forError,
            Func<TValue, TOutcome> forSuccess)
        {
            Contract.Requires(result != null);
            Contract.Requires(forError != null);
            Contract.Requires(forSuccess != null);

            if (result.IsSuccess())
            {
                var success = (Result<TError, TValue>.Success)result;

                return forSuccess(success.Value);
            }
            else
            {
                var error = (Result<TError, TValue>.Error)result;

                return forError(error.Value);
            }
        }

        /// <summary>
        /// Converts to an option.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A Option of TValue.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> ToOption<TError, TValue>(this Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return result.Substitute(e => Option.None<TValue>(), v => Option.OfValue(v));
        }

        /// <summary>
        /// Converts to a Nullable.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A nullable TValue</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? Nullable<TError, TValue>(this Result<TError, TValue> result)
            where TValue : struct
        {
            Contract.Requires(result != null);

            return result.Substitute(x => new Nullable<TValue>(), y => new Nullable<TValue>(y));
        }
    }
}
