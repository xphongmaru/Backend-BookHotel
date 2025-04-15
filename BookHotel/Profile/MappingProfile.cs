using AutoMapper;
using BookHotel.Models; 
using BookHotel.DTOs;

namespace BookHotel.Profiles 
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TypeRoom, TypeRoomDto>();
            CreateMap<TypeRoomCreateDto, TypeRoom>();

            CreateMap<Amenities, AmenityDto>();
            CreateMap<AmenityCreateDto, Amenities>();
        }
    }
}
