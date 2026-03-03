using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BioDataApp.Pages.Users
{
    public class EditBioModel(BioDataDbContext bioDataDbContext) : PageModel
    {
        private readonly BioDataDbContext bioDataDbContext = bioDataDbContext;

        [BindProperty]
        public User UserDataTab { get; set; } = new User();

        public string Status { get; set; } = "";

        // Runs when page loads - fetches the user by id from the URL
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await bioDataDbContext.Users.FindAsync(id);

            //if (user == null)
            //{
            //    return RedirectToPage("/Users/List");
            //}

            UserDataTab = user;
            return Page();
        }

        // Runs when Save button is clicked
        public async Task<IActionResult> OnPostSaveAsync()
        {
            var user = await bioDataDbContext.Users.FindAsync(UserDataTab.Id);

            if (user == null)
            {
                Status = "User not found.";
                return Page();
            }

            user.Title = UserDataTab.Title;
            user.FirstName = UserDataTab.FirstName;
            user.LastName = UserDataTab.LastName;
            user.MiddleName = UserDataTab.MiddleName;
            user.Address = UserDataTab.Address;
            user.Email = UserDataTab.Email;
            user.PhoneNumber = UserDataTab.PhoneNumber;
            user.DateofBirth = UserDataTab.DateofBirth;
            user.Gender = UserDataTab.Gender;

            //test

            bioDataDbContext.Users.Update(user);
            int count = await bioDataDbContext.SaveChangesAsync();

            Status = count > 0 ? "Saved Successfully" : "Save Failed";
            return RedirectToPage("/Users/List");

        }

        // Runs when Delete button is clicked
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var user = await bioDataDbContext.Users.FindAsync(UserDataTab.Id);

            if (user == null)
            {
                Status = "User not found.";
                return Page();
            }

            bioDataDbContext.Users.Remove(user);
            await bioDataDbContext.SaveChangesAsync();

            return RedirectToPage("/Users/List");
        }
    }
}