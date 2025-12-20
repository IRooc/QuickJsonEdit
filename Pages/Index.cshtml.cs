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