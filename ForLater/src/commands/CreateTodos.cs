using System;
using System.IO;
using System.Linq;

namespace ForLater.commands
{
    class CreateTodos : Command
    {
        public CreateTodos(Repository repository) : base(repository) { }

        public override void Execute()
        {
            // LoadMarkdown();
            SaveMarkdown();
        }

        /// <summary>
        /// Generates a TODO.md file with found TODOs.
        /// </summary>
        private void SaveMarkdown()
        {
            using var output = new StreamWriter(Path.Join(repository.Root, "TODO.md"));

            output.WriteLine("# TODO List\n");

            foreach (var keyword in items.GroupBy(item => item.Keyword))
            {
                output.WriteLine($"## {keyword.Key}\n");

                foreach (var item in keyword)
                {
                    output.WriteLine($"{item.MarkdownString()}\n");
                }
            }
        }

        /// <summary>
        /// Loads tracked items from TODO.md in the root of repository.
        /// </summary>
        private void LoadMarkdown()
        {
            // TODO: This didn't go as I expected...
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to update source code with ID to tracked todo item.
        /// </summary>
        /// <param name="item">Item containing necessary information.</param>
        private static void ReplaceInCode(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
