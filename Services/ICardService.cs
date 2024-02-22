using Cards.Models;
using Cards.Models.DTO;

namespace Cards.Services
{
    public interface ICardService
    {
        Task DeleteAsync(Card card);
        Task<Card> GetByIdAsync(Guid id);
        Task UpdateAsync(Card card);
        Task<List<CardDto>> GetFilteredCardsAsync(Guid userId, string name, string color, string status, DateTime? createdFrom, string sortBy, int? page, int? size);
        Task<List<CardDto>> GetFilteredCardsByAdminsync(Guid userId, string name, string color, string status, DateTime? createdFrom, string sortBy, int? page, int? size);
        
    }
}

