using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BioDataApp.Pages.Users
{
    public class ListModel(BioDataDbContext bioDataDbContext) : PageModel
    {
        private readonly BioDataDbContext bioDataDbContext = bioDataDbContext;

        public List<User> UserList { get; set; } = new List<User>();

        public async Task OnGetAsync()
        {
            UserList = await bioDataDbContext.Users.ToListAsync();
        }
    }
}