using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForConsilium;

public record GetAvailableDoctorsForConsiliumQuery : IQuery<List<AvailableDoctorDto>>;
