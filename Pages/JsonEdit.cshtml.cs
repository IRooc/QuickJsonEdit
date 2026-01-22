using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace QuickJsonEdit.Pages
{
public class JsonEditModel : PageModel
{
    public void CollectEditablePaths(JsonNode node, string prefix, List<string> paths)
    {
        if (node is JsonObject obj)
        {
            foreach (var kvp in obj)
            {
                var key = kvp.Key;
                var child = kvp.Value;
                string currentPath = string.IsNullOrEmpty(prefix) ? key : prefix + "/" + key;
                if (child is JsonValue val)
                {
                    if (val.GetValueKind() == JsonValueKind.String)
                    {
                        paths.Add(currentPath);
                    }
                }
                else if (child is JsonObject)
                {
                    // recurse
                    CollectEditablePaths(child, currentPath, paths);
                }
                // arrays are skipped
            }
        }
    }

    public JsonNode? GetNodeAtPath(string path)
    {
        var parts = path.Split('/');
        JsonNode? currentNode = Config.JsonDocument;
        foreach (var part in parts)
        {
            if (currentNode is JsonObject obj)
            {
                if (obj.ContainsKey(part))
                {
                    currentNode = obj[part];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return currentNode;
    }

    public bool TryUpdateValueAtPath(string path, string newValue)
    {
        var parts = path.Split('/');
        JsonNode? currentNode = Config.JsonDocument;
        for(int i=0; i < parts.Length - 1; i++)
        {
            var part = parts[i];
            if (currentNode is JsonObject obj)
            {
                if (obj.ContainsKey(part))
                {
                    currentNode = obj[part];
                }
                else
                {
                    // Create new JsonObject if missing intermediate key
                    var newObj = new JsonObject();
                    obj[part] = newObj;
                    currentNode = newObj;
                }
            }
            else
            {
                return false;
            }
        }
        // Update last part
        string lastPart = parts[^1];
        if (currentNode is JsonObject parentObj)
        {
            // overwrite or create new string value
            parentObj[lastPart] = newValue;
            return true;
        }
        return false;
    }


        [BindProperty(SupportsGet = true)]
        public string? Key { get; set; }
        public JsonConfig Config { get; }
        public JsonEditModel(JsonConfig config)
        {
            this.Config = config;
        }

        public void OnGet()
        {
        }

public IActionResult OnPostSaveNewValue()
        {
            var k = Request.Form["newKey"];
            var v = Request.Form["newValue"];

            bool updated = TryUpdateValueAtPath(k, v);
            if (!updated)
            {
                // fallback update root as before
                Config.JsonDocument[k] = v.ToString();
            }
            return RedirectToPage(new { Key = k, Saved = true });
        }

        public IActionResult OnPostSaveFile()
        {
            System.IO.File.WriteAllText(Config.JsonFilename, Config.JsonDocument.ToString());
            return RedirectToPage(new { Saved = true });
        }
    }
}
