using CsvHelper;
using CsvHelper.Configuration.Attributes;
using DriveSafe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DriveSafe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleInfoService vehicleService;

        public VehiclesController(IVehicleInfoService vehicleService)
        {
            this.vehicleService = vehicleService;
        }
       
        [HttpPost]
        public async Task<Vehicle[]> GetVehicles([FromBody] VehicleRequest request, CancellationToken token = default)
        {
            var result = new List<Vehicle>();
            foreach (var registrationNumber in request.RegistrationNumbers)
            {
                var (info, err) = await vehicleService.GetVehicleInfoAsync(registrationNumber, token);
                if (err is null)
                {
                    result.Add(new Vehicle(info!));
                }
                else
                {
                    result.Add(new Vehicle(registrationNumber, err));
                }
            }
            return result.ToArray();
        }

        [HttpPost("download")]
        public async Task<IActionResult> GetCsv([FromBody]DownloadRequest request, CancellationToken token = default)
        {
            var result = new List<VehicleRow>();
            foreach (var registrationNumber in request.RegistrationNumbers)
            {
                var (info, err) = await vehicleService.GetVehicleInfoAsync(registrationNumber, token);
                if (err is null)
                {
                    result.Add(new VehicleRow
                    {
                        Location = request.LocationCode,
                        VolunteerName = request.VolunteerName,
                        RegistrationNumber = info!.RegistrationNumber,
                        Make = info!.Make,
                        Colour = info!.Colour
                    });                    
                }
                else
                {
                    result.Add(new VehicleRow { Location = request.LocationCode, VolunteerName = request.VolunteerName, RegistrationNumber = registrationNumber.ToUpperInvariant().Replace(" ", "") });
                }
            }

            using var stream = new MemoryStream();
            using var sw = new StreamWriter(stream);
            using var csv = new CsvWriter(sw,System.Globalization.CultureInfo.InvariantCulture);
            csv.WriteHeader<VehicleRow>();
            csv.NextRecord();
            csv.WriteRecords(result);
            sw.Flush();
            return File(stream.ToArray(),"text/csv","drivesafe.csv");
        }
    }

    public class VehicleRequest
    {
        [Required]
        public string[] RegistrationNumbers { get; init; } = Array.Empty<string>();
    }

    public class DownloadRequest
    {
        [Required]
        public string[] RegistrationNumbers { get; init; } = Array.Empty<string>();

        public string LocationCode { get; init; } = "41 G2 004";
        public string VolunteerName { get; init; } = String.Empty;
    }

    public class Vehicle
    {
        public Vehicle(VehicleInfo info)
        {
            this.RegistrationNumber = info.RegistrationNumber;
            this.Colour = info.Colour;
            this.Make = info.Make;
            this.IsError = false;
        }

        public Vehicle(string registrationNumber, string err)
        {
            this.RegistrationNumber = registrationNumber.ToUpperInvariant().Replace(" ", "");
            this.IsError = true;
            this.ErrorMessage = err;
        }
        public string? RegistrationNumber { get; init; }
        public string? Colour { get; init; }
        public string? Make { get; init; }

        public bool IsError { get; init; }
        public string? ErrorMessage { get; init; }
    }

    public class VehicleRow
    {
        [Name("Location Code")]
        public string Location { get; set; } = String.Empty;
        [Name("Volunteer Name")]
        public string VolunteerName { get; set; } = String.Empty;
        [Name("Date")]
        [Format("dd/MM/yyyy")]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [Name("Time")]
        public TimeOnly Time { get; set; } = new TimeOnly(0, 0);
        [Name("VRM")]
        public string RegistrationNumber { get; set; } = String.Empty;
        [Name("MAKE")]
        public string Make { get; set; } = String.Empty;
        [Name("MODEL")]
        public string Model { get; set; } = String.Empty;
        [Name("Colour")]
        public string Colour { get; set; } = String.Empty;
        [Name("Offense Type")]
        public string OffenseType { get; set; } = "SPEED";
        [Name("Speed")]
        public int Speed { get; set; } = 0;


    }
}
