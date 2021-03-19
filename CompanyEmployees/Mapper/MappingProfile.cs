using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAddress,
                            c => c.MapFrom(c => c.Address + " " + c.Country));

            CreateMap<Employee, EmployeeDto>();
            CreateMap<CompanyForCreationDto, Company>();//p82
            CreateMap<EmployeeForCreationDto, Employee>();//86
            CreateMap<EmployeeForUpdateDto, Employee>();//101
            CreateMap<CompanyForUpdateDto, Company>();
        }
    }
}
