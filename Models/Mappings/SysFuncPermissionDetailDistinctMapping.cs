using AutoMapper;

namespace Models.Mappings
{
    public class SysFuncPermissionDetailDistinctMapping : Profile
    {
        public SysFuncPermissionDetailDistinctMapping()
        {
            CreateMap<SysFuncPermissionDetail, SysFuncPermissionDetailDistinct>();
        }
    }
}
