using App_Proyecto.DTOS;
using App_Proyecto.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;

namespace App_Proyecto.Mapping
{
    public class UserProfile:Profile 
    {
      
    public UserProfile()
        {
            CreateMap<Usuario, LoginResponseDto>()
                .ForMember(d => d.AccessToken, Opt => Opt.Ignore())
                .ForMember(d => d.RefreshToken, Opt => Opt.Ignore())
                .ForMember(d => d.ExpiresAt, Opt => Opt.Ignore());



                CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(d => d.password, Opt => Opt.Ignore())
                .ForMember(d => d.EmailConfirmed, opt => opt.MapFrom(Src => false))
                .ForMember(d => d.EmailConfirmToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.RefresTokenExpiryTime, opt => opt.Ignore())
                .ForMember(d => d.PasswordResetToken, opt => opt.Ignore())
                .ForMember(d => d.Role, opt => opt.Ignore());
        }


    }
}
