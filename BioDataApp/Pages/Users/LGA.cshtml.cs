using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
#nullable disable

namespace BioDataApp.Pages.Users
{
    public class LGAModel : PageModel
    {
        // I'm injecting the database context so I can communicate with the database
        private readonly BioDataDbContext context;

        public LGAModel(BioDataDbContext context)
        {
            this.context = context;
        }

        // This is my ViewModel — it holds everything the form will submit
        // CountryId = the country the user selected from the dropdown
        // StateId = the state the user selected from the dropdown
        // LGAList = the list of LGAs the user typed into the table
        public class LGAVM
        {
            public int CountryId { get; set; }
            public int StateId { get; set; }
            public List<LGATab> LGAList { get; set; } = [];
        }

        // I'm binding this so when the form is submitted
        // ASP.NET automatically fills LGATab with what the user selected and typed
        [BindProperty]
        public LGAVM LGATab { get; set; } = new();

        // This list holds all countries from the database
        // I use it to populate the country dropdown on the page
        public List<CountryTab> CountryList { get; set; } = [];

        // This list holds states from the database
        // I use it to populate the state dropdown on the page
        public List<StateTab> StateList { get; set; } = [];

        // OnGetAsync runs when the page first loads
        public async Task<IActionResult> OnGetAsync()
        {
            // I'm loading all countries from the database to fill the country dropdown
            CountryList = await context.CountryTab.ToListAsync();

            // I'm loading all states from the database to fill the state dropdown
            StateList = await context.StateTab.ToListAsync();

            // I'm pre-filling the table with 5 blank rows so the user
            // has something to type into right away without clicking Add Row
            for (int i = 0; i < 5; i++)
            {
                LGATab.LGAList.Add(new LGATab
                {
                    Id = 0,
                    LGACode = "",
                    LGAName = ""
                });
            }

            return Page();
        }

        // This handler is called via AJAX when the user selects a country
        // It returns only the states that belong to that country as JSON
        // The JavaScript on the frontend uses this to populate the state dropdown
        public async Task<IActionResult> OnGetStatesByCountryAsync(int countryId)
        {
            // I'm fetching only states that belong to the selected country
            // and returning just the Id and StateName — nothing extra needed
            var states = await context.StateTab
                .Where(s => s.CountryTabId == countryId)
                .Select(s => new { s.Id, s.StateName })
                .ToListAsync();

            // I'm returning the states as JSON so JavaScript can read them
            return new JsonResult(states);
        }

        // OnPostAsync runs when the user clicks Save or Submit
        // All validation is handled on the frontend so I just save here
        public async Task<IActionResult> OnPostAsync()
        {

            // I'm clearing ModelState so ASP.NET doesn't show its own
            // auto-generated validation errors for the blank rows in the table
            // My frontend already handled all the validation before this runs
            ModelState.Clear();

            // I'm deleting all existing LGAs for this state from the database
            // so I can replace them cleanly with the new ones the user submitted
            var existingLGAs = await context.LGATab
                .Where(l => l.StateTabId == LGATab.StateId)
                .ToListAsync();

            // I'm only removing if there are existing records to avoid
            // unnecessary database calls
            if (existingLGAs.Count > 0)
                context.LGATab.RemoveRange(existingLGAs);

            // I'm filtering out blank rows before saving
            // so empty rows never hit the database and cause NULL errors
            // The frontend already validated that at least one row is filled
            // but I still filter here to keep the database clean
            var filledRows = LGATab.LGAList
                .Where(l => !string.IsNullOrWhiteSpace(l.LGACode)
                         || !string.IsNullOrWhiteSpace(l.LGAName))
                .ToList();

            if (filledRows.Count > 0)
            {
                // I'm projecting each filled row into a new LGATab database record
                // and linking it to the state the user selected
                var newLGAs = filledRows.Select(l => new Models.Users.LGATab
                {
                    // I'm linking each LGA to the state the user selected
                    StateTabId = LGATab.StateId,
                    LGACode = l.LGACode,
                    LGAName = l.LGAName
                }).ToList();

                // I'm adding all the new LGA records to the database context
                context.LGATab.AddRange(newLGAs);
            }

            // I'm saving everything to the database here
            await context.SaveChangesAsync();

            // I'm reloading the country dropdown after saving
            // so the page still shows all countries correctly
            CountryList = await context.CountryTab.ToListAsync();

            // I'm reloading only the states for the selected country
            // so the state dropdown stays correctly filtered after the save
            StateList = await context.StateTab
                .Where(s => s.CountryTabId == LGATab.CountryId)
                .ToListAsync();

            return Page();
        }
    }
}