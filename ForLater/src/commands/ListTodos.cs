using System;
using System.Collections.Generic;
using System.Text;

namespace ForLater.commands {
    class ListTodos : Command {
        public ListTodos(Repository repository) : base(repository) { }

        public override void Execute() {
            repository.ParseFiles(items);

            foreach (var item in items) {
                Console.WriteLine($"*  {item}");
            }
        }
    }
}
