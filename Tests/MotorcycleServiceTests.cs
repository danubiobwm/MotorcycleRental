using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests
{
    public class MotorcycleServiceTests
    {
        private readonly Mock<IRepository<Motorcycle>> _mockRepo;
        private readonly MotorcycleService _service;

        public MotorcycleServiceTests()
        {
            _mockRepo = new Mock<IRepository<Motorcycle>>();
            _service = new MotorcycleService(_mockRepo.Object);
        }

        [Fact(DisplayName = "Deve retornar todas as motos")]
        public async Task GetAllAsync_ShouldReturnAllMotorcycles()
        {
            // Arrange
            var motorcycles = new List<Motorcycle>
            {
                new Motorcycle { Id = Guid.NewGuid(), Plate = "ABC1234", Model = "Honda CG 160", Year = 2022 },
                new Motorcycle { Id = Guid.NewGuid(), Plate = "XYZ5678", Model = "Yamaha Fazer 250", Year = 2023 }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(motorcycles);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Motorcycle>)result).Count);
        }

        [Fact(DisplayName = "Deve criar moto com sucesso quando dados forem válidos")]
        public async Task CreateAsync_ShouldSucceed_WhenValidData()
        {
            // Arrange
            var dto = new MotorcycleCreateDto
            {
                Plate = "JKL9M10",
                Model = "Honda Biz 125",
                Year = 2021
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Motorcycle>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(dto.Plate, result.Data.Plate);
            Assert.Equal(dto.Model, result.Data.Model);
            Assert.Equal(dto.Year, result.Data.Year);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Motorcycle>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact(DisplayName = "Não deve criar moto quando placa for vazia")]
        public async Task CreateAsync_ShouldFail_WhenPlateIsEmpty()
        {
            // Arrange
            var dto = new MotorcycleCreateDto
            {
                Plate = "",
                Model = "Honda Biz 125",
                Year = 2021
            };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Plate is required.", result.Errors);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Motorcycle>()), Times.Never);
        }

        [Fact(DisplayName = "Deve criar moto com ID gerado automaticamente")]
        public async Task CreateAsync_ShouldGenerateId()
        {
            // Arrange
            var dto = new MotorcycleCreateDto
            {
                Plate = "DEF4567",
                Model = "Yamaha NMax",
                Year = 2024
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Motorcycle>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.Data.Id);
        }
    }
}
