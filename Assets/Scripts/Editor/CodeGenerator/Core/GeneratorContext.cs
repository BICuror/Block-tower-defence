using System.Collections.Generic;

namespace CuroCodeGen
{
    public sealed class GeneratorContext
    {
        private List<CodeText> _codeList = new List<CodeText>();
        public IReadOnlyList<CodeText> codeList => _codeList;

        private string _folderPath = null;
        public string FolderPath => _folderPath;

        public void AddCode(string fileName, string text)
        {
            _codeList.Add(new CodeText() { FileName = fileName, Text = text });
        }

        public void SetFolderPath(string path)
        {
            _folderPath = path;
        }
    }
}