using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subject, SubjectDTO>();
            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();
            CreateMap<Topic, TopicDTO>();
            CreateMap<CreateTopicDto, Topic>();
            CreateMap<UpdateTopicDto, Topic>();
        }
    }
}