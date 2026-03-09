using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;
using System.Text;

namespace BioDataApp.Pages.Users
{
    public class PPEGroupModel : PageModel
    {
        public Dictionary<string, string> param = [];
        public class PPEGroupVM
        {
            public int? Id { get; set; }
            public string UserCode { get; set; }
            public string ModuleCode { get; set; }
            public string FormCode { get; set; }
            public string PpeCode { get; set; }
            public string PpeDesc { get; set; }
        }
        [BindProperty]
        public PPEGroupVM PPEGroupTab { get; set; } = new();
        public List<PPEGroupVM> PPEGroupTabs { get; set; } = [];
        public string apiURI = "";
        private readonly IConfiguration configuration;

        public PPEGroupModel(IConfiguration configuration)
        {
            this.configuration = configuration;
            apiURI = configuration.GetValue<string>("WebAPIBaseUrl") ?? "";
        }
        public async Task<IActionResult> OnGetAsync()
        {

            PPEGroupTab.UserCode = "Admin";
            PPEGroupTab.ModuleCode = "3";
            PPEGroupTab.FormCode = "PPEGroup";

            var uri = $"{apiURI}/api/PPEGroup/GetPPEgroupALL/GetPPEgroupALl";

            param =  new Dictionary<string, string>
            {
                ["usercode"] = "Admin"
            };

            var body = new StringContent(JsonConvert.SerializeObject(PPEGroupTab), Encoding.UTF8, "application/json");

            using (var client =  new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer","Token");
                var link = QueryHelpers.AddQueryString(uri, param);
                using (var reponse =  await client.PostAsync(uri, body))
                {
                    var json =  await reponse.Content.ReadAsStringAsync();
                    if (reponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        PPEGroupTabs = JsonConvert.DeserializeObject<List<PPEGroupVM>>(json) ?? [];
                        //post
                        var success = JsonConvert.DeserializeObject<PPEGroupVM>(json) ?? new();
                    }
                }
            }

            return Page();
        }
    }
}
