using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;

namespace BioDataApp.Pages.Users
{
    public class PPEGroupModel : PageModel
    {
        // I'm using this dictionary to hold query parameters for API calls
        public Dictionary<string, string> param = [];

        // This is my ViewModel — it maps to what the API expects to receive and return
        public class PPEGroupVM
        {
            public int? Id { get; set; }
            public string UserCode { get; set; }
            public string ModuleCode { get; set; }
            public string FormCode { get; set; }
            public string PpeCode { get; set; }
            public string PpeDesc { get; set; }
        }

        // I'm binding this so when the form submits
        // ASP.NET automatically fills PPEGroupTab with what the user typed
        [BindProperty]
        public PPEGroupVM PPEGroupTab { get; set; } = new();

        // This list holds all PPE Groups returned from the GET API call
        // I use it to populate the table on the page
        public List<PPEGroupVM> PPEGroupTabs { get; set; } = [];

        // I'm using this flag to tell the frontend whether the save was successful
        // It starts as false and I only set it to true after a successful save
        public bool SaveSuccess { get; set; } = false;

        // I'm using this to show any error message returned from the API
        // if the save fails so the user knows what went wrong
        public string SaveError { get; set; } = "";

        // I'm storing the base API URL from my configuration (appsettings.json)
        // so I don't hardcode it everywhere
        public string apiURI = "";

        private readonly IConfiguration configuration;

        public PPEGroupModel(IConfiguration configuration)
        {
            this.configuration = configuration;

            // I'm reading the base API URL from appsettings.json
            apiURI = configuration.GetValue<string>("WebAPIBaseUrl") ?? "";
        }

        // OnGetAsync runs when the page first loads
        // I'm calling the API to get all existing PPE Groups
        // and populate the table on the page
        public async Task<IActionResult> OnGetAsync()
        {
            // I'm setting the default values that the API expects
            PPEGroupTab.UserCode = "Admin";
            PPEGroupTab.ModuleCode = "3";
            PPEGroupTab.FormCode = "PPEGroup";
            PPEGroupTab.Id = 0;

            // I'm building the GET endpoint URL
            var uri = $"{apiURI}/api/PPEGroup/GetPPEgroupALL/GetPPEgroupALl";

            // I'm building the query parameters to pass in the URL
            param = new Dictionary<string, string>
            {
                ["usercode"] = "Admin"
            };

            // I'm appending the query params to the URL
            // e.g /api/PPEGroup/GetPPEgroupALL/GetPPEgroupALl?usercode=Admin
            var link = QueryHelpers.AddQueryString(uri, param);

            using (var client = new HttpClient())
            {
                // I'm attaching the Bearer token so the API knows I'm authorized
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Token");

                // I'm using GetAsync because I'm fetching data not saving it
                using (var response = await client.GetAsync(link))
                {
                    // I'm reading the response body as a string
                    var json = await response.Content.ReadAsStringAsync();

                    // I'm only deserializing if the request was successful
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // I'm deserializing the JSON array into my list of PPEGroupVM
                        PPEGroupTabs = JsonConvert.DeserializeObject<List<PPEGroupVM>>(json) ?? [];
                    }
                }
            }

            return Page();
        }

        // OnPostAsync runs when the user clicks Save
        // I'm sending the form data to the API save endpoint
        public async Task<IActionResult> OnPostAsync()
        {
            // I'm clearing ModelState so ASP.NET doesn't block the save
            // with its own auto-generated validation errors
            ModelState.Clear();

            

            // I'm building the POST/Save endpoint URL
            var saveUri = "https://bsslapidemo.dcontroller.com/api/PPEGroup/SaveGrpSetupRecord/SaveRecord";

            // I'm serializing my PPEGroupTab ViewModel into JSON
            // so the API can read it as the request body
            var body = new StringContent(
                JsonConvert.SerializeObject(PPEGroupTab),
                Encoding.UTF8,
                "application/json"
            );

            using (var client = new HttpClient())
            {
                // I'm attaching the Bearer token so the API knows I'm authorized
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Token");

                // I'm using PostAsync because I'm sending data to be saved
                using (var response = await client.PostAsync(saveUri, body))
                {
                    // I'm reading the response body as a string
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // I'm setting SaveSuccess to true so the frontend
                        // knows the save was successful and shows the success message
                        SaveSuccess = true;

                        // I'm clearing the form inputs after a successful save
                        // so the user can enter a new PPE Group right away
                        PPEGroupTab = new PPEGroupVM
                        {
                            UserCode = "Admin",
                            ModuleCode = "3",
                            FormCode = "PPEGroup"
                        };
                    }
                    else
                    {
                        // I'm storing the error message returned by the API
                        // so the frontend can show it to the user
                        SaveError = $"Save failed: {response.StatusCode} — {json}";
                    }
                }
            }

            // I'm reloading the PPE Group list after saving
            // so the table reflects the latest data including the new record
            await LoadPPEGroupsAsync();

            return Page();
        }

         //LoadPPEGroupsAsync: I'm extracting the GET logic into its own method
         //so I can call it from both OnGetAsync and OnPostAsync without duplicating code
        private async Task LoadPPEGroupsAsync()
        {
            // I'm building the GET endpoint URL
            var uri = $"{apiURI}/api/PPEGroup/GetPPEgroupALL/GetPPEgroupALl";

            // I'm building the query parameters
            param = new Dictionary<string, string>
            {
                ["usercode"] = "Admin"
            };

            // I'm appending the query params to the URL
            var link = QueryHelpers.AddQueryString(uri, param);

            using (var client = new HttpClient())
            {
                // I'm attaching the Bearer token so the API knows I'm authorized
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "Token");

                using (var response = await client.GetAsync(link))
                {
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // I'm deserializing the JSON array into my list
                        PPEGroupTabs = JsonConvert.DeserializeObject<List<PPEGroupVM>>(json) ?? [];
                    }
                }
            }
        }
    }
}