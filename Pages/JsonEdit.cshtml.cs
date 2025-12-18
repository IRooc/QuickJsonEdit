using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuickJsonEdit.Pages
{
    public class JsonEditModel : PageModel
    {
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

            Config.JsonDocument[k] = v.ToString();
            return RedirectToPage(new { Key = k, Saved = true });
        }
    }
}
