using AutoMapper;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.Application.DTOs.Requests;

namespace SurveyHeaven.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() { 
            //Answer
            CreateMap<Answer,AnswerDisplayResponse>();
            CreateMap<CreateAnswerRequest, Answer>();
            CreateMap<UpdateAnswerRequest, Answer>().ReverseMap();

            //Survey
            CreateMap<Survey, SurveyDisplayResponse>();
            CreateMap<CreateSurveyRequest, Survey>();
            CreateMap<UpdateSurveyRequest, Survey>().ReverseMap();

            //User
            CreateMap<User, UserDisplayResponse>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>().ReverseMap();
        }
    }
}
