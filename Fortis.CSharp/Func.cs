//-----------------------------------------------------------------------
// <copyright file="Func.cs" company="Open Source">
//     Defensive coding libaries
//     Code defensively
// </copyright>
// <author>Huw Simpson</author>
//-----------------------------------------------------------------------

namespace Fortis.CSharp
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Helpers and extensions for functions.
    /// </summary>
    public static class Func
    {
        /// <summary>
        /// Identity function.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The input value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Identity<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Ignores the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A Unit, placeholder value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value",
            Justification = "We explicitely are ignoring the value parameter")]
        public static Unit Ignore(object value)
        {
            return new Unit();
        }

        /// <summary>
        /// Curries the specified function.
        /// </summary>
        /// <typeparam name="TFirstArg">The type of the first argument.</typeparam>
        /// <typeparam name="TSecondArg">The type of the second argument.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>A curried function.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design requires it")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TFirstArg, Func<TSecondArg, TReturn>> Curry<TFirstArg, TSecondArg, TReturn>(
            this Func<TFirstArg, TSecondArg, TReturn> function)
        {
            return
                (t1) =>
                (t2) => function(t1, t2);
        }

        /// <summary>
        /// Curries the specified function.
        /// </summary>
        /// <typeparam name="TFirstArg">The type of the first argument.</typeparam>
        /// <typeparam name="TSecondArg">The type of the second argument.</typeparam>
        /// <typeparam name="TThirdArg">The type of the third argument.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>A curried function</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design requires it")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TFirstArg, Func<TSecondArg, Func<TThirdArg, TReturn>>>
            Curry<TFirstArg, TSecondArg, TThirdArg, TReturn>(
                this Func<TFirstArg, TSecondArg, TThirdArg, TReturn> function)
        {
            return
                (t1) =>
                (t2) =>
                (t3) => function(t1, t2, t3);
        }

        /// <summary>
        /// Curries the specified function.
        /// </summary>
        /// <typeparam name="TFirstArg">The type of the first argument.</typeparam>
        /// <typeparam name="TSecondArg">The type of the second argument.</typeparam>
        /// <typeparam name="TThirdArg">The type of the third argument.</typeparam>
        /// <typeparam name="TFourthArg">The type of the fourth argument.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>A curried function</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design requires it")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TFirstArg, Func<TSecondArg, Func<TThirdArg, Func<TFourthArg, TReturn>>>>
            Curry<TFirstArg, TSecondArg, TThirdArg, TFourthArg, TReturn>(
                this Func<TFirstArg, TSecondArg, TThirdArg, TFourthArg, TReturn> function)
        {
            return
                (t1) =>
                (t2) =>
                (t3) =>
                (t4) => function(t1, t2, t3, t4);
        }

        /// <summary>
        /// Curries the specified function.
        /// </summary>
        /// <typeparam name="TFirstArg">The type of the first argument.</typeparam>
        /// <typeparam name="TSecondArg">The type of the second argument.</typeparam>
        /// <typeparam name="TThirdArg">The type of the third argument.</typeparam>
        /// <typeparam name="TFourthArg">The type of the fourth argument.</typeparam>
        /// <typeparam name="TFifthArg">The type of the fifth argument.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>A curried function</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design requires it")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TFirstArg, Func<TSecondArg, Func<TThirdArg, Func<TFourthArg, Func<TFifthArg, TReturn>>>>>
            Curry<TFirstArg, TSecondArg, TThirdArg, TFourthArg, TFifthArg, TReturn>(
                this Func<TFirstArg, TSecondArg, TThirdArg, TFourthArg, TFifthArg, TReturn> function)
        {
            return
                (t1) =>
                (t2) =>
                (t3) =>
                (t4) =>
                (t5) => function(t1, t2, t3, t4, t5);
        }
    }
}
