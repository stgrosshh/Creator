using Innoactive.Hub.Config;

namespace Innoactive.Hub.Training.Configuration
{
    /// <summary>
    /// Configuration that determines which entity can log messages in the entity state logger.
    /// </summary>
    public class EntityStateLoggerConfig : ConfigBase
    {
        /// <inheritdoc />
        public override string DirectoryPath
        {
            get
            {
                return "./Config";
            }
        }

        /// <summary>
        /// True, if behaviors are allowed to be logged.
        /// </summary>
        public bool LogBehaviors = false;

        /// <summary>
        /// True, if conditions are allowed to be logged.
        /// </summary>
        public bool LogConditions = false;

        /// <summary>
        /// True, if chapters are allowed to be logged.
        /// </summary>
        public bool LogChapters = false;

        /// <summary>
        /// True, if steps are allowed to be logged.
        /// </summary>
        public bool LogSteps = false;

        /// <summary>
        /// True, if transitions are allowed to be logged.
        /// </summary>
        public bool LogTransitions = false;
    }
}