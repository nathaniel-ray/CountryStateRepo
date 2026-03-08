using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BioDataApp.Pages.Users
{
    public class CountryModel : PageModel
    {
        // I'm injecting the database context so I can communicate with the database
        private readonly BioDataDbContext context;

        public CountryModel(BioDataDbContext context)
        {
            this.context = context;
        }

        // I'm binding this so when the form submits
        // ASP.NET automatically fills CountryTab with what the user typed
        [BindProperty]
        public CountryTab CountryTab { get; set; } = new();

        // This list holds all countries from the database
        // I use it to populate the table on the page
        public List<CountryTab> CountryList { get; set; } = [];

        // I'm using this flag to tell the frontend whether the save was successful
        // It starts as false and I only set it to true after a successful save
        // The frontend reads this to decide whether to show the success message
        public bool SaveSuccess { get; set; } = false;

        // OnGetAsync runs when the page first loads
        public async Task<IActionResult> OnGetAsync()
        {
            // I'm loading all countries from the database to fill the table
            CountryList = await context.CountryTab.ToListAsync();
            return Page();
        }

        // OnPostAsync runs when the user clicks Save
        // All validation is handled on the frontend so I just save here
        public async Task<IActionResult> OnPostAsync()
        {
            // I'm only saving if ASP.NET model validation passed
            // Since frontend handles the visible validation this mostly
            // acts as a safety net in case someone bypasses the frontend
            if (ModelState.IsValid)
            {
                // I'm checking if a country with this Id already exists in the database
                // If it does I update it, if not I create a new one
                var add = await context.CountryTab
                    .Where(i => i.Id == CountryTab.Id)
                    .FirstOrDefaultAsync();

                // If no existing record was found I create a new empty one
                add ??= new();

                // I'm copying the values the user typed into the record
                add.CountryCode = CountryTab.CountryCode;
                add.CountryName = CountryTab.CountryName;

                // If Id is greater than 0 it's an existing record so I update it
                // If Id is 0 it's a new record so I add it
                if (CountryTab.Id > 0)
                    context.CountryTab.Update(add);
                else
                    context.CountryTab.Add(add);

                // I'm saving the record to the database
                int succeed = await context.SaveChangesAsync();

                // I'm setting SaveSuccess to true so the frontend knows
                // the save completed successfully and can show the success message
                SaveSuccess = true;

                // I'm clearing ModelState so ASP.NET doesn't carry over
                // old validation state from the previous submit
                // I'm not changing this block as requested
                ModelState.Clear();
                CountryTab = new CountryTab
                {
                    Id = 0,
                    CountryCode = "",
                    CountryName = ""
                };

                // I'm reloading the country list so the table
                // shows the latest data after saving
                CountryList = await context.CountryTab.ToListAsync();
            }

            return Page();
        }
    }
}