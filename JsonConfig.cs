using System.Text.Json;
using System.Text.Json.Nodes;

namespace QuickJsonEdit
{
    public class JsonConfig
    {
        public string JsonFilename {get; set;}
        public JsonNode JsonDocument { get; set; }

        public void SetJson(string f)
        {
            JsonFilename = f;
            using var s = File.OpenRead(f);
            using var reader = new StreamReader(s);
            string fileBody = reader.ReadToEnd();
            JsonDocument = JsonNode.Parse(fileBody ?? "{}")!;

        }
    }
}