
using Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;
using MoviesWeb.Server.Contracts.Validators;
using MoviesWeb.Server.MapperProfile;
using Repositories.CommentsRepo;
using Repositories.Contexts;
using Repositories.MovieRepo;
using Repositories.SessionsRepo;
using Services;
using Services.Helpers;
using Services.MapperProfile;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("def")));
builder.Services.AddIdentityCore<User>(options =>
    options.Password.RequireDigit = true
)
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager(); 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddTransient<IMovieRepository, MovieRepository>();
builder.Services.AddTransient<ICommentsRepository, CommentRepository>();
builder.Services.AddTransient<ISessionsRepository, SessionsRepository>();

builder.Services.AddTransient<MovieService>();
builder.Services.AddTransient<IdentityAuthService>();
builder.Services.AddTransient<CommentService>();
builder.Services.AddTransient<SessionsService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddValidatorsFromAssemblyContaining<AddCommentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MovieAddValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateMovieValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MovieParamsValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SessionUpdateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SessionCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SessionParamsValidator>();



builder.Services.AddAutoMapper(opt => {
    opt.AddProfile<CommentMapperProfile>();
    opt.AddProfile<MovieMapperProfile>();
    opt.AddProfile<UserMapperProfile>();
    opt.AddProfile<SessionsMapperProfile>();

    opt.AddProfile<CommentMapperProfileService>();
    opt.AddProfile<MovieMapperProfileService>();
    opt.AddProfile<UserMapperProfileService>();
    opt.AddProfile<SessionMapperProfileService>();



});





var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{    
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Movies"));

}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();
app.UseCors("AllowAngularApp");
app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
