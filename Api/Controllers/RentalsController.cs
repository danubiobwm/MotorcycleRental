using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;
using System.Threading.Tasks;
using System;

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

        /// <summary>
        /// Cria um novo aluguel de motocicleta.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentalCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { error = "Body (dto) is required" });

            try
            {
                var rental = await _rentalService.CreateRentalAsync(dto);

                if (rental == null)
                    return BadRequest(new { error = "Courier or Motorcycle not found" });

                return CreatedAtAction(nameof(GetAll), new { id = rental.Id }, rental);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        /// <summary>
        /// Devolve uma motocicleta alugada.
        /// </summary>
        [HttpPost("return")]
        public async Task<IActionResult> Return([FromBody] RentalReturnDto dto)
        {
            if (dto == null)
                return BadRequest(new { error = "Body (dto) is required" });

            try
            {
                var result = await _rentalService.ReturnRentalAsync(dto);

                if (result == null)
                    return NotFound(new { error = "Rental not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os aluguéis (para debug ou histórico).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromServices] Domain.Interfaces.IRepository<Domain.Entities.Rental> rentalRepository)
        {
            var rentals = await rentalRepository.GetAllAsync();
            return Ok(rentals);
        }
    }
}
