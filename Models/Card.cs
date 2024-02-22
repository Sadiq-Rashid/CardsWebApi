using System.ComponentModel.DataAnnotations;

namespace Cards.Models

{
    public class Card
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "The 'Name' field is required.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Status { get; set; } = "To Do";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
       
    }

}
