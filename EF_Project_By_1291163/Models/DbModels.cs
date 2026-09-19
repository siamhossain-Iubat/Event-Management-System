using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystem.Models
{
    public class Attendee
    {
        [Key]
        public int AttendeeId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; } 

        [Required, StringLength(50)]
        public string? Email { get; set; } 

        [Required, StringLength(15), Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        [Required,StringLength(250)]
        public string? Image { get; set; }
        [Required]
        public bool IsAdult { get; set; } 

        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }

    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required, StringLength(100)]
        public string? EventName { get; set; }

        [Required, Column(TypeName ="date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EventDate { get; set; } 

        [Column(TypeName = "money"),Display(Name = "Price Per Ticket")]
        public decimal TicketPrice { get; set; }
    }

    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }

        [ForeignKey("Attendee")]
        public int AttendeeId { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }

        [Required,Display(Name = "Number Of Ticket")]
        public int TicketCount { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime RegistrationDate { get; set; }
        [Column(TypeName ="money")]
        public decimal TotalPaid { get; set; }

        public virtual Attendee? Attendee { get; set; }
        public virtual Event? Event { get; set; }
    }
}
