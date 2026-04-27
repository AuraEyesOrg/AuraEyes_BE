using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Entities.Scheduling;
using System.Text.Json;

public class DbCheck
{
    public static async Task Run(ApplicationDbContext context)
    {
        var visits = await context.PatientVisits
            .Include(v => v.Patient)
            .Include(v => v.Appointment)
                .ThenInclude(a => a.AppointmentSlot)
            .ToListAsync();

        Console.WriteLine($"Total visits: {visits.Count}");
        foreach (var v in visits)
        {
            Console.WriteLine($"Visit ID: {v.Id}, Status: {v.Status}, CheckedInAt: {v.CheckedInAt}, Appointment: {v.Appointment?.Id}, Slot: {v.Appointment?.AppointmentSlot?.Id}");
        }
    }
}
