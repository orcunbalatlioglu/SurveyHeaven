using SurveyHeaven.Application.Mapping;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.DomainService.Repositories;
using WebAPI.Logger;

namespace WebAPI.Extensions
{
    public static class IoCExtensions
    {
        public static IServiceCollection AddInjections(this IServiceCollection services)
        {
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<ISurveyRepository, SurveyRepository>();

            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IAnswerLogManager, AnswerLogManager>();
            services.AddScoped<IUserLogManager, UserLogManager>();
            services.AddScoped<ISurveyLogManager, SurveyLogManager>();

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }

        public static IServiceCollection AddMongoDbSettings(this IServiceCollection services,
           IConfiguration configuration)
        {
            return services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = configuration
                    .GetSection(nameof(MongoDbSettings) + ":" + MongoDbSettings.ConnectionStringValue).Value;
                options.Database = configuration
                    .GetSection(nameof(MongoDbSettings) + ":" + MongoDbSettings.DatabaseValue).Value;
            });
        }

    }
}
