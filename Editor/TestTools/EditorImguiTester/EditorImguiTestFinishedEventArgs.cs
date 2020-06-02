using System;

namespace Innoactive.CreatorEditor.TestTools
{
    /// <summary>
    /// Event args for event which is fired when a <see cref="IEditorImguiTest"/> test finishes it's execution.
    /// </summary>
    internal class EditorImguiTestFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Result from last <see cref="IEditorImguiTest"/>.
        /// </summary>
        public TestState Result { get; }

        public EditorImguiTestFinishedEventArgs(TestState result)
        {
            Result = result;
        }
    }
}
