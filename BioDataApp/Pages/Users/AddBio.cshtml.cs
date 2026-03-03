using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace BioDataApp.Pages.Users
{
    public class AddBioModel(BioDataDbContext bioDataDbContext) : PageModel
    {
        private readonly BioDataDbContext bioDataDbContext = bioDataDbContext;

        [BindProperty]
        public User UserDataTab { get; set; } = new User();// call data base table name "user"

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await bioDataDbContext.Users.Where(k => k.Id == UserDataTab.Id).FirstOrDefaultAsync();
            //If sequence contains no element
            if (user == null)
            {
                user = new User
                {
                    Title = UserDataTab.Title,
                    FirstName = UserDataTab.FirstName,
                    LastName = UserDataTab.LastName,
                    MiddleName = UserDataTab.MiddleName,
                    Address = UserDataTab.Address,
                    Email = UserDataTab.Email,
                    PhoneNumber = UserDataTab.PhoneNumber,
                    DateofBirth = UserDataTab.DateofBirth,
                    Gender = UserDataTab.Gender,
                };
                bioDataDbContext.Users.Add(user);
            }
            else
            {
                user.Title = UserDataTab.Title;
                user.FirstName = UserDataTab.FirstName;
                user.LastName = UserDataTab.LastName;
                user.MiddleName = UserDataTab.MiddleName;
                user.Address = UserDataTab.Address;
                user.Email = UserDataTab.Email;
                user.PhoneNumber = UserDataTab.PhoneNumber;
                user.DateofBirth = UserDataTab.DateofBirth;
                user.Gender = UserDataTab.Gender;

                bioDataDbContext.Users.Update(user);
            }
            await bioDataDbContext.SaveChangesAsync();

            return RedirectToPage("/users/list");
        }
    }
}
