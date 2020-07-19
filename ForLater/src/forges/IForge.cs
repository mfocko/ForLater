using System;

namespace ForLater.forges
{
    /// <summary>
    /// Specifies an interface that needs to be implemented by forges, e.g. GitHub or GitLab.
    /// </summary>
    interface IForge
    {
        /// <summary>
        /// Holds string containing host of forge, e.g. self-hosted GitLab instance.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Returns an issue object from git forge.
        /// </summary>
        /// <param name="todo">To-Do object of already reported item, which holds issue id</param>
        public void GetIssue(Item todo);

        /// <summary>
        /// Creates a new issue.
        /// </summary>
        /// <param name="todo">To-Do object to be reported</param>
        /// <param name="description">Body of an issue (if has any)</param>
        public void PostIssue(Item todo, string description);

        /// <summary>
        /// Method that is used to acquire a forge object.
        /// Acquires token from config file and returns an object that holds that token.
        /// </summary>
        /// <returns>Instance of forge object that holds authentication token</returns>
        public IForge? Get();
    }
}
