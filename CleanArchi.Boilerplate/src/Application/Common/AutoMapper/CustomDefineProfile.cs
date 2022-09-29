using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchi.Boilerplate.Application.PONumbers.Queries;
using CleanArchi.Boilerplate.Domain.Entities;

namespace CleanArchi.Boilerplate.Application.AutoMapper;

public class CustomDefineProfile : Profile
{
    public CustomDefineProfile()
    {
        CreateMap<PONumberTest, PONumberDto>();
    }
 }
