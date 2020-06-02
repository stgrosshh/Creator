using System;

namespace Innoactive.Creator.Core
{
    /// <summary>
    /// Event that fired when the current stage changes.
    /// </summary>
    public class ActivationStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// New stage.
        /// </summary>
        public readonly Stage Stage;

        public ActivationStateChangedEventArgs(Stage stage)
        {
            Stage = stage;
        }
    }
}
