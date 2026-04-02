using AutoMapper;
using Domain.Entities.Consultation;

namespace Application.ConsultationSessions.Common;

public class ConsultationSessionProfile : Profile
{
    public ConsultationSessionProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>();

        CreateMap<ConsultationSession, ConsultationSessionDto>()
            .ForMember(dest => dest.Messages, opt => opt.Ignore())
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.PatientAvatarUrl, opt => opt.Ignore())
            .ForMember(dest => dest.OphthalmologistName, opt => opt.Ignore())
            .ForMember(dest => dest.OrganisationName, opt => opt.Ignore())
            .ForMember(dest => dest.OphthalmologistAvatarUrl, opt => opt.Ignore());

        CreateMap<ConsultationSession, ConsultationSessionListDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.PatientAvatarUrl, opt => opt.Ignore())
            .ForMember(dest => dest.OphthalmologistName, opt => opt.Ignore())
            .ForMember(dest => dest.OrganisationName, opt => opt.Ignore())
            .ForMember(dest => dest.OphthalmologistAvatarUrl, opt => opt.Ignore())
            .ForMember(dest => dest.CaseSnapshot, opt => opt.Ignore())
            .ForMember(dest => dest.LatestMessagePreview, opt => opt.Ignore());
    }
}