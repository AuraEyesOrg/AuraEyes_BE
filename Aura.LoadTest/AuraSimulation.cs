using System.Text.Json;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Contracts;
using NBomber.Contracts.Stats;

namespace Aura.LoadTest;

public static class AuraSimulation
{
    private const string BaseUrl = "https://web.auraeyes.site";
    
    // Pools for real IDs to avoid 404
    private static List<Guid> _patientIds = new();
    private static List<Guid> _slotIds = new();

    private static ScenarioProps BuildReadQueueScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("read_queue_scenario", async context =>
        {
            var request = Http
                .CreateRequest("GET", "/api/clinic-queue")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            return await Http.Send(httpClient, request);
        });
    }

    private static ScenarioProps BuildBookingScenario(HttpClient httpClient, string token)
    {
        return Scenario.Create("booking_scenario", async context =>
        {
            if (_patientIds.Count == 0 || _slotIds.Count == 0)
            {
                return Response.Fail(message: "No real IDs available for booking");
            }

            // Pick random IDs from the pool
            var patientId = _patientIds[Random.Shared.Next(_patientIds.Count)];
            var slotId = _slotIds[Random.Shared.Next(_slotIds.Count)];

            var bookingBody = new
            {
                slotId = slotId,
                patientId = patientId,
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
        });
    }

    public static void Run()
    {
        using var httpClient = BuildHttpClient();
        
        Console.WriteLine("\nLogging in as multiple roles...");

        // 1. Coordinator Token (for Queue - Fixes 403)
        string coordinatorToken = GetAuthToken(httpClient, "coordinator@auraeyes.vn", "Staff@123$").GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(coordinatorToken)) return;
        Console.WriteLine("Coordinator Token obtained (for Queue).");

        // 2. Staff Token (for Booking & Data Fetching)
        string staffToken = GetAuthToken(httpClient, "receptionist@auraeyes.vn", "Staff@123$").GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(staffToken)) return;
        Console.WriteLine("Staff Token obtained (for Booking).");

        // 3. Doctor Token (for Doctor Dashboard)
        string doctorToken = GetAuthToken(httpClient, "doctor@auraeyes.vn", "Doctor@123$").GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(doctorToken)) return;
        Console.WriteLine("Doctor Token obtained.");

        // 4. Patient Token (for Patient History & Slot Searching)
        string patientToken = GetAuthToken(httpClient, "patient@auraeyes.vn", "Patient@123$").GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(patientToken)) return;
        Console.WriteLine("Patient Token obtained.");

        // Pre-fetch real IDs to avoid 404
        Console.WriteLine("Fetching real Patient and Slot IDs...");
        _patientIds = FetchPatientIds(httpClient, staffToken).GetAwaiter().GetResult();
        _slotIds = FetchSlotIds(httpClient, patientToken).GetAwaiter().GetResult();
        Console.WriteLine($"Found {_patientIds.Count} patients and {_slotIds.Count} available slots.");

        var oneSecond = TimeSpan.FromSeconds(1);
        var rampDuration = TimeSpan.FromSeconds(30);
        var peakDuration = TimeSpan.FromMinutes(5);

        // --- Scenarios ---

        // 1. Coordinator: Read Queue (200 RPS Peak)
        var readQueueScenario = BuildReadQueueScenario(httpClient, coordinatorToken)
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 57, interval: oneSecond, during: rampDuration),
                Simulation.Inject(rate: 200, interval: oneSecond, during: peakDuration)
            );

        // 2. Staff: Booking (100 RPS Peak)
        var bookingScenario = BuildBookingScenario(httpClient, staffToken)
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 29, interval: oneSecond, during: rampDuration),
                Simulation.Inject(rate: 100, interval: oneSecond, during: peakDuration)
            );

        // 3. Doctor: Dashboard (25 RPS Peak)
        var doctorReviewScenario = Scenario.Create("doctor_dashboard_scenario", async context =>
        {
            var request = Http
                .CreateRequest("GET", "/api/consultation-sessions?pageNumber=1&pageSize=10")
                .WithHeader("Authorization", $"Bearer {doctorToken}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            return await Http.Send(httpClient, request);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 7, interval: oneSecond, during: rampDuration),
            Simulation.Inject(rate: 25, interval: oneSecond, during: peakDuration)
        );

        // 4. Patient: History (25 RPS Peak)
        var patientHistoryScenario = Scenario.Create("patient_history_scenario", async context =>
        {
            var request = Http
                .CreateRequest("GET", "/api/screenings/recent?limit=10")
                .WithHeader("Authorization", $"Bearer {patientToken}")
                .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString());

            return await Http.Send(httpClient, request);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingInject(rate: 7, interval: oneSecond, during: rampDuration),
            Simulation.Inject(rate: 25, interval: oneSecond, during: peakDuration)
        );

        NBomberRunner
            .RegisterScenarios(
                readQueueScenario,
                bookingScenario,
                doctorReviewScenario,
                patientHistoryScenario
            )
            .WithWorkerPlugins(new HttpMetricsPlugin())
            .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
            .WithReportFileName("aura_load_test_report")
            .Run();
    }

    private static async Task<string> GetAuthToken(HttpClient client, string email, string password)
    {
        try
        {
            var loginBody = new { email, password };
            var content = new StringContent(JsonSerializer.Serialize(loginBody), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("data").GetProperty("accessToken").GetString() ?? "";
            }
            
            Console.WriteLine($"[ERROR] Login failed for {email}: {response.StatusCode}");
            return "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Auth error for {email}: {ex.Message}");
            return "";
        }
    }

    private static async Task<List<Guid>> FetchPatientIds(HttpClient client, string token)
    {
        var ids = new List<Guid>();
        try
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/clinic/patients");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                foreach (var item in doc.RootElement.GetProperty("data").EnumerateArray())
                {
                    // FIXED: Property name is "id" in RecentClinicPatientDto
                    ids.Add(item.GetProperty("id").GetGuid());
                }
            }

            // FALLBACK: If no patients found, create one!
            if (ids.Count == 0)
            {
                Console.WriteLine("No patients found. Creating a walk-in patient for testing...");
                var walkInBody = new
                {
                    fullName = "Load Test Patient",
                    email = $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@auraeyes.vn",
                    phoneNumber = "0900000000",
                    gender = "1" // Male
                };
                var content = new StringContent(JsonSerializer.Serialize(walkInBody), System.Text.Encoding.UTF8, "application/json");
                var createResponse = await client.PostAsync("/api/clinic-staff/patients/walk-in", content);
                if (createResponse.IsSuccessStatusCode)
                {
                    var createJson = await createResponse.Content.ReadAsStringAsync();
                    using var createDoc = JsonDocument.Parse(createJson);
                    // The API returns the Guid of the created patient
                    var patientId = createDoc.RootElement.GetProperty("data").GetGuid();
                    ids.Add(patientId);
                    Console.WriteLine($"Successfully created walk-in patient: {patientId}");
                }
            }
        }
        catch (Exception ex) { Console.WriteLine($"Fetch patients error: {ex.Message}"); }
        return ids;
    }

    private static async Task<List<Guid>> FetchSlotIds(HttpClient client, string token)
    {
        var ids = new List<Guid>();
        try
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/patient/search/available-slots?pageSize=50");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data");
                var items = data.GetProperty("items");
                foreach (var item in items.EnumerateArray())
                {
                    ids.Add(item.GetProperty("id").GetGuid());
                }
            }
        }
        catch (Exception ex) { Console.WriteLine($"Fetch slots error: {ex.Message}"); }
        return ids;
    }

    private static HttpClient BuildHttpClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }
}
