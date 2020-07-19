using System;
using System.Collections.Generic;
using System.Text;

namespace ForLater.commands
{
    class CleanTodos : Command
    {
        public CleanTodos(Repository repository) : base(repository) { }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
