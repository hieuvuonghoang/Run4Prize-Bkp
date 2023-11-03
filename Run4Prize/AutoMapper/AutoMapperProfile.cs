using AutoMapper;

namespace Run4Prize.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
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

