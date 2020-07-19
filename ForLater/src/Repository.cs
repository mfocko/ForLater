using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace ForLater
{
    class Repository
    {
        /// <summary>
        /// Representation of a keyword.
        /// Stores keyword and compiled regular expressions for matching in sources.
        /// </summary>
        private struct Keyword
        {
            public string keyword;

            /// <summary>
            /// Used to match a TODO that is not tracked in markdown or as issue.
            /// </summary>
            public Regex newTodo;

            /// <summary>
            /// Used to match a TODO that is tracked in markdown or as issue.
            /// </summary>
            public Regex oldTodo;

            public Keyword(string keyword)
            {
                this.keyword = keyword;
                newTodo = new Regex($"^(.*){keyword}: (.*)$");
                oldTodo = new Regex($"^(.*)\\((.*)\\){keyword}: (.*)$");
            }
        }

        /// <summary>
        /// Holds absolute path to the root of a repository.
        /// </summary>
        public string Root { get; }
        private readonly List<Keyword> Keywords = new List<Keyword>();
        private readonly List<string> Files;

        /// <summary>
        /// Loads and compiles regular expressions for tracked keywords specified in configuration file.
        /// 
        /// If no keywords or config file is found, automatically creates a default TODO keyword.
        /// </summary>
        private void LoadKeywords()
        {
            var configFilePath = Path.Join(Root, ".forlater.yml");
            if (!File.Exists(configFilePath))
            {
                Keywords.Add(new Keyword("TODO"));
                return;
            }

            using var configFile = new StreamReader(configFilePath);

            var yamlFile = new YamlStream();
            yamlFile.Load(configFile);

            var rootNode = (YamlMappingNode)yamlFile.Documents[0].RootNode;
            var keywords = (YamlSequenceNode)rootNode.Children[new YamlScalarNode("keywords")];

            foreach (YamlScalarNode keyword in keywords)
            {
                if (keyword.Value != null)
                {
                    Keywords.Add(new Keyword(keyword.Value));
                }
            }

            if (Keywords.Count < 1)
            {
                // handles no explicit keywords
                Keywords.Add(new Keyword("TODO"));
            }
        }

        /// <summary>
        /// Loads tracked keywords from configuration file and creates a list
        /// of files that are to be checked later.
        /// </summary>
        /// <param name="path">Absolute path to the repository</param>
        public Repository(string path)
        {
            Root = path;
            LoadKeywords();
            Files = ListFiles(path);
        }

        /// <summary>
        /// Checks line from source code against one of tracked keywords.
        /// </summary>
        /// <param name="line">line of source that is to be checked</param>
        /// <param name="item">output parameter that is set accordingly to result</param>
        /// <returns>True if todo is found, False otherwise</returns>
        private bool CheckTodo(string? line, out Item? item)
        {
            item = null;
            if (line == null)
            {
                return false;
            }

            foreach (var keyword in Keywords)
            {
                // need to check already processed
                var matched = keyword.oldTodo.Match(line);
                if (matched.Success)
                {
                    item = new Item
                    {
                        Prefix = matched.Groups[1].Value,
                        ID = int.Parse(matched.Groups[2].Value),
                        Title = matched.Groups[3].Value,
                        Keyword = keyword.keyword
                    };
                    return true;
                }

                // need to check new TODO
                matched = keyword.newTodo.Match(line);
                if (matched.Success)
                {
                    item = new Item
                    {
                        Prefix = matched.Groups[1].Value,
                        Title = matched.Groups[2].Value,
                        Keyword = keyword.keyword
                    };
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if line has same prefix as last processed todo,
        /// if does, adds the text to the body of a todo.
        /// </summary>
        /// <param name="line">line of text from source code</param>
        /// <param name="item">last processed item/todo</param>
        /// <returns>True if body was extended, False otherwise</returns>
        private bool CheckBody(string? line, Item item)
        {
            if (line != null && line.StartsWith(item.Prefix))
            {
                item.Body += line.Substring(item.Prefix.Length) + "\n";
                return true;
            }
            return false;
        }

        /// <summary>
        /// Asynchronously parses file.
        /// </summary>
        /// <param name="filepath">Filepath to the file that is parsed</param>
        /// <param name="items">List of items where todos are added</param>
        /// <returns>Action that is executed asynchronously</returns>
        private Action ParseFile(string filepath, List<Item> items)
        {
            return () =>
            {
                if (Directory.Exists(filepath))
                {
                    Console.WriteLine($"Skipping directory: {filepath}");
                    return;
                }
                else if (filepath.ToLower().EndsWith("todo.md") || filepath.ToLower().EndsWith(".forlater.yml"))
                {
                    Console.WriteLine($"Skipping list of TODOs or configuration file: {filepath}");
                    return;
                }

                using var source = new StreamReader(filepath);

                Item? item = null;
                uint linenum = 0;
                while (!source.EndOfStream)
                {
                    var line = source.ReadLine();
                    linenum++;

                    if (item == null)
                    {
                        // Need to check for a new TODO
                        if (CheckTodo(line, out var todo) && todo != null)
                        {
                            todo.FilePath = filepath.Substring(filepath.LastIndexOf('\\') + 1);
                            todo.Line = linenum;

                            item = todo;
                        }
                    }
                    else if (CheckTodo(line, out var todo) && todo != null)
                    {
                        // Need to check for a new TODO right under previous
                        items.Add(item);

                        todo.FilePath = filepath.Substring(filepath.LastIndexOf('\\') + 1);
                        todo.Line = linenum;

                        item = todo;
                    }
                    else if (CheckBody(line, item))
                    {
                        // Need to check for a body of TODO
                        /* no-op */
                    }
                    else
                    {
                        // Need to check if we're looking for next TODO
                        items.Add(item);
                        item = null;
                    }
                }

                if (item != null)
                {
                    items.Add(item);
                }
            };
        }

        /// <summary>
        /// Parses all files tracked by git.
        /// Ignores directories, configuration file of this tool and list
        /// of TODOs if stored as markdown.
        /// </summary>
        /// <param name="items">List of items that is expanded</param>
        public void ParseFiles(List<Item> items)
        {
            var tasks = new List<Task>();
            foreach (var filename in Files)
            {
                tasks.Add(Task.Run(ParseFile(Path.Join(Root, filename), items)));
            }

            try
            {
                // FIXME: doesn't seem to wait for all every time
                // fixed I guess... can't use async read in ľambda :pepeHands:
                // how to search: Task.WaitAll doesn't wait for all...
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                Console.Error.WriteLine(e);
                Environment.Exit(2);
            }
        }

        /// <summary>
        /// Gets all the files tracked by git.
        /// </summary>
        /// <param name="path">Path to the root of git repository</param>
        /// <returns>List of all files</returns>
        public static List<string> ListFiles(string path)
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "git";
            p.StartInfo.Arguments = "ls-files";
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            var files = p.StandardOutput.ReadToEnd().Split("\n").ToList();
            p.WaitForExit();

            return files;
        }
    }
}
