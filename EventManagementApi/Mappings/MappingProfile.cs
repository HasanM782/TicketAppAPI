using AutoMapper;
using EventManagementApi.DTOs.Event;
using EventManagementApi.DTOs.Organizer;
using EventManagementApi.DTOs.Ticket;
using EventManagementApi.Models;

namespace EventManagementApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Event
            CreateMap<Event, EventGetDto>()
                .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.Name));
            CreateMap<CreateEventDto, Event>();
            CreateMap<UpdateEventDto, Event>();

            // Organizer
            CreateMap<Organizer, OrganizerGetDto>();
            CreateMap<CreateOrganizerDto, Organizer>();
            CreateMap<UpdateOrganizerDto, Organizer>();

            // Ticket
            CreateMap<Ticket, TicketGetDto>()
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));
            CreateMap<CreateTicketDto, Ticket>();
            CreateMap<UpdateTicketDto, Ticket>();
        }
    }
}
