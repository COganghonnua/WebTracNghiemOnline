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
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            CreateMap<CreateQuestionDto, Question>()
           .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<UpdateQuestionDto, Question>().ReverseMap();
            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<Answer, AnswerDTO>();
            CreateMap<CreateExamDto, Exam>()
     .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

            CreateMap<UpdateAnswerDto, Answer>();
            CreateMap<Topic, TopicDTO>()
            .ForMember(dest => dest.ListSubjectDTO, opt => opt.MapFrom(src => src.Subjects));
            CreateMap<Exam, ExamDTO>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName)).ReverseMap()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration)).ReverseMap();
            CreateMap<CreateExamDto, Exam>();
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<RegisterUserDto, User>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

            CreateMap<Exam, ExamWithQuestionsDto>()
     .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)))
     .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
     .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration)); // Đảm bảo tồn tại
            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                 .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
            CreateMap<Exam, ExamWithQuestionsDto>()
    .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
    .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)));
            CreateMap<UserOnlineRoom, UserOnlineRoomDto>()
    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<OnlineRoom, OnlineRoomDto>()
    .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName)); // Giả sử OnlineRoom có Topic


            // Mapping cho Exercise
            CreateMap<CreateExerciseDto, Exercise>()
                .ForMember(dest => dest.ExerciseQuestions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<Exercise, SimpleExerciseDto>();
            CreateMap<CreateExerciseQuestionDto, ExerciseQuestion>()
                 .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => src.Explanation ?? string.Empty))
                .ForMember(dest => dest.ExerciseAnswers, opt => opt.MapFrom(src => src.Answers));

            CreateMap<CreateExerciseAnswerDto, ExerciseAnswer>();

            CreateMap<Exercise, ExerciseDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExerciseQuestions));

            CreateMap<ExerciseQuestion, ExerciseQuestionDto>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.ExerciseAnswers));

            CreateMap<ExerciseAnswer, ExerciseAnswerDto>();
            CreateMap<UserAnswerDto, ExerciseAnswer>();

            //notifi
            CreateMap<NotificationRequestDto, Notification>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Post, PostResponseDto>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.FileAttachments))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User.FullName)) // Thông tin người tạo bài viết
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments)); // Danh sách bình luận

            CreateMap<Comment, CommentResponseDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName)); // Người bình luận


            CreateMap<FileAttachment, FileAttachmentDto>();

        }
    }
}