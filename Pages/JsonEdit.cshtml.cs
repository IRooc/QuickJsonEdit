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
            else if (child is JsonArray arr)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    var elem = arr[i];
                    string arrPath = currentPath + "/[" + i + "]";
                    if (elem is JsonValue v && v.GetValueKind() == JsonValueKind.String)
                    {
                        paths.Add(arrPath);
                    }
                    else
                    {
                        // recurse into array element
                        CollectEditablePaths(elem, arrPath, paths);
                    }
                }
            }
        }
    }
    else if (node is JsonArray arr)
    {
        for (int i = 0; i < arr.Count; i++)
        {
            var elem = arr[i];
            string arrPath = string.IsNullOrEmpty(prefix) ? "[" + i + "]" : prefix + "/[" + i + "]";
            if (elem is JsonValue v && v.GetValueKind() == JsonValueKind.String)
            {
                paths.Add(arrPath);
            }
            else
            {
                CollectEditablePaths(elem, arrPath, paths);
            }
        }
    }
}



public JsonNode? GetNodeAtPath(string path)
{
    var parts = SplitPath(path);
    JsonNode? currentNode = Config.JsonDocument;
    foreach (var part in parts)
    {
        if (string.IsNullOrEmpty(part)) continue;
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
        else if (currentNode is JsonArray arr)
        {
            if (TryParseArrayIndex(part, out int index) && index >= 0 && index < arr.Count)
            {
                currentNode = arr[index];
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

private string[] SplitPath(string path)
{
    var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    var processedParts = new List<string>();
    foreach (var part in parts)
    {
        if (part.Contains('['))
        {
            int start = 0;
            for (int i = 0; i < part.Length; i++)
            {
                if (part[i] == '[')
                {
                    if (i > start)
                        processedParts.Add(part.Substring(start, i - start));
                    int end = part.IndexOf(']', i);
                    if (end == -1)
                        processedParts.Add(part.Substring(i));
                    else
                        processedParts.Add(part.Substring(i, end - i + 1));
                    start = end + 1;
                    i = end;
                }
            }
            if (start < part.Length)
                processedParts.Add(part.Substring(start));
        }
        else
        {
            processedParts.Add(part);
        }
    }
    return processedParts.ToArray();
}

private bool TryParseArrayIndex(string segment, out int index)
{
    index = -1;
    if (segment.StartsWith("[") && segment.EndsWith("]"))
    {
        var numberStr = segment.Substring(1, segment.Length - 2);
        return int.TryParse(numberStr, out index);
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

    public bool TryUpdateValueAtPath(string path, string newValue)
    {
        var parts = SplitPath(path);
        JsonNode? currentNode = Config.JsonDocument;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            var part = parts[i];
            if (string.IsNullOrEmpty(part)) continue;
            if (currentNode is JsonObject obj)
            {
                if (obj.ContainsKey(part))
                {
                    currentNode = obj[part];
                }
                else
                {
                    // Create new JsonObject if missing key
                    var newObj = new JsonObject();
                    obj[part] = newObj;
                    currentNode = newObj;
                }
            }
            else if (currentNode is JsonArray arr)
            {
                if (TryParseArrayIndex(part, out int index))
                {
                    if (index >= 0 && index < arr.Count)
                    {
                        currentNode = arr[index];
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        string lastPart = parts[^1];
        if (string.IsNullOrEmpty(lastPart)) return false;
        if (currentNode is JsonObject parentObj)
        {
            parentObj[lastPart] = newValue;
            return true;
        }
        else if (currentNode is JsonArray parentArr)
        {
            if (TryParseArrayIndex(lastPart, out int index))
            {
                if (index >= 0 && index < parentArr.Count)
                {
                    parentArr[index] = newValue;
                    return true;
                }
            }
            return false;
        }
        return false;
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
