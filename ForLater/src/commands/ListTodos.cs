using System;

namespace ForLater.commands
{
    class ListTodos : Command
    {
        public ListTodos(Repository repository) : base(repository) { }

        public override void Execute()
        {
            foreach (var item in items)
            {
                Console.WriteLine($"*  {item}");
            }
        }
    }
}
