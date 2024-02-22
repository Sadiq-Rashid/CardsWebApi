using Cards.Data;
using Cards.Models;
using Cards.Models.DTO;
using Cards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cards.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly CardDbContext dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICardService _cardService;
        private readonly IAuthService _authService;
        public CardsController(UserManager<IdentityUser> userManager,CardDbContext dbContext,ICardService cardService, IAuthService authService) { 

            this.dbContext = dbContext;
            _userManager = userManager;
            _cardService = cardService;
            _authService = authService;
        }

        //Gett All CARDS
        // GET: htpps;//localhost:7155/api/GetAllCardsByUser
        [HttpGet("GetAllMemberCardsById")]
        public async Task<IActionResult> GetAllMemberCards(
            string? name,
            string? color,
            string? status,
            DateTime? createdAt,
            int? page,
            int? size,
            string? sortBy)
                {
            Guid userId = await _authService.GetLoggedInUserIdAsync(User);
            var cardsDto = await _cardService.GetFilteredCardsAsync(userId,name,color,status,createdAt,sortBy,page,size);

                return Ok(cardsDto);
          
        }
        //Gett All CARDS
        // GET: htpps;//localhost:7155/api/GetAllCards
        [Authorize(Roles ="Admin")]
        [HttpGet("GetAllCards")]
        public async Task<IActionResult> GetAllCards(
            string? name,
            string? color,
            string? status,
            DateTime? createdAt,
            int? page,
            int? size,
            string? sortBy)
        {
            Guid userId = await _authService.GetLoggedInUserIdAsync(User);
            var cardsDto = await _cardService.GetFilteredCardsByAdminsync(userId, name, color, status, createdAt, sortBy, page, size);

            return Ok(cardsDto);

        }

        //GET SINGLE CARD BY ID
        //GET: htpps;//localhost:7155/api/cards
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCardById(Guid id)
        {
            //get data from database = domain models
            var card = await dbContext.Cards.FindAsync(id);


            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);

        }

        //POST TO CREATE A NEW CARD
        //post: https://localhost
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody]CardDto addCardDto) {
            var User = HttpContext.User;
            Guid id = await _authService.GetLoggedInUserIdAsync(User);

            if (!addCardDto.Color.StartsWith("#"))
                return BadRequest("Color must start with #");
            var card = new Card()
            {
                Name = addCardDto.Name,
                Color = addCardDto.Color,
                CreatedAt = addCardDto.CreatedAt,
                Description = addCardDto.Description,
                Status = addCardDto.Status,
                UserId = id


            };

            //use Card Model to create a cards
            dbContext.Cards.Add(card);
            dbContext.SaveChanges();

            //Map back to DTO

            var cardDto = new CardDto
            {
                Id = card.Id,
                Name = card.Name,
                Color = card.Color,
                CreatedAt = card.CreatedAt,
                Description = card.Description,
                Status = card.Status
               

            };

            return CreatedAtAction(nameof(GetCardById), new {id = cardDto.Id}, cardDto);
        
        }


        //[Authorize(Roles = "Member, Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCard(Guid id, [FromBody] CardDto cardUpdateDto)
        {
            // Validate input and ensure the user has access to update the card
            var card = await _cardService.GetByIdAsync(id);

            if (card == null)
            {
                return NotFound(); // Card not found
            }

            if (card.UserId != await _authService.GetLoggedInUserIdAsync(User))
            {
                return Forbid(); // User doesn't have access to update this card
            }

            // Update card properties
            card.Name = cardUpdateDto.Name;
            card.Description = cardUpdateDto.Description;
            card.Color = cardUpdateDto.Color;
            card.Status = cardUpdateDto.Status;

            // Save changes to the database
            await _cardService.UpdateAsync(card);

            return Ok("Record Updated Successfully"); // Updated successfully
        }



        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCard(Guid id)
        {
            // Validate input and ensure the user has access to delete the card
            var card = await _cardService.GetByIdAsync(id);

            if (card == null)
            {
                return NotFound(); // Card not found
            }

             //Check if the user has permission to delete the card.
             //This is an example, uncomment the following lines if needed.
             if (card.UserId != await _authService.GetLoggedInUserIdAsync(User))
             {
                 return Forbid(); // User doesn't have access to delete this card
             }

            // Perform the delete operation
            await _cardService.DeleteAsync(card);

            return Ok("Record Deleted Successfully"); // Deleted successfully
        }




    }
}
