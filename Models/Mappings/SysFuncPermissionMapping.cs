using AutoMapper;

namespace Models.Mappings
{
    public class SysFuncPermissionMapping : Profile
    {
        public SysFuncPermissionMapping()
        {
            CreateMap<UserPermission, SysFuncPermission>()
                .ForMember(x => x.Table, y => y.MapFrom(o => nameof(UserPermission)))
                .ForMember(x => x.Id, y => y.MapFrom(o => o.UserPermissionId))
                .ForMember(x => x.RoleId, y => y.MapFrom(o => string.Empty));

            CreateMap<RolePermission, SysFuncPermission>()
                .ForMember(x => x.Table, y => y.MapFrom(o => nameof(RolePermission)))
                .ForMember(x => x.Id, y => y.MapFrom(o => o.RolePermissionId))
                .ForMember(x => x.UserId, y => y.MapFrom(o => string.Empty));
        }
    }
}
