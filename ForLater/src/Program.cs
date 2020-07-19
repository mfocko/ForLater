using ForLater.commands;

using System;
using System.IO;

namespace ForLater {
    class Program {

        /// <summary>
        /// Recursively searches for the root of a git repository.
        /// </summary>
        /// <returns>Path to the root of the git repository if found, otherwise null</returns>
        private static string? FindGitInfo() {
            string? path = Environment.CurrentDirectory;

            while (!string.IsNullOrEmpty(path)) {
                if (Directory.Exists(Path.Join(path, ".git"))) {
                    return path;
                }
                path = Path.GetDirectoryName(path);
            }

            return null;
        }

        private static void PrintUsage() {
            Console.WriteLine("forLater [command]");
            Console.WriteLine("Commands:");
            Console.WriteLine("\tlist\tlists all found TODOs");
            Console.WriteLine("\tcreate\tcreates all found unprocessed TODOs");
            Console.WriteLine("\tclean\tremoves all finished TODOs");
        }

        public static void Main(string[] args) {
            string? gitRoot = FindGitInfo();
            if (gitRoot == null) {
                Console.Error.WriteLine("Couldn't find root of git repository.");
                Environment.Exit(1);
            }
            Console.WriteLine($"Found git repository: {gitRoot}");

            Repository repository = new Repository(gitRoot);
            if (args.Length < 1) {
                PrintUsage();
                Environment.Exit(0);
            }

            Command? command = default;
            switch (args[0]) {
                case "list":
                    command = new ListTodos(repository);
                    break;
                case "create":
                    command = new CreateTodos(repository);
                    break;
                case "clean":
                    command = new CleanTodos(repository);
                    break;
            }
            if (command == null) {
                PrintUsage();
                Environment.Exit(0);
            }
            command.Execute();
        }

        // Debug functions
        private static void PrintTrackedFiles(string path) {
            Console.WriteLine($"Tracked files in {path}: ");
            foreach (var file in Repository.ListFiles(path)) {
                Console.WriteLine(file);
            }
        }
    }
}
