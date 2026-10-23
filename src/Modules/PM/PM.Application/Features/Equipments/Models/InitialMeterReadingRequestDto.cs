namespace Himapp.PM.Application.Features.Equipments.Models;

public sealed class InitialMeterReadingRequestDto
{
    public decimal? OpeningReadingAmount { get; set; }
    public string? OpeningDate { get; set; }
    public int? MeterReadingID { get; set; }
}
