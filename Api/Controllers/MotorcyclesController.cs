using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly IMotorcycleRepository _repository;

        public MotorcyclesController(IMotorcycleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var motorcycles = await _repository.GetAllAsync();
            return Ok(motorcycles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Motorcycle motorcycle)
        {
            await _repository.AddAsync(motorcycle);
            return CreatedAtAction(nameof(GetAll), motorcycle);
        }
    }
}
