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

    public static class ResultExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsError<TError, TValue>(this Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return (result is Result<TError, TValue>.Error) == true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccess<TError, TValue>(this Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return (result is Result<TError, TValue>.Success) == true;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2"),
         SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> ToOption<TError, TValue>(this Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return result.Substitute(e => Option.None<TValue>(), v => Option.OfValue(v));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue? Nullable<TError, TValue>(this Result<TError, TValue> result)
            where TValue : struct
        {
            Contract.Requires(result != null);

            return result.Substitute(x => new Nullable<TValue>(), y => new Nullable<TValue>(y));
        }
    }
}
