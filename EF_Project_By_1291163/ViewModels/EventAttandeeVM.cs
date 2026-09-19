using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EventManagementSystem.ViewModels
{
   
    public class RegistrationVM
    {
        public int RegistrationId { get; set; } 

        [Required]
        public int EventId { get; set; }

        public string? EventName { get; set; }

        [Required, Range(1, 100), Display(Name = "Quantity")]
        public int TicketCount { get; set; }

        [Display(Name = "Price")]
        public decimal TicketPrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;


        public decimal TotalPaid => TicketCount * TicketPrice;


        public bool IsDeleted { get; set; } = false;
    }

    public class AttendeeVM
    {
        public int AttendeeId { get; set; }

        [Required, StringLength(100), Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required, EmailAddress, StringLength(50)]
        public string? Email { get; set; }

        [Required, StringLength(15), Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Adult?")]
        public bool IsAdult { get; set; }


        [Display(Name = "Profile Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImage { get; set; }


        public List<RegistrationVM> Registrations { get; set; } = new List<RegistrationVM>();
    }
}
