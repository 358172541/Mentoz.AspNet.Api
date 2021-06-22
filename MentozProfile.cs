using AutoMapper;
using System;
using System.Collections.Generic;

namespace Mentoz.AspNet.Api
{
    public class MentozProfile : Profile
    {
        public static Dictionary<bool, string> AVAILABLE =
            new Dictionary<bool, string>
            {
                { true, "TRUE" },
                { false, "FALSE" }
            };

        public static Dictionary<RescType, string> RESCTYPE =
            new Dictionary<RescType, string>
            {
                { RescType.MENU, "MENU" },
                { RescType.CTRL, "CTRL" }
            };

        public static Dictionary<string, string> ICON =
            new Dictionary<string, string> {
                { "form", "FORM" }
            };

        public static Dictionary<UserType, string> USERTYPE =
            new Dictionary<UserType, string>
            {
                { UserType.MANAGER, "MANAGER" },
                { UserType.MEMBER, "MEMBER" }
            };

        public MentozProfile()
        {
            CreateMap<Resc, RescModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.RescId))
                .ForMember(
                    dest => dest.TypeDisplay,
                    opts => opts.MapFrom(src => RESCTYPE[src.Type]))
                .ForMember(
                    dest => dest.AvailableDisplay,
                    opts => opts.MapFrom(src => AVAILABLE[src.Available]));

            CreateMap<RescCreateModel, Resc>()
                .ForMember(
                    dest => dest.RescId,
                    opts => opts.MapFrom(src => Guid.NewGuid()));

            CreateMap<Resc, RescUpdateModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.RescId))
                .ForMember(
                    dest => dest.Version,
                    opts => opts.MapFrom(src => src.Version.ToHexString()));

            CreateMap<RescUpdateModel, Resc>()
                .ForMember(
                    dest => dest.RescId,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.Version,
                    opts => opts.Ignore());

            CreateMap<Role, RoleModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.RoleId))
                .ForMember(
                    dest => dest.AvailableDisplay,
                    opts => opts.MapFrom(src => AVAILABLE[src.Available]));

            CreateMap<RoleCreateModel, Role>()
                .ForMember(
                    dest => dest.RoleId,
                    opts => opts.MapFrom(src => Guid.NewGuid()));

            CreateMap<Role, RoleUpdateModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.RoleId))
                .ForMember(
                    dest => dest.Version,
                    opts => opts.MapFrom(src => src.Version.ToHexString()));

            CreateMap<RoleUpdateModel, Role>()
                .ForMember(
                    dest => dest.RoleId,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.Version,
                    opts => opts.Ignore());

            CreateMap<User, UserModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.UserId))
                .ForMember(
                    dest => dest.AvailableDisplay,
                    opts => opts.MapFrom(src => AVAILABLE[src.Available]));

            CreateMap<UserCreateModel, User>()
                .ForMember(
                    dest => dest.UserId,
                    opts => opts.MapFrom(src => Guid.NewGuid()));

            CreateMap<User, UserUpdateModel>()
                .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.UserId))
                .ForMember(
                    dest => dest.Version,
                    opts => opts.MapFrom(src => src.Version.ToHexString()));

            CreateMap<UserUpdateModel, User>()
                .ForMember(
                    dest => dest.UserId,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.Version,
                    opts => opts.Ignore());
        }
    }
}