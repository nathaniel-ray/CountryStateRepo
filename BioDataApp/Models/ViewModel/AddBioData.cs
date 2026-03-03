using System.ComponentModel.DataAnnotations;

namespace BioDataApp.Models.ViewModel
{
    public class AddBioData
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string DateofBirth { get; set; }
        public string PhoneNumber { get; set; }
    }
}
