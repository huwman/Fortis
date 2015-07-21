//-----------------------------------------------------------------------
// <copyright file="Result.cs" company="Open Source">
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

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Option")]
    public static class Result
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Result<TError, TValue>.Error Error<TError, TValue>(TError value)
        {
            Contract.Requires(value != null);

            return new Result<TError, TValue>.Error(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Result<TError, TValue>.Success Success<TError, TValue>(TValue value)
        {
            Contract.Requires(value != null);

            return new Result<TError, TValue>.Success(value);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"),
         SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static Result<Exception,TValue> Guard<TValue>(Func<TValue> callback)
        {
            Contract.Requires(callback != null);

            try
            {
                var value = callback();
                if (value == null)
                {
                    return Result.Error<Exception, TValue>(
                        new InvalidOperationException("callback returned null"));
                }
                else 
                {
                    return Result.Success<Exception, TValue>(value);
                }
            }
            catch (Exception ex)
            {
                return Result.Error<Exception, TValue>(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<Unit, Unit> OfBool(bool value)
        {
            if (value)
            {
                return new Result<Unit, Unit>.Success(new Unit());
            }
            else
            {
                return new Result<Unit, Unit>.Error(new Unit());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TError> Errors<TError, TValue>(Result<TError, TValue> result)
        {
            return result.Substitute(Option.OfValue, y => Option.None<TError>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> Successes<TError, TValue>(Result<TError, TValue> result)
        {
            return result.Substitute(x => Option.None<TValue>(), Option.OfValue);
        }
    }

    public abstract class Result<TError, TValue>
    {
        public override int GetHashCode()
        {
            var success = this as Success;
            var error = this as Error;
            Contract.Assume(success != null || error != null);

            return ((object)success == null) ? error.Value.GetHashCode() : success.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Result<TError, TValue>)
            {
                var thisError = this as Error;
                var otherError = obj as Error;
                var thisSuccess = this as Success;
                var otherSuccess = obj as Success;

                if ((object)thisError != null && (object)otherError != null)
                {
                    return object.Equals(thisError.Value, otherError.Value);
                }
                else if ((object)thisSuccess != null && (object)otherSuccess != null)
                {
                    return object.Equals(thisSuccess.Value, otherSuccess.Value);
                }
            }

            return false;
        }

        public static bool operator ==(Result<TError, TValue> resultA, Result<TError, TValue> resultB)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(resultA, resultB))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)resultA == null) || ((object)resultB == null))
            {
                return false;
            }

            // Return true if the fields match:
            return resultA.Equals(resultB);
        }

        public static bool operator !=(Result<TError, TValue> resultA, Result<TError, TValue> resultB)
        {
            return !(resultA == resultB);
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"),
         SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")]
        public sealed class Error : Result<TError, TValue>
        {
            public TError Value
            {
                get;
                private set;
            }

            public Error(TError value)
            {
                Contract.Requires(value != null);

                this.Value = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class Success : Result<TError, TValue>
        {
            public TValue Value
            {
                get;
                private set;
            }

            public Success(TValue value)
            {
                Contract.Requires(value != null);

                this.Value = value;
            }
        }
    }
}
