using System.Threading.Tasks;
using AutoMapper;
using HSCore.Entities;

namespace HSGameAnalyzer.Mappings
{
    public class MappingsConfiguration : Profile
    {
        public MappingsConfiguration()
        {
            CreateMap<HSCard, HSCardDto>();
            CreateMap<HSCardDto, HSCard>();

            CreateMap<HSDeck, HSDeckDto>();
            CreateMap<HSDeckDto, HSDeck>();
        }
    }
}
