using System;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly MotorcycleService _motorcycleService;

        public MotorcyclesController(MotorcycleService motorcycleService)
        {
            _motorcycleService = motorcycleService;
        }

        /// <summary>
        /// Lista todas as motocicletas cadastradas.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var motorcycles = await _motorcycleService.GetAllAsync();
            return Ok(motorcycles);
        }

        /// <summary>
        /// Busca uma motocicleta específica por ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var moto = await _motorcycleService.GetByIdAsync(id);
            if (moto == null)
                return NotFound(new { error = "Motorcycle not found" });

            return Ok(moto);
        }

        /// <summary>
        /// Cria uma nova motocicleta.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MotorcycleCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _motorcycleService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
