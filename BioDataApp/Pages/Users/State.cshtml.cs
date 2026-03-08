using BioDataApp.Data;
using BioDataApp.Models.Users;
using BioDataApp.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
#nullable disable

namespace BioDataApp.Pages.Users
{
    public class StateModel : PageModel
    {
        // I'm injecting the database context and the service
        // The service handles my queries cleanly and the context handles saves
        private readonly BioDataDbContext context;
        private readonly ICountryStateLGAServices services;

        public StateModel(BioDataDbContext context, ICountryStateLGAServices services)
        {
            this.context = context;
            this.services = services;
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

        // This list holds all countries from the database
        // I use it to populate the country dropdown on the page
        public List<CountryTab> CountryList { get; set; } = [];

        // I'm using this flag to tell the frontend whether the save was successful
        // It starts as false and I only set it to true after a successful save
        // The frontend reads this to decide whether to show the success message
        public bool SaveSuccess { get; set; } = false;

        // OnGetAsync runs when the page first loads
        // id = the countryId passed in the URL when a country is selected
        // When id is provided I load existing states for that country
        // When it is null I just load 5 blank rows
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // I'm creating a default list of 5 blank state rows
            // This is what shows when no country has been selected yet
            List<StateTab> states = [];
            for (int i = 0; i < 5; i++)
            {
                states.Add(new StateTab
                {
                    CountryTab = null,
                    Id = 0,
                    StateCode = "",
                    StateName = ""
                });
            }

            // If a countryId was passed in the URL I load existing states for it
            if (id != null)
            {
                // I'm using the service to get all states for this country
                var stateList = await services.GetStatesByCountryId(Convert.ToInt32(id));

                // If there are existing states I show them
                // If there are none I fall back to the 5 blank rows
                StateTab = new StateVM
                {
                    CountryId = Convert.ToInt32(id),
                    StateList = stateList.Count > 0 ? stateList : states
                };
            }
            else
            {
                // No country selected yet so I load a blank form
                StateTab = new StateVM
                {
                    CountryId = 0,
                    StateList = states
                };
            }

            // I'm loading all countries from the database to fill the country dropdown
            CountryList = await services.GetAllCountries();

            return Page();
        }

        // OnPostAsync runs when the user clicks Submit
        // All validation is handled on the frontend so I just save here
        public async Task<IActionResult> OnPostAsync()
        {
            // I'm clearing ModelState so ASP.NET doesn't show its own
            // auto-generated validation errors for the blank rows in the table
            // My frontend already handled all the validation before this runs
            ModelState.Clear();

            // I'm fetching all existing states for the selected country
            // so I can compare them against what the user submitted
            // I'm doing this BEFORE any reset so I still have the submitted CountryId
            var states = await context.StateTab
                .Where(i => i.CountryTabId == StateTab.CountryId)
                .ToListAsync();

            // I'm building a dictionary of existing states keyed by StateCode
            // This lets me quickly check if a submitted state already exists
            // instead of looping through the whole list every time
            var statesDictionary = states.ToDictionary(i => i.StateCode);

            // I'm filtering to only keep rows where BOTH StateCode and StateName have a value
            // Using && instead of || means a row only passes through if neither field is empty
            // This prevents a null StateCode from reaching the dictionary lookup below
            // which was causing the ArgumentNullException crash
            var newStates = StateTab.StateList
                .Where(i => !string.IsNullOrWhiteSpace(i.StateCode)
                         && !string.IsNullOrWhiteSpace(i.StateName)).ToList();

            if (newStates.Count > 0)
            {
                foreach (var state in newStates)
                {
                    // I'm double checking StateCode is not null before hitting the dictionary
                    // just in case a row slipped through the filter above
                    if (string.IsNullOrWhiteSpace(state.StateCode)) continue;

                    if (statesDictionary.TryGetValue(state.StateCode, out var existing))
                    {
                        // The state already exists so I'm just updating its name
                        existing.StateName = state.StateName;
                    }
                    else
                    {
                        // This is a brand new state so I'm creating a new record
                        var newState = new StateTab
                        {
                            CountryTabId = StateTab.CountryId,
                            StateCode = state.StateCode,
                            StateName = state.StateName
                        };

                        if (newState != null)
                            context.StateTab.Add(newState);
                    }
                }
            }

            // I'm saving all changes — both updates and new records — to the database
            int succeed = await context.SaveChangesAsync();

            // I'm setting SaveSuccess to true so the frontend knows
            // the save completed successfully and can show the success message
            SaveSuccess = true;

            // I'm reloading the country list after saving
            // so the dropdown still shows all countries on the page
            CountryList = await services.GetAllCountries();

            // I'm saving the current CountryId before resetting
            // so I can keep the country dropdown selected after save
            var currentCountryId = StateTab.CountryId;

            // I'm reloading the updated states for the current country
            // so the table shows the latest saved data instead of clearing
            var updatedStates = await services.GetStatesByCountryId(currentCountryId);

            // I'm resetting StateTab but keeping the CountryId
            // so the country dropdown stays selected and the table shows fresh data
            StateTab = new StateVM
            {
                CountryId = currentCountryId,
                StateList = updatedStates.Count > 0 ? updatedStates : []
            };

            return Page();
        }
    }
}