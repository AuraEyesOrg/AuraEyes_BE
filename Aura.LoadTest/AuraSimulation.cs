// =============================================================================
// AuraSimulation.cs
// Project  : AURA – Hệ thống hỗ trợ chẩn đoán bệnh lý võng mạc
// Purpose  : Load / Performance test mô phỏng 3 nhóm hành vi người dùng thực tế
//
// Ghi chú kỹ thuật:
//   - Sử dụng NBomber v5.8.
//   - Đã fix lỗi deserialize JSON bằng JsonElement.
//   - Đã thêm Authentication (JWT Bearer).
// =============================================================================

using System.Net.Http.Json;
using System.Text.Json;
using NBomber.CSharp;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace Aura.LoadTest;

public static class AuraSimulation
{
    private const string BaseUrl = "https://web.auraeyes.site";
    
    // Tài khoản test từ DatabaseSeeder
    private const string TestUser = "doctor@auraeyes.vn";
    private const string TestPass = "Doctor@123$";

    private static HttpClient BuildHttpClient() => new()
    {
        BaseAddress = new Uri(BaseUrl),
        Timeout = TimeSpan.FromSeconds(30),
        DefaultRequestHeaders =
        {
            { "Accept", "application/json" },
            { "User-Agent", "AURA-LoadTest/1.0" }
        }
    };

    /// <summary>
    /// Lấy JWT Token từ Auth API
    /// </summary>
    private static async Task<string?> GetAuthToken(HttpClient client)
    {
        try
        {
            var loginBody = new { Email = TestUser, Password = TestPass };
            var response = await client.PostAsJsonAsync("/api/auth/login", loginBody);
            
            if (response.IsSuccessStatusCode)
            {
                var root = await response.Content.ReadFromJsonAsync<JsonElement>();
                
                // ApiResponse<T> has 'success' and 'data' (camelCase in JSON)
                if (root.TryGetProperty("success", out var success) && success.GetBoolean())
                {
                    var data = root.GetProperty("data");
                    if (data.TryGetProperty("accessToken", out var token))
                    {
                        return token.GetString();
                    }
                }
                
                Console.WriteLine($"[ERROR] Login successful but could not parse token. Response: {root}");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ERROR] Login failed with status {response.StatusCode}. Response: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Login exception: {ex.Message}");
        }
        return null;
    }

    private static ScenarioProps BuildReadQueueScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("read_queue_scenario", async context =>
        {
            var getQueueRequest = Http
                .CreateRequest("GET", "/api/clinic-queue")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            var queueResponse = await Http.Send(httpClient, getQueueRequest);

            if (!queueResponse.IsError)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(500, 1500)));
            }

            return queueResponse;
        });
    }

    private static ScenarioProps BuildBookingScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("booking_scenario", async context =>
        {
            var clinicId = Guid.NewGuid();
            var checkSlotRequest = Http
                .CreateRequest("GET", $"/api/appointment-slots?clinicId={clinicId}")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            var slotResponse = await Http.Send(httpClient, checkSlotRequest);

            if (!slotResponse.IsError)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(1000, 3000)));

                var bookingBody = new
                {
                    slotId = Guid.NewGuid(),
                    patientId = Guid.NewGuid(),
                    visitReason = "Retinal screening - load test",
                    pricingType = "AutoAssign"
                };

                var createBookingRequest = Http
                    .CreateRequest("POST", "/api/clinic-appointments")
                    .WithHeader("Authorization", $"Bearer {token}")
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString())
                    .WithJsonBody(bookingBody);

                return await Http.Send(httpClient, createBookingRequest);
            }

            return slotResponse;
        });
    }

    private static ScenarioProps BuildDoctorReviewScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("doctor_review_scenario", async context =>
        {
            // 1. Xem danh sách các ca tư vấn gần đây (Paging)
            var getSessionsRequest = Http
                .CreateRequest("GET", "/api/consultation-sessions?pageNumber=1&pageSize=10")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            var sessionsResponse = await Http.Send(httpClient, getSessionsRequest);

            if (!sessionsResponse.IsError)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(1000, 2000)));

                // 2. Xem các kết quả tầm soát AI gần đây nhất
                var getRecentAiRequest = Http
                    .CreateRequest("GET", "/api/screenings/recent?limit=10")
                    .WithHeader("Authorization", $"Bearer {token}")
                    .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

                return await Http.Send(httpClient, getRecentAiRequest);
            }

            return sessionsResponse;
        });
    }

    public static void Run()
    {
        using var httpClient = BuildHttpClient();
        
        Console.WriteLine("Logging in to get authentication token...");
        var token = GetAuthToken(httpClient).GetAwaiter().GetResult();
        
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("[CRITICAL] Could not obtain auth token. Simulation aborted.");
            return;
        }
        
        Console.WriteLine("Auth Token obtained successfully.");

        var rampDuration = TimeSpan.FromSeconds(30);
        var peakDuration = TimeSpan.FromMinutes(5);
        var oneSecond    = TimeSpan.FromSeconds(1);

        // Load Simulations
        var readQueueScenario = BuildReadQueueScenario(httpClient, token)
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 57, interval: oneSecond, during: rampDuration),
                Simulation.Inject(rate: 200, interval: oneSecond, during: peakDuration)
            );

        var bookingScenario = BuildBookingScenario(httpClient, token)
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 29, interval: oneSecond, during: rampDuration),
                Simulation.Inject(rate: 100, interval: oneSecond, during: peakDuration)
            );

        var doctorReviewScenario = BuildDoctorReviewScenario(httpClient, token)
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 14, interval: oneSecond, during: rampDuration),
                Simulation.Inject(rate: 50, interval: oneSecond, during: peakDuration)
            );

        NBomberRunner
            .RegisterScenarios(
                readQueueScenario,
                bookingScenario,
                doctorReviewScenario
            )
            .WithWorkerPlugins(new HttpMetricsPlugin())
            .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
            .WithReportFileName("aura_load_test_report")
            .Run();
    }
}
