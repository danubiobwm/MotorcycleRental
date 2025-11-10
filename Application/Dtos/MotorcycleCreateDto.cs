namespace Application.Dtos
{
    public class MotorcycleCreateDto
    {
        public string Model { get; set; } = string.Empty;
        public string Plate { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
