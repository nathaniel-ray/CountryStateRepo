using System.ComponentModel.DataAnnotations;

namespace BioDataApp.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        //[DataType("Email")]
        public string Email { get; set; }
        public string DateofBirth { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class CountryTab
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

    }
    public class StateTab
    {
        public int Id { get; set; }
        public int? CountryTabId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public CountryTab CountryTab { get; set; }
    }
    public class  LGATab
    {
        public int Id { get; set; }
        public string LGACode { get; set; }
        public string LGAName { get; set; }
        public StateTab StateTab { get; set; }
    }
}
