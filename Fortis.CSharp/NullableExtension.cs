//-----------------------------------------------------------------------
// <copyright file="NullableExtension.cs" company="Open Source">
//     Defensive coding libaries
//     Code defensively
// </copyright>
// <author>Huw Simpson</author>
//-----------------------------------------------------------------------

namespace Fortis.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Nullable Extension Methods
    /// </summary>
    public static class NullableExtension
    {
        /// <summary>
        /// Substitutes for the nullable value using the substitution functions.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TOutcome">The type of the outcome.</typeparam>
        /// <param name="nullable">The nullable.</param>
        /// <param name="forNull">For null.</param>
        /// <param name="forValue">For value.</param>
        /// <returns>The outcome</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
            Justification = "Validated by code contracts"),
         SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2",
            Justification = "Validated by code contracts")]
        public static TOutcome Substitute<TValue, TOutcome>(
            this TValue? nullable,
            Func<Unit, TOutcome> forNull,
            Func<TValue, TOutcome> forValue)
            where TValue : struct
        {
            Contract.Requires(forNull != null);
            Contract.Requires(forValue != null);

            if (nullable.HasValue)
            {
                return forValue(nullable.Value);
            }
            else
            {
                return forNull(new Unit());
            }
        }

        /// <summary>
        /// Convert a nullable to an option.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="nullable">The nullable value.</param>
        /// <returns>An option of TValue</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TValue> ToOption<TValue>(this TValue? nullable)
            where TValue : struct
        {
            return nullable.Substitute(x => Option.None<TValue>(), y => Option.OfValue(nullable.Value));
        }
    }
}
