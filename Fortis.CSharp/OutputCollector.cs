//-----------------------------------------------------------------------
// <copyright file="OutputCollector.cs" company="Open Source">
//     Defensive coding libaries
//     Code defensively
// </copyright>
// <author>Huw Simpson</author>
//-----------------------------------------------------------------------

namespace Fortis.CSharp
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Collects out values within lambda functions.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public class OutputCollector<T>
    {
        /// <summary>
        /// The value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields",
            Justification = "The design warrants it")]
        public T Value = default(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCollector{T}"/> class.
        /// </summary>
        internal OutputCollector()
        {
        }
    }
}
