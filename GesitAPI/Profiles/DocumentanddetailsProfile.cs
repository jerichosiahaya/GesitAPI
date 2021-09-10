using AutoMapper;
using GesitAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GesitAPI.Profiles
{
    public class DocumentanddetailsProfile : Profile
    {
        public DocumentanddetailsProfile()
        {
            CreateMap<Documentanddetails, DocumentanddetailsMap>();
        }
    }
}
