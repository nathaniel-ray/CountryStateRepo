using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
#nullable disable

namespace BioDataApp.Pages.Users
{
    public class StateModel : PageModel
    {
        // I'm injecting the database context so I can communicate with the database
        private readonly BioDataDbContext context;

        public StateModel(BioDataDbContext context)
        {
            this.context = context;
        }

        // This is my ViewModel — it holds everything the form will submit
        // CountryId = the country the user selected from the dropdown
        // StateList = the list of states the user typed into the table
        public class StateVM
        {
            public int CountryId { get; set; }
            public List<StateTab> StateList { get; set; } = [];
        }

        // I'm binding this so when the form is submitted
        // ASP.NET automatically fills StateTab with what the user selected and typed
        [BindProperty]
        public StateVM StateTab { get; set; } = new();

        // This list holds all states from the database
        public List<StateTab> StateList { get; set; } = [];

        // This list holds all countries from the database
        // I use it to populate the country dropdown on the page
        public List<CountryTab> CountryList { get; set; } = [];

        // OnGetAsync runs when the page first loads
        public async Task<IActionResult> OnGetAsync()
        {
            // I'm loading all states from the database
            StateList = await context.StateTab.ToListAsync();

            // I'm loading all countries from the database to fill the country dropdown
            CountryList = await context.CountryTab.ToListAsync();

            // I'm pre-filling the table with 5 blank rows so the user
            // has something to type into right away without clicking Add Row
            for (int i = 0; i < 5; i++)
            {
                StateTab.StateList.Add(new StateTab
                {
                    CountryTab = null,
                    Id = 0,
                    StateCode = "",
                    StateName = ""
                });
            }

            return Page();
        }

        // OnPostAsync runs when the user clicks Save or Submit
        // All validation is handled on the frontend so I just save here
        // OnPostAsync runs when the user clicks Save or Submit
        // All validation is handled on the frontend so I just save here
        public async Task<IActionResult> OnPostAsync()
        {
            // I'm clearing ModelState so ASP.NET doesn't show its own
            // auto-generated validation errors for the blank rows in the table
            // My frontend already handled all the validation before this runs
            ModelState.Clear();

            // I'm fetching all existing states for the selected country
            // so I can delete them and replace them with the new submission
            // I'm doing this BEFORE resetting StateTab because I still need
            // the CountryId and StateList the user submitted
            var states = await context.StateTab
                .Where(i => i.CountryTabId == StateTab.CountryId)
                .ToListAsync();

            // I'm removing existing states for this country to avoid duplicates
            if (states.Count > 0)
                context.StateTab.RemoveRange(states);

            if (StateTab.StateList.Count > 0)
            {
                // I'm filtering out blank rows before saving
                // so empty rows never hit the database and cause NULL errors
                // The frontend already validated that at least one row is filled
                // but I still filter here to keep the database clean
                var newStates = StateTab.StateList
                    .Where(i => !string.IsNullOrWhiteSpace(i.StateCode)
                             || !string.IsNullOrWhiteSpace(i.StateName))
                    .Select(i => new StateTab
                    {
                        // I'm linking each state to the country the user selected
                        CountryTabId = StateTab.CountryId,
                        StateCode = i.StateCode,
                        StateName = i.StateName
                    }).ToList();

                // I'm adding all the new state records to the database context
                context.StateTab.AddRange(newStates);
            }

            // I'm saving everything to the database here
            int succeed = await context.SaveChangesAsync();

            // I'm reloading the country list after saving
            // so the dropdown still shows all countries on the page
            CountryList = await context.CountryTab.ToListAsync();

            // I'm resetting StateTab AFTER saving so the form is clean
            // and ready for new input without carrying over the old data
            // I'm doing this last so the submitted data is fully saved first
            StateTab = new StateVM
            {
                CountryId = 0,
                StateList = []
            };

            // I'm reloading the 5 blank rows after reset
            // so the table is ready for new input after the save
            for (int i = 0; i < 5; i++)
            {
                StateTab.StateList.Add(new StateTab
                {
                    CountryTab = null,
                    Id = 0,
                    StateCode = "",
                    StateName = ""
                });
            }

            return Page();
        }
    }
}