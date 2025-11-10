using System;
using System.Linq;
using Domain.Entities;

namespace Infra.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Motorcycles.Any() || context.Couriers.Any())
                return;

            // --- Motocicletas iniciais ---
            var motorcycles = new[]
            {
                new Motorcycle { Id = Guid.NewGuid(), Model = "Honda CG 160", Plate = "ABC1D23", Year = 2022 },
                new Motorcycle { Id = Guid.NewGuid(), Model = "Yamaha Factor 150", Plate = "XYZ9K87", Year = 2023 },
                new Motorcycle { Id = Guid.NewGuid(), Model = "Honda Biz 125", Plate = "QWE4R56", Year = 2021 }
            };

            context.Motorcycles.AddRange(motorcycles);

            // --- Entregadores iniciais ---
            var couriers = new[]
            {
                new Courier
                {
                    Id = Guid.NewGuid(),
                    Name = "João Silva",
                    Cnpj = "12345678000199",
                    BirthDate = new DateTime(1990, 5, 10),
                    CnhNumber = "9988776655",
                    CnhCategory = "A",
                    CnhImagePath = "/uploads/cnh/joao.jpg"
                },
                new Courier
                {
                    Id = Guid.NewGuid(),
                    Name = "Maria Oliveira",
                    Cnpj = "98765432000188",
                    BirthDate = new DateTime(1988, 3, 15),
                    CnhNumber = "5566778899",
                    CnhCategory = "A",
                    CnhImagePath = "/uploads/cnh/maria.jpg"
                }
            };

            context.Couriers.AddRange(couriers);

            context.SaveChanges();
        }
    }
}
