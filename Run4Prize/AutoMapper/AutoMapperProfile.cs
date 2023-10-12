using AutoMapper;
using com.strava.v3.api.Activities;
using com.strava.v3.api.Athletes;
using com.strava.v3.api.Authentication;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Run4Prize.Models.DBContexts.AppContext;
using Run4Prize.Models.Domains;
using System;

namespace Run4Prize.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Athlete, AthleteEntity>();
            CreateMap<AthleteEntity, Athlete>();
            CreateMap<Athlete, Athlete>();
            CreateMap<AthleteEntity, AthleteEntity>();

            CreateMap<AccessToken, AccessTokenEntity>();
            CreateMap<AccessTokenEntity, AccessToken>();
            CreateMap<AccessToken, AccessToken>();
            CreateMap<AccessTokenEntity, AccessTokenEntity>();

            CreateMap<AccessTokenDomain, AccessTokenEntity>()
                .ForMember(dest => dest.Athlete, act => act.Ignore());
            CreateMap<AccessTokenEntity, AccessTokenDomain>()
                .ForMember(dest => dest.Athlete, act => act.Ignore());

            CreateMap<AthleteDomain, AthleteEntity>()
                .ForMember(dest => dest.Activities, act => act.Ignore());
            CreateMap<AthleteEntity, AthleteDomain>();
            //    .ForMember(dest => dest.AccessToken, act => act.Ignore())
            //    .ForMember(dest => dest.Activities, act => act.Ignore());

            CreateMap<ActivityDomain, ActivityEntity>()
                .ForMember(dest => dest.Athlete, act => act.Ignore());
            CreateMap<ActivityEntity, ActivityDomain>();
            //   .ForMember(dest => dest.Athlete, act => act.Ignore());

            CreateMap<ActivitySummary, ActivityEntity>()
                .ForMember(dest => dest.Athlete, act => act.Ignore());

            CreateMap<string, DateTime>().ConvertUsing<StringToDateTimeConverter>();
        }
    }

    public class StringToDateTimeConverter : ITypeConverter<string, DateTime>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            DateTime dateTime;
            if (source == null)
            {
                return default(DateTime);
            }
            if (DateTime.TryParse(source, out dateTime))
            {
                return dateTime;
            }
            return default(DateTime);
        }
    }
}

