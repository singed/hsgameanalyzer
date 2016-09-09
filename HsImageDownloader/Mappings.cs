using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HSCore.Entities;

namespace HsImageDownloader
{
    public class MappingsConfiguration : Profile
    {
        public MappingsConfiguration()
        {
            CreateMap<HSCard, HSCardDto>();
            CreateMap<HSCardDto, HSCard>();

            CreateMap<HSDeck, HSDeckDto>();
            CreateMap<HSDeckDto, HSDeck>();

            CreateMap<HSTurn, HSTurnDto>();
            CreateMap<HSTurnDto, HSTurn>();

            CreateMap<HSGame, HSGameDto>();
            CreateMap<HSGameDto, HSGame>();
        }
    }
}
