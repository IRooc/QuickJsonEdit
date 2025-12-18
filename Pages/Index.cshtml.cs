using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuickJsonEdit.Pages;

public class IndexModel : PageModel
{
    public IndexModel(JsonConfig config)
    {
        this.Config = config;
    }

    public JsonConfig Config { get; }

    public void OnGet()
    {

    }
    public IActionResult OnPostGotoEdit()
    {
        var f = Request.Form.Files["file"];
        if (Path.GetExtension(f.FileName) != ".json")
        {
            ModelState.AddModelError("file", "Only .json files are supported.");
            return Page();
        }
        var fileContent = f.OpenReadStream();

        Config.SetJson(f.FileName, fileContent);
        return RedirectToPage("JsonEdit");
    }
}