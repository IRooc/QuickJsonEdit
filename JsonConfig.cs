using System.Text.Json;
using System.Text.Json.Nodes;

namespace QuickJsonEdit
{
    public class JsonConfig
    {
        private Dictionary<string, JsonNode> _openFiles = new Dictionary<string, JsonNode>();

        public JsonNode GetJson(string f)
        {
            if (!_openFiles.ContainsKey(f)) LoadJson(f);
            return _openFiles[f];
        }

        public void LoadJson(string f, bool forceReload = false)
        {
            if (!forceReload && _openFiles.ContainsKey(f)) return;

            using var s = File.OpenRead(f);
            using var reader = new StreamReader(s);
            string fileBody = reader.ReadToEnd();
            try {
                _openFiles[f] = JsonNode.Parse(fileBody ?? "{}")!;
            } catch {
                _openFiles[f] = JsonNode.Parse("{}")!;
            }
        }
    }
}