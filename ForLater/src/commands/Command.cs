using System.Collections.Generic;

namespace ForLater.commands
{
    /// <summary>
    /// Represents an abstract class for a command for better abstraction.
    /// </summary>
    abstract class Command
    {
        private protected Repository repository;
        private protected List<Item> items = new List<Item>();

        public Command(Repository repository)
        {
            this.repository = repository;
            repository.ParseFiles(items);
        }

        public abstract void Execute();
    }
}
