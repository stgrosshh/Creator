using System.Collections.Generic;

namespace Innoactive.CreatorEditor.Tabs
{
    /// <summary>
    /// Draws a view with multiple tabs.
    /// </summary>
    internal interface ITabsGroup
    {
        /// <summary>
        /// Index of the currently selected tab.
        /// </summary>
        int Selected { get; set; }

        /// <summary>
        /// Tabs to display. See <seealso cref="ITab"/>.
        /// </summary>
        IList<ITab> Tabs { get; }
    }
}
