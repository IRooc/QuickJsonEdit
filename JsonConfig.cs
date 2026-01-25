using System.Text.Json;
using System.Text.Json.Nodes;

namespace QuickJsonEdit
{
    public class JsonConfig
    {
        private Dictionary<string, JsonNode> _fileCache = new Dictionary<string, JsonNode>();

        public JsonNode GetJson(string filePath)
        {
            if (!_fileCache.ContainsKey(filePath)) LoadJson(filePath);
            return _fileCache[filePath];
        }

        public void LoadJson(string filePath, bool forceReload = false)
        {
            if (!forceReload && _fileCache.ContainsKey(filePath)) {
                Console.WriteLine($"File already loaded: {filePath}");
                return;
            }

            if (!File.Exists(filePath)) {
                _fileCache[filePath] = JsonNode.Parse("{}")!;
                return;
            }

            try {
                using var stream = File.OpenRead(filePath);
                using var reader = new StreamReader(stream);
                string fileBody = reader.ReadToEnd();
                _fileCache[filePath] = JsonNode.Parse(fileBody ?? "{}")!;
            } catch(Exception ex) {
                Console.WriteLine($"Failed to parse body of : {filePath} because {ex.Message}");
                _fileCache[filePath] = JsonNode.Parse("{}")!;
            }
        }
    }
}