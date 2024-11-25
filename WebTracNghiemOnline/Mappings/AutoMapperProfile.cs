using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Subject, SubjectDTO>()
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.TopicName));
            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();
            CreateMap<Topic, TopicDTO>();
            CreateMap<CreateTopicDto, Topic>();
            CreateMap<UpdateTopicDto, Topic>();
            CreateMap<Question, QuestionDTO>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName));
            CreateMap<CreateQuestionDto, Question>();
            CreateMap<UpdateQuestionDto, Question>();

            CreateMap<Answer, AnswerDTO>();
            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<UpdateAnswerDto, Answer>();
            CreateMap<Topic, TopicDTO>()
            .ForMember(dest => dest.ListSubjectDTO, opt => opt.MapFrom(src => src.Subjects));
            CreateMap<Exam, ExamDTO>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName)).ReverseMap();

            CreateMap<CreateExamDto, Exam>();
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<RegisterUserDto, User>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
        }
    }
}