﻿using AutoMapper;
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
            CreateMap<AnswerDTO, Answer>().ReverseMap();
            CreateMap<Answer, CreateAnswerDto>().ReverseMap();
            CreateMap<Answer, UpdateAnswerDto>().ReverseMap();
            CreateMap<QuestionDTO, Question>().ReverseMap();
            CreateMap<Question, CreateQuestionRequestDto>().ReverseMap();
            CreateMap<Question, UpdateQuestionRequestDto>().ReverseMap();
        }
    }
}