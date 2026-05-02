// =============================================================================
// Program.cs – Entry Point cho Aura.LoadTest
// Khởi động toàn bộ Load Simulation cho hệ thống AURA.
// =============================================================================

namespace Aura.LoadTest;

internal class Program
{
    /// <summary>
    /// Entry point của .NET Console App.
    /// Chạy trực tiếp bằng: dotnet run --project Aura.LoadTest
    /// </summary>
    static void Main(string[] args)
    {
        Console.WriteLine("=== AURA Load Test Simulation ===");
        Console.WriteLine($"Target : https://web.auraeyes.site");
        Console.WriteLine($"Profile: Ramp 0→100 RPS (30s) → Peak 350 RPS (5 min)");
        Console.WriteLine("Starting simulation...\n");

        // Khởi chạy toàn bộ simulation
        AuraSimulation.Run();

        Console.WriteLine("\n=== Simulation completed. Check HTML report in reports/ ===");
    }
}
