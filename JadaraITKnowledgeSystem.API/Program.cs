using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Application.Interfaces.Services.Generic;
using JadaraITKnowledgeSystem.Application.Mappings;
using JadaraITKnowledgeSystem.Application.Services;
using JadaraITKnowledgeSystem.Application.Services.Generic;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Repositories;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.Generic;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.UnitOfWork;
using JadaraITKnowledgeSystem.Infrastructure.Services.JWT;
using JadaraITKnowledgeSystem.Infrastructure.Services.Password;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Storage Service
builder.Services.AddHttpClient<IStorageService, BunnyStorageService>();

// Register Services
builder.Services.AddScoped<IChoiceService,ChoiceService>();
builder.Services.AddScoped<ICourseMaterialService,CourseMaterialService>();
builder.Services.AddScoped<ICourseRequirementMappingService,CourseRequirementMappingService>();
builder.Services.AddScoped<ICourseService,CourseService>();
builder.Services.AddScoped<IFacultyService,FacultyService>();
builder.Services.AddScoped<IMajorService,MajorService>();
builder.Services.AddScoped<IQuestionService,QuestionService>();
builder.Services.AddScoped<IQuizService,QuizService>();
builder.Services.AddScoped<IQuizAttemptService,QuizAttemptService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IUniversityService,UniversityService>();
builder.Services.AddScoped<IUserReactionService,UserReactionService>();

//External Services
builder.Services.AddScoped<IJwtTokenService,JwtTokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Register Repositories
builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
builder.Services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
builder.Services.AddScoped<ICourseRequirementMappingRepository, CourseRequirementMappingRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IMajorRepository, MajorRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<IUserReactionRepository, UserReactionRepository>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register Generic Service
//builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<,>));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
