using AutoMapper;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;

namespace HostelMealManagement.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Member, MemberVm>().ReverseMap();
            CreateMap<MealBazar, MealBazarVm>().ReverseMap();
            CreateMap<MealAttendance, MealAttendanceVm>().ReverseMap();

            // ✅ REQUIRED FOR NORMAL PAYMENT (THIS WAS MISSING)
            CreateMap<NormalPayment, NormalPaymentVm>().ReverseMap();
        }
    }
}
