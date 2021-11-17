namespace DriveSafe.Services
{
    public interface IVehicleInfoService
    {
        Task<(VehicleInfo?, string?)> GetVehicleInfoAsync(string registrationNumber, CancellationToken token);
    }

    public record VehicleInfo(string RegistrationNumber, string Colour, string Make);
}
