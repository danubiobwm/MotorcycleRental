using System;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouriersController : ControllerBase
    {
        private readonly CourierService _courierService;

        public CouriersController(CourierService courierService)
        {
            _courierService = courierService;
        }

        /// <summary>
        /// Lista todos os entregadores cadastrados.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromServices] Domain.Interfaces.ICourierRepository courierRepository)
        {
            var couriers = await courierRepository.GetAllAsync();
            return Ok(couriers);
        }

        /// <summary>
        /// Cria um novo entregador.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourierCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _courierService.CreateAsync(dto);

                if (!result.Success)
                    return BadRequest(new { errors = result.Errors });

                return CreatedAtAction(nameof(GetAll), new { id = result.Data?.Id }, result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
