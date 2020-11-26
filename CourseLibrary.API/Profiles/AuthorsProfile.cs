using AutoMapper;
using CourseLibrary.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    // Profiles are a neat way to nicely organize our mapping configuration.
    public class AuthorsProfile : Profile
    {
        // Mapping configuration can be added via the constructor. 
        public AuthorsProfile()
        {
            // We need to map the author entity to author dto
            // To create such a map we call into CreateMap passing in the source and destination objects.
            // AutoMapper is a conventional based, i.e. it will map property names on source to destination.
            // If a property doesn't exist, it will be ignored.
            // .ForMemeber() method is used for projection.
            // Projection transforms the source to the destination beyond flattening the object.
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.FirstName}{src.LastName}"))
                .ForMember(
                dest => dest.Age,
                opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<Models.AuthorForCreationDto, Entities.Author>();

        }
    }
}
