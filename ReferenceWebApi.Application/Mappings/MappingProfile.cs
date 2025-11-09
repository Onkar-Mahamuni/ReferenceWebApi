using AutoMapper;
using ReferenceWebApi.Domain.Entities;
using ReferenceWebApi.Application.Dtos.Employee;

namespace ReferenceWebApi.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();
        }
    }
}
