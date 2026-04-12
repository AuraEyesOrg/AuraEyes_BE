using Application.Common.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Commands.PaySalary;

public class PayOphthalmologistSalaryCommand : ICommand<string>
{
    public Guid OphthalmologistId { get; set; }
    public decimal? Amount { get; set; }
    public string? Note { get; set; }
}
