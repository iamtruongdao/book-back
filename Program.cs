using back.helper;
using back.models;
using back.services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.IdentityModel.Logging;
using MongoDB.Bson;
using back.Viewmodel;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Course Management", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
// cors
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,policy =>{policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();});
});

//Mapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
//jwtOption setting
builder.Services.Configure<jwtOptions>(builder.Configuration.GetSection("jwtOptions"));
//mongoDb setting
var mongoDbSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSetting>();
builder.Services.Configure<MongoDbSetting>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbSetting>(sp => sp.GetRequiredService<IOptions<MongoDbSetting>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(mongoDbSettings?.ConnectionURI));
//Authentication
IdentityModelEventSource.ShowPII = true;
builder.Services.AddIdentityMongoDbProvider<User,Role,ObjectId>(
    identity =>
    {
        identity.Password.RequiredLength = 8;
        // other optio
    },
    mongo =>
    {
        mongo.ConnectionString = mongoDbSettings?.ConnectionURI + mongoDbSettings?.DatabaseName;
        // other options
    }
).AddUserManager<UserManager<User>>().AddRoleManager<RoleManager<Role>>().AddSignInManager<SignInManager<User>>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["jwtOptions:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["jwtOptions:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtOptions:SecretKey"]!)),
    };
    opts.Events = new JwtBearerEvents
    {
      OnAuthenticationFailed = context =>
      {
           if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.StatusCode = StatusCodes.Status410Gone;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsJsonAsync(new
                    {
                        Error = "TokenExpired",
                        Message = "Your token has expired. Please refresh your token.",
                        ExpiredAt = ((SecurityTokenExpiredException)context.Exception).Expires
                    });
                }
          return Task.CompletedTask;
      },
    };
});
// DI for services
builder.Services.AddHostedService<ConfigureMongoDbIndexesService>();
builder.Services.AddSingleton<ICloundinaryService,CloudinaryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICartServices, CartService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// app.Use(async (context, next) =>
// {
//     Console.WriteLine(context.Request.Path);
//     await next();
// });

app.UseSwagger();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Course Management V1");
});

app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

