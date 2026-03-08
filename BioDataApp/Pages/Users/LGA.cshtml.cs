using BioDataApp.Data;
using BioDataApp.Models.Users;
using BioDataApp.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
#nullable disable

namespace BioDataApp.Pages.Users
{
    public class LGAModel : PageModel
    {
        // I'm injecting both the database context and the service
        // The service handles all my database queries in a clean reusable way
        // The context is still needed for direct save operations in OnPostAsync
        private readonly BioDataDbContext context;
        private readonly ICountryStateLGAServices services;

        public LGAModel(BioDataDbContext context, ICountryStateLGAServices services)
        {
            this.context = context;
            this.services = services;
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

        // I'm using this flag to tell the frontend whether the save was successful
        // It starts as false and I only set it to true after a successful save
        // The frontend reads this to decide whether to show the success message
        public bool SaveSuccess { get; set; } = false;

        // OnGetAsync runs when the page first loads
        // id = the stateId passed in the URL when a state is selected
        // countryId = the countryId ALSO passed in the URL so the country
        // dropdown stays selected after the page reloads when a state is picked
        // Without countryId the country dropdown would clear on every reload
        public async Task<IActionResult> OnGetAsync(int? id, int? countryId)
        {
            // I'm creating a default list of 5 blank LGA rows
            // This is what shows when no state has been selected yet
            List<LGATab> lgas = [];
            for (int i = 0; i < 5; i++)
            {
                lgas.Add(new LGATab
                {
                    Id = 0,
                    LGACode = "",
                    LGAName = ""
                });
            }

            // If a stateId was passed in the URL I load the existing LGAs for that state
            if (id != null)
            {
                // I'm using the service to get all LGAs that belong to this state
                var lgaList = await services.GetLGAsByStateId(Convert.ToInt32(id));

                // If there are existing LGAs I show them in the table
                // If there are none I fall back to the 5 blank rows
                LGATab = new LGAVM
                {
                    // I'm storing the countryId here so the Razor page can
                    // pre-select the correct country option when it renders
                    // This is what keeps the country dropdown from clearing
                    CountryId = countryId.HasValue ? Convert.ToInt32(countryId) : 0,
                    StateId = Convert.ToInt32(id),
                    LGAList = lgaList.Count > 0 ? lgaList : lgas
                };
            }
            else
            {
                // No state was selected yet so I load a blank form
                LGATab = new LGAVM
                {
                    CountryId = 0,
                    StateId = 0,
                    LGAList = lgas
                };
            }

            // I'm loading all countries from the database to fill the country dropdown
            CountryList = await services.GetAllCountries();

            // I'm loading all states so the state dropdown can be pre-populated
            // on page reload — the frontend will filter them by country
            StateList = await services.GetStatesByCountry();

            return Page();
        }

        // This handler is called via AJAX when the user selects a country
        // It returns only the states that belong to that country as JSON
        // The JavaScript uses this to populate the state dropdown dynamically
        public async Task<IActionResult> OnGetStatesByCountryAsync(int countryId)
        {
            // I'm using the service to get only states for the selected country
            var states = await services.GetStatesByCountryId(countryId);

            // I'm returning just the Id and StateName as JSON
            // The frontend only needs these two fields to build the dropdown options
            var result = states.Select(s => new { s.Id, s.StateName }).ToList();

            return new JsonResult(result);
        }

        // OnPostAsync runs when the user clicks Submit
        // All validation is handled on the frontend so I just save here
        public async Task<IActionResult> OnPostAsync()
        {
            // I'm clearing ModelState so ASP.NET doesn't show its own
            // auto-generated validation errors for the blank rows in the table
            // My frontend already handled all the validation before this runs
            ModelState.Clear();

            // I'm using the service to fetch all existing LGAs for the selected state
            // so I can compare them against what the user submitted
            // I'm doing this BEFORE any reset so I still have the submitted StateId
            var existingLGAs = await services.GetLGAsByStateId(LGATab.StateId);

            // I'm building a dictionary of existing LGAs keyed by LGACode
            // This lets me quickly check if a submitted LGA already exists
            // instead of looping through the whole list every time
            var lgaDictionary = existingLGAs.ToDictionary(l => l.LGACode);

            // I'm using && so only rows with BOTH fields filled pass through
            // This prevents a null LGACode from crashing the dictionary lookup
            var filledRows = LGATab.LGAList
                .Where(l => !string.IsNullOrWhiteSpace(l.LGACode)
                         && !string.IsNullOrWhiteSpace(l.LGAName))
                .ToList();

            if (filledRows.Count > 0)
            {
                // I'm looping through each filled row the user submitted
                foreach (var lga in filledRows)
                {
                    // I'm double checking LGACode is not null before hitting the dictionary
                    if (string.IsNullOrWhiteSpace(lga.LGACode)) continue;
                    // I'm checking if an LGA with this code already exists
                    // in the database for this state
                    if (lgaDictionary.TryGetValue(lga.LGACode, out var existing))
                    {
                        // The LGA already exists so I'm just updating its name
                        // This way I don't create duplicates
                        existing.LGAName = lga.LGAName;
                    }
                    else
                    {
                        // This is a brand new LGA so I'm creating a new record
                        // and linking it to the selected state
                        var newLGA = new Models.Users.LGATab
                        {
                            StateTabId = LGATab.StateId,
                            LGACode = lga.LGACode,
                            LGAName = lga.LGAName
                        };

                        // I'm adding the new LGA to the database context
                        context.LGATab.Add(newLGA);
                    }
                }
            }

            // I'm saving all changes — both updates and new records — to the database
            await context.SaveChangesAsync();

            // I'm setting SaveSuccess to true so the frontend knows
            // the save completed successfully and can show the success message
            SaveSuccess = true;

            // I'm reloading the country and state lists after saving
            // so both dropdowns still work correctly on the page
            CountryList = await services.GetAllCountries();
            StateList = await services.GetStatesByCountry();

            // I'm keeping the current CountryId and StateId in LGATab
            // so both dropdowns stay selected after the save
            // This way the user can keep adding more LGAs to the same state
            // without having to re-select the country and state again
            var currentCountryId = LGATab.CountryId;
            var currentStateId = LGATab.StateId;

            // I'm reloading the LGAs for the current state after saving
            // so the table reflects the latest saved data
            var updatedLGAs = await services.GetLGAsByStateId(currentStateId);

            // I'm resetting LGATab but keeping the CountryId and StateId
            // so the dropdowns stay selected and the table shows fresh data
            LGATab = new LGAVM
            {
                CountryId = currentCountryId,
                StateId = currentStateId,
                LGAList = updatedLGAs.Count > 0 ? updatedLGAs : []
            };

            return Page();
        }
    }
}