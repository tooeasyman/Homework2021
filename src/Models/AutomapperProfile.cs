using AutoMapper;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     <see cref="AutoMapper" /> profile
    /// </summary>
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<User, UserPaymentsPayload>().ReverseMap();
            CreateMap<User, UserPayload>().ReverseMap();
            CreateMap<Payment, PaymentPayload>()
                .ForMember(x => x.Status, opt => opt.MapFrom(x => x.Status.StatusValue))
                .ReverseMap();
        }
    }
}