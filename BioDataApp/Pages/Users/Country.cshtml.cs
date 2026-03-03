using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace BioDataApp.Pages.Users
{
    public class CountryModel : PageModel
    {
        private readonly BioDataDbContext context;

        public CountryModel(BioDataDbContext context)
        {
            this.context = context;
        }
        [BindProperty]
        public CountryTab CountryTab { get; set; } = new();
        public List<CountryTab> CountryList { get; set; } = [];
        public async Task<IActionResult> OnGetAsync()
        {
            CountryList = await context.CountryTab.ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var add = await context.CountryTab.Where(i => i.Id == CountryTab.Id).FirstOrDefaultAsync();
                add ??= new();

                add.CountryCode = CountryTab.CountryCode;
                add.CountryName = CountryTab.CountryName;

                if (CountryTab.Id > 0)
                    context.CountryTab.Update(add);
                else
                    context.CountryTab.Add(add);

                int succeed = await context.SaveChangesAsync();

                ModelState.Clear();
                CountryTab = new CountryTab
                {
                    Id = 0,
                    CountryCode = "",
                    CountryName = ""
                };
                CountryList = await context.CountryTab.ToListAsync();
            }
           
            return Page();
        }
    }
}
