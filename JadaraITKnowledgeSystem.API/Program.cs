using AutoMapper;
using JadaraITKnowledgeSystem.API.Middlewares;
using JadaraITKnowledgeSystem.Application;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Infrastructure;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.Generic;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.UnitOfWork;
using JadaraITKnowledgeSystem.Infrastructure.Services.FileMangment;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Storage Service
//builder.Services.AddHttpClient<IStorageService, BunnyStorageService>();

builder.Services.AddHttpClient<IStorageService, BunnyStorageService>();
builder.Services.AddScoped<IFileManager, FileManager>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Register Services
//...... gone

//External Services
//builder.Services.AddScoped<IJwtTokenService,JwtTokenService>();
//builder.Services.AddScoped<IPasswordService, PasswordService>();

// Register Repositories
//builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
//builder.Services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
//builder.Services.AddScoped<ICourseRequirementMappingRepository, CourseRequirementMappingRepository>();
//builder.Services.AddScoped<ICourseRepository, CourseRepository>();
//builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
//builder.Services.AddScoped<IMajorRepository, MajorRepository>();
//builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
//builder.Services.AddScoped<IQuizRepository, QuizRepository>();
//builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
//builder.Services.AddScoped<IUserReactionRepository, UserReactionRepository>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register Generic Service
//builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<,>));

// Register AutoMapper
//builder.Services.AddSingleton<IMapper>(provider =>
//{
//    var config = new MapperConfiguration(cfg =>
//    {
//        cfg.AddProfile<MappingProfile>();
//        // ....
//    });

//    config.AssertConfigurationIsValid();

//    return config.CreateMapper();
//});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //options.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Check if running in Docker or set environment variable
    var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
                   || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker";

    if (!isDocker)
    {
        // HTTPS only when not in Docker
        serverOptions.Listen(IPAddress.Any, 6001, listenOptions =>
        {
            listenOptions.UseHttps(); // Enable HTTPS
        });
    }//////

    // HTTP for all environments
    serverOptions.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

