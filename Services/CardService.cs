using Cards.Data;
using Cards.Models;
using Cards.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing;
using System.Globalization;

namespace Cards.Services
{
    public class CardService : ICardService
    {
        private readonly CardDbContext _cardDbContext;
        private readonly IAuthService _authService;

        public CardService(CardDbContext cardDbContext, IAuthService authService)
        {
            _cardDbContext = cardDbContext;
            _authService = authService;
        }

     
        public async Task<Card> GetByIdAsync(Guid id)
        {       
            return await _cardDbContext.Cards.FindAsync(id);
        }
        public async Task UpdateAsync(Card card)
        {         
            // Update the entity state in the DbContext
            _cardDbContext.Entry(card).State = EntityState.Modified;
            // Save changes to the database
            await _cardDbContext.SaveChangesAsync();
         
        }

        public async Task DeleteAsync(Card card)
        {
            //Remove the card from the database
            _cardDbContext.Cards.Remove(card);
            await _cardDbContext.SaveChangesAsync();
        }


        public async Task<List<CardDto>> GetFilteredCardsAsync(Guid userId, string name, string color, string status, DateTime? CreatedAt, string sortBy, int? page, int? size)
        {
           
            
                var query = _cardDbContext.Cards.Where(user => user.UserId == userId);

                ApplyFilters(ref query, name, color, status, CreatedAt);
                ApplySorting(ref query, sortBy);
                ApplyPagination(ref query, page, size);

                return await query
                    .Select(card => new CardDto
                    {
                        Id = card.Id,
                        Name = card.Name,
                        Color = card.Color,
                        CreatedAt = card.CreatedAt,
                        Description = card.Description,
                        Status = card.Status
                    })
                    .ToListAsync();
            
        }


        public async Task<List<CardDto>> GetFilteredCardsByAdminsync(Guid userId, string name, string color, string status, DateTime? CreatedAt, string sortBy, int? page, int? size)
        {

                var query = _cardDbContext.Cards.AsQueryable();

                ApplyFilters(ref query, name, color, status, CreatedAt);
                ApplySorting(ref query, sortBy);
                ApplyPagination(ref query, page, size);

                        return await query
                            .Select(card => new CardDto
                            {
                                Id = card.Id,
                                Name = card.Name,
                                Color = card.Color,
                                CreatedAt = card.CreatedAt,
                                Description = card.Description,
                                Status = card.Status
            })
                            .ToListAsync();
        }

        private static void ApplyFilters(ref IQueryable<Card> query, string name, string color, string status, DateTime? CreatedAt)
        {
            if (!string.IsNullOrEmpty(name))
                query = query.Where(card => card.Name.Contains(name));

            if (!string.IsNullOrEmpty(color))
                query = query.Where(card => card.Color == color);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(card => card.Status == status);

            if (CreatedAt.HasValue)
                query = query.Where(card => card.CreatedAt >= CreatedAt.Value);
        }

        private static void ApplySorting(ref IQueryable<Card> query, string sortBy)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(card => card.Name);
                        break;
                    case "color":
                        query = query.OrderBy(card => card.Color);
                        break;
                    case "status":
                        query = query.OrderBy(card => card.Status);
                        break;
                    case "createdat":
                        query = query.OrderBy(card => card.CreatedAt);
                        break;
                    default:
                        throw new ArgumentException("Invalid sortBy parameter");
                }
            }
        }

        private static void ApplyPagination(ref IQueryable<Card> query, int? page, int? size)
        {
            if (page.HasValue && size.HasValue)
                query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
        }


        public static bool IsColornameValid(string color)
        {
            // Check if the username is not null and starts with '#'
            return !string.IsNullOrEmpty(color) && color.StartsWith("#");
        }

    }
}
