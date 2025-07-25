using AutoMapper;

namespace Models.Mappings
{
    public class SysFuncPermissionDetailMapping : Profile
    {
        public SysFuncPermissionDetailMapping()
        {
            CreateMap<SysFuncPermission, SysFuncPermissionDetail>();

            CreateMap<Func, SysFuncPermissionDetail>();
        }
    }
}
