using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuickJsonEdit.Pages;

public class IndexModel : PageModel
{
    public JsonConfig Config { get; }
    [BindProperty(SupportsGet = true)]
    public string FolderPath { get; set; }
    public List<string> JsonFiles { get; set; }

    public IndexModel(JsonConfig config)
    {
        this.Config = config;
        this.JsonFiles = new List<string>();
    }

    public void OnGet()
    {
        if (!string.IsNullOrEmpty(FolderPath))
        {
            if (Directory.Exists(FolderPath))
            {
                JsonFiles = Directory.GetFiles(FolderPath, "*.json").ToList();
            }
            else
            {
                ModelState.AddModelError("folder", "The folder path does not exist.");
                JsonFiles = new List<string>();
            }
        }
    }

    public IActionResult OnPostListFiles()
    {
        FolderPath = Request.Form["folder"];
        if (!Directory.Exists(FolderPath))
        {
            ModelState.AddModelError("folder", "The folder path does not exist.");
            JsonFiles = new List<string>();
            return Page();
        }

        JsonFiles = Directory.GetFiles(FolderPath, "*.json").ToList();

        return Page();
    }

    public IActionResult OnPostGotoEdit()
    {
        var f = Request.Form["file"];
        if (Path.GetExtension(f) != ".json")
        {
            ModelState.AddModelError("file", "Only .json files are supported.");
            return Page();
        }
        Config.SetJson(f);
        return RedirectToPage("JsonEdit");
    }
}