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
        private readonly BioDataDbContext context;

        public StateModel(BioDataDbContext context)
        {
            this.context = context;
        }
        public class StateVM
        {
            public int CountryId { get; set; }
            public List<StateTab> StateList { get; set; } = [];
        }



        [BindProperty]
        public StateVM StateTab { get; set; } = new();
        public List<StateTab> StateList { get; set; } = [];
        public List<CountryTab> CountryList { get; set; } = [];
        public async Task<IActionResult> OnGetAsync()
        {
            StateList = await context.StateTab.ToListAsync();
            CountryList = await context.CountryTab.ToListAsync();
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
        public async Task<IActionResult> OnPostAsync()
        {
            var states = await context.StateTab.Where(i => i.CountryTabId == StateTab.CountryId).ToListAsync();
            if (states.Count > 0)
                context.StateTab.RemoveRange(states);

            
            if (StateTab.StateList.Count > 0)
            {
                //projection 
                var newStates = StateTab.StateList.Select(i => new StateTab
                {
                    CountryTabId = StateTab.CountryId,
                    StateCode = i.StateCode,
                    StateName = i.StateName
                }).ToList();

                context.StateTab.AddRange(newStates);

            }

            int succeed = await context.SaveChangesAsync();


            CountryList = await context.CountryTab.ToListAsync();
            return Page();
        }
    }
}
