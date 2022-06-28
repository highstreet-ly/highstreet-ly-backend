using AutoMapper;
using Highstreetly.Management.Resources;

namespace Highstreetly.Management
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TicketType, TicketTypeConfiguration>();
            CreateMap<TicketTypeConfiguration, TicketType>();

            CreateMap<EventSeries, EventSeries>();
            CreateMap<EventSeries, EventSeries>();

            CreateMap<DashboardStat, DashboardStat>();
            // CreateMap<Resources.OrdersByDay, OrdersByDayViewModel>();
            // CreateMap<Resources.RefundsByDay, RefundsByDayViewModel>();
            // CreateMap<Resources.RegisteredInterestByDay, RegisteredInterestByDayViewModel>();
            // CreateMap<Resources.TicketsSoldByDay, TicketsSoldByDayViewModel>();
        }
    }
}