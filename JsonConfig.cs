using System.Text.Json;
using System.Text.Json.Nodes;

namespace QuickJsonEdit
{
    public class JsonConfig
    {
        public string JsonFilename {get; set;}
        public JsonNode JsonDocument { get; set; }

        public void SetJson(string f, Stream fileContent)
        {
            JsonFilename = f;

            var reader = new StreamReader(fileContent);
            string fileBody = reader.ReadToEnd();
            JsonDocument = JsonNode.Parse(fileBody);

        }
    }
}