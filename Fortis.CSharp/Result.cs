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

    /// <summary>
    /// Result type helper functions.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Option")]
    public static class Result
    {
        /// <summary>
        /// Error, represents a error result.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A result of {TError, TValue}.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Result<TError, TValue>.Error Error<TError, TValue>(TError value)
        {
            Contract.Requires(value != null);

            return new Result<TError, TValue>.Error(value);
        }

        /// <summary>
        /// Success, represents a success result.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A result of {TError, TValue}.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Result<TError, TValue>.Success Success<TError, TValue>(TValue value)
        {
            Contract.Requires(value != null);

            return new Result<TError, TValue>.Success(value);
        }

        /// <summary>
        /// Guards the specified callback.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="callback">The callback.</param>
        /// <returns>A result of {Exception, TValue}.</returns>
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

        /// <summary>
        /// Create a result from a boolean value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <returns>Success of {Unit, Unit} when value <c>true</c>, otherwise Error of {Unit, Unit}.</returns>
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

        /// <summary>
        /// Filters for error results.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>An Option of TError.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TError> Errors<TError, TValue>(Result<TError, TValue> result)
        {
            Contract.Requires(result != null);

            return result.Substitute(Option.OfValue, y => Option.None<TError>());
        }

        /// <summary>
        /// Filters for success results.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>An option of TValue.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> Successes<TError, TValue>(Result<TError, TValue> result)
        {
            Contract.Requires(result != null);
            
            return result.Substitute(x => Option.None<TValue>(), Option.OfValue);
        }
    }

    /// <summary>
    /// Result type.
    /// Use as a type-safe replacement for returning nulls or throwing exceptions.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public abstract class Result<TError, TValue>
    {
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var success = this as Success;
            var error = this as Error;
            Contract.Assume(success != null || error != null);

            return ((object)success == null) ? error.Value.GetHashCode() : success.Value.GetHashCode();
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

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="resultA">The result a.</param>
        /// <param name="resultB">The result b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
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

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="resultA">The result a.</param>
        /// <param name="resultB">The result b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Result<TError, TValue> resultA, Result<TError, TValue> resultB)
        {
            return !(resultA == resultB);
        }

        /// <summary>
        /// An Error Result.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"),
         SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")]
        public sealed class Error : Result<TError, TValue>
        {
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public TError Value
            {
                get;
                private set;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Error"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Error(TError value)
            {
                Contract.Requires(value != null);

                this.Value = value;
            }
        }

        /// <summary>
        /// A Success Result.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class Success : Result<TError, TValue>
        {
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

            /// <summary>
            /// Initializes a new instance of the <see cref="Success"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Success(TValue value)
            {
                Contract.Requires(value != null);

                this.Value = value;
            }
        }
    }
}
