using AutoMapper;
using Domain.Entities.MedicalRecords;

namespace Application.MedicalRecords.Common;

public class MedicalRecordProfile : Profile
{
    public MedicalRecordProfile()
    {
        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapStatus(src.Status)));
    }

    private static string MapStatus(MedicalRecordStatus status)
    {
        return (int)status switch
        {
            <= 1 => "Draft_Admin",
            2 => "Pending_Clinical",
            _ => "Finalized"
        };
    }
}
