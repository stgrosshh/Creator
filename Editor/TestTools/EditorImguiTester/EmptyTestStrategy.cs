using Innoactive.Creator.Core;
using Innoactive.CreatorEditor.UI.Windows;

namespace Innoactive.CreatorEditor.TestTools
{
    /// <summary>
    /// An editing strategy that does nothing. Use it to isolate windows logic during testing.
    /// </summary>
    internal class EmptyTestStrategy : IEditingStrategy
    {
        public ICourse CurrentCourse { get; }

        /// <inheritdoc/>
        public void HandleNewCourseWindow(CourseWindow window)
        {
        }

        /// <inheritdoc/>
        public void HandleNewStepWindow(StepWindow window)
        {
        }

        /// <inheritdoc/>
        public void HandleCurrentCourseModified()
        {
        }

        /// <inheritdoc/>
        public void HandleCourseWindowClosed(CourseWindow window)
        {
        }

        /// <inheritdoc/>
        public void HandleStepWindowClosed(StepWindow window)
        {
        }

        /// <inheritdoc/>
        public void HandleStartEditingCourse()
        {
        }

        /// <inheritdoc/>
        public void HandleCurrentCourseChanged(string courseName)
        {
        }

        /// <inheritdoc/>
        public void HandleCurrentStepModified(IStep step)
        {
        }

        /// <inheritdoc/>
        public void HandleStartEditingStep()
        {
        }

        /// <inheritdoc/>
        public void HandleCurrentStepChanged(IStep step)
        {
        }

        /// <inheritdoc/>
        public void HandleProjectIsGoingToUnload()
        {
        }

        /// <inheritdoc/>
        public void HandleProjectIsGoingToSave()
        {
        }

        /// <inheritdoc/>
        public void HandleExitingPlayMode()
        {
        }

        /// <inheritdoc/>
        public void HandleEnterPlayMode()
        {
        }
    }
}
