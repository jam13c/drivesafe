using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;

namespace DriveSafe.Services
{
    public class VehicleInfoService : IVehicleInfoService   
    {
        private readonly IMemoryCache memoryCache;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly VehicleInfoServiceConfig vehicleInfoServiceConfig;    
        public VehicleInfoService(IOptions<VehicleInfoServiceConfig> config, IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            this.vehicleInfoServiceConfig = config.Value;
            this.memoryCache = cache;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<(VehicleInfo?,string?)> GetVehicleInfoAsync(string registrationNumber, CancellationToken token)
        {
            if(memoryCache.TryGetValue<VehicleInfo>(registrationNumber, out var vehicleInfo))
            {
                return (vehicleInfo, null);
            }

            var client = httpClientFactory.CreateClient("dvla");
            
            var req = new HttpRequestMessage(HttpMethod.Post, vehicleInfoServiceConfig.ServiceUri);
            req.Headers.Add("x-api-key",vehicleInfoServiceConfig.ApiKey);
            req.Content = JsonContent.Create(new Request(registrationNumber));
            var response = await client.SendAsync(req);
            if(response.IsSuccessStatusCode)
            {
                var info = await response.Content.ReadFromJsonAsync<VehicleInfo>(cancellationToken:token);
                memoryCache.Set(registrationNumber, info);
                return (info,null);
            }

            var err = await GetErrorFromResponse(response, token);
            return (null, err);

        }

        private static async Task<string> GetErrorFromResponse(HttpResponseMessage response, CancellationToken token)
        {
            var err = await response.Content.ReadFromJsonAsync<Error>(cancellationToken: token);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => $"Registration number looks invalid - check you have typed it correctly",
                HttpStatusCode.Forbidden => $"Server error - authentication failure ({err?.message ?? "No further details"})",
                HttpStatusCode.TooManyRequests => "Server error - too many requests",
                HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout => $"Server error - DVLA service failure ({err?.message ?? "No further details"})",
                HttpStatusCode.NotFound => "Registration number not found - check you have typed it correctly",
                _ => $"Unknown error - {response.StatusCode} ({(int)response.StatusCode}) - {err?.message ?? "No further details"}"
            };
        }

        private record Error ( string message);
        private record Request( string registrationNumber );
    }
}
