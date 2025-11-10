using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : ControllerBase
    {
        private readonly RentalService _rentalService;

        public RentalsController(RentalService rentalService)
        {
            _rentalService = rentalService;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentalCreateDto dto)
        {
            var rental = await _rentalService.CreateRentalAsync(dto);

            if (rental == null)
                return BadRequest(new { error = "Courier or Motorcycle not found" });

            return CreatedAtAction(nameof(Create), new { id = rental.Id }, rental);
        }


        [HttpPost("return")]
        public async Task<IActionResult> Return([FromBody] RentalReturnDto dto)
        {
            var result = await _rentalService.ReturnRentalAsync(dto);

            if (result == null)
                return NotFound(new { error = "Rental not found" });

            return Ok(result);
        }
    }
}
