namespace ForLater
{
    class Item
    {
        /// <summary>
        /// Holds a prefix of an item, e.g. `// `
        /// </summary>
        public string Prefix { get; set; } = "";

        /// <summary>
        /// Holds a keyword that is registered.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Keeps ID of issue or index in markdown list of todos.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Keeps filepath from root of a git repository.
        /// </summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// Keeps line number on which the todo has been located.
        /// </summary>
        public uint Line { get; set; } = 0;

        /// <summary>
        /// Title of a TODO (text that follows tag).
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Following lines with the same prefix as line with title.
        /// </summary>
        public string Body { get; set; } = "";

        /// <summary>
        /// True if issue is closed or item in markdown list is marked as done.
        /// </summary>
        public bool Finished { get; set; } = false;

        /// <summary>
        /// Provides pretty-printing of ID for string methods.
        /// </summary>
        public string StringID => (ID == 0) ? "" : $"({ID})";

        /// <summary>
        /// Provides pretty-printing of body.
        /// </summary>
        public string BodyInLines => (!string.IsNullOrEmpty(Body)) ? $"\n{Body.Trim()}" : Body;

        public override string ToString()
        {
            return $"{FilePath}@{Line}: {StringID}{Keyword}: {Title}{BodyInLines.Replace("\n", "\n   ")}";
        }

        public string MarkdownString()
        {
            var firstLine = $"- [{(Finished ? 'x' : ' ')}] {FilePath}@{Line}<br>\n      ";
            var secondLine = $"{StringID}<strong>{Title}</strong>";
            var body = $"{BodyInLines.Replace("\n", "<br>\n      ")}";
            return $"{firstLine}{secondLine}{body}";
        }
    }
}
