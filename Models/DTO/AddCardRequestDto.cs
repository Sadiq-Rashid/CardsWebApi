namespace Cards.Models.DTO
{
    public class AddCardRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Status { get; set; } = "To Do";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       
       
    
    }
}
