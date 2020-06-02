using System;

namespace Innoactive.Creator.Core.Attributes
{
    /// <summary>
    /// Declares the drawing order for this element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DrawingPriorityAttribute : Attribute
    {
        /// <summary>
        /// Lower goes first.
        /// </summary>
        public int Priority { get; }

        public DrawingPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
