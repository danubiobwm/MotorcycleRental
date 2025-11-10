using System;
using System.Linq;
using Domain.Entities;

namespace Infra.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Motorcycles.Any())
            {
                context.Motorcycles.AddRange(
                    new Motorcycle
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Plate = "ABC1D23",
                        Model = "Honda CG 160",
                        Year = 2022,
                    },
                    new Motorcycle
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Plate = "XYZ9Z99",
                        Model = "Yamaha Fazer 250",
                        Year = 2023,
                    }
                );
            }

            if (!context.Couriers.Any())
            {
                context.Couriers.AddRange(
                    new Courier
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Name = "João Entregador",
                        Cnpj = "12345678000100",
                        BirthDate = new DateTime(1990, 5, 12),
                        CnhNumber = "12345678900",
                        CnhCategory = "A",
                        CnhImagePath = "/storage/cnh/joao.jpg"
                    },
                    new Courier
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Name = "Maria das Entregas",
                        Cnpj = "98765432000199",
                        BirthDate = new DateTime(1985, 8, 22),
                        CnhNumber = "98765432100",
                        CnhCategory = "AB",
                        CnhImagePath = "/storage/cnh/maria.jpg"
                    }
                );
            }

            context.SaveChanges();
        }
    }
}
