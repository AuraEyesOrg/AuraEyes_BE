// =============================================================================
// AuraSimulation.cs
// Project  : AURA – Hệ thống hỗ trợ chẩn đoán bệnh lý võng mạc
// Purpose  : Load / Performance test mô phỏng 3 nhóm hành vi người dùng thực tế
//
// Ghi chú kỹ thuật:
//   - Sử dụng NBomber v5.8.
//   - Đã cập nhật chính xác các Endpoint từ Backend controllers.
//   - Đã thêm Authentication (JWT Bearer) để vượt qua lớp bảo mật.
// =============================================================================

using System.Net.Http.Json;
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
            var response = await client.PostAsJsonAsync("/api/auth/login", new { Email = TestUser, Password = TestPass });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                // Dựa trên ApiResponse<AuthResponse> structure
                return result?.data?.accessToken;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Login failed: {ex.Message}");
        }
        return null;
    }

    private static ScenarioProps BuildReadQueueScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("read_queue_scenario", async context =>
        {
            // 1. Xem danh sách hàng đợi (api/clinic-queue)
            var getQueueRequest = Http
                .CreateRequest("GET", "/api/clinic-queue")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            var queueResponse = await Http.Send(httpClient, getQueueRequest);

            if (!queueResponse.IsError)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(500, 1500)));

                // 2. Giả lập xem chi tiết một hồ sơ trong hàng đợi (nếu có logic bổ sung)
                // Ở đây ta có thể gọi lại chính endpoint này hoặc endpoint detail nếu cần
            }

            return queueResponse;
        });
    }

    private static ScenarioProps BuildBookingScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("booking_scenario", async context =>
        {
            // 1. Kiểm tra slot trống (api/appointment-slots)
            var clinicId = Guid.NewGuid(); // Demo ID
            var checkSlotRequest = Http
                .CreateRequest("GET", $"/api/appointment-slots?clinicId={clinicId}")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            var slotResponse = await Http.Send(httpClient, checkSlotRequest);

            if (!slotResponse.IsError)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(1000, 3000)));

                // 2. Đặt lịch khám (api/clinic-appointments)
                var bookingBody = new
                {
                    slotId = Guid.NewGuid(), // Demo ID
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

    private static ScenarioProps BuildAiScreeningScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("ai_screening_scenario", async context =>
        {
            // 1. Tạo session sàng lọc (api/screenings/create-session)
            var createSessionBody = new
            {
                modelVersion = "AURA_v1.0",
                retinalImages = new List<object>()
            };

            var createSessionRequest = Http
                .CreateRequest("POST", "/api/screenings/create-session")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("Content-Type", "application/json")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString())
                .WithJsonBody(createSessionBody);

            var sessionResponse = await Http.Send(httpClient, createSessionRequest);

            if (!sessionResponse.IsError)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(2000, 5000)));

                // 2. Upload ảnh võng mạc (api/screenings/upload-images)
                var fakeImageBytes = new byte[50 * 1024]; // 50KB
                Random.Shared.NextBytes(fakeImageBytes);

                using var formContent = new MultipartFormDataContent();
                formContent.Add(new ByteArrayContent(fakeImageBytes), "images", "eye_scan.jpg");

                using var uploadHttpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/screenings/upload-images")
                {
                    Content = formContent
                };
                uploadHttpRequest.Headers.Add("Authorization", $"Bearer {token}");
                uploadHttpRequest.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());

                using var uploadResponse = await httpClient.SendAsync(uploadHttpRequest);
                var statusCode = ((int)uploadResponse.StatusCode).ToString();
                
                return uploadResponse.IsSuccessStatusCode
                    ? Response.Ok(statusCode: statusCode)
                    : Response.Fail(statusCode: statusCode, message: "Upload failed", sizeBytes: 0);
            }

            return sessionResponse;
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

        var aiScreeningScenario = BuildAiScreeningScenario(httpClient, token)
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 14, interval: oneSecond, during: rampDuration),
                Simulation.Inject(rate: 50, interval: oneSecond, during: peakDuration)
            );

        NBomberRunner
            .RegisterScenarios(
                readQueueScenario,
                bookingScenario,
                aiScreeningScenario
            )
            .WithWorkerPlugins(new HttpMetricsPlugin())
            .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
            .WithReportFileName("aura_load_test_report")
            .Run();
    }
}
