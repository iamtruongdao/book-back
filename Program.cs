using BackEnd.helper;
using BackEnd.models;
using BackEnd.services;
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
using BackEnd.Viewmodel;
using System.Text.Json;
using BackEnd.services.Categories;
using System.Text.Json.Serialization;
using BackEnd.services.Email;
using BackEnd.services.VNPay;
using BackEnd.Middleware;
using Serilog;
using BackEnd.Repository;
using BackEnd.services.Ship;
using BackEnd.services.Posts;
using BackEnd.services.Tag;
using BackEnd.hub;
using BackEnd.services.Notifications;
using BackEnd.services.Discounts;
using BackEnd.services.UserDiscounts;
var builder = WebApplication.CreateBuilder(args); 
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
//http client
builder.Services.AddHttpClient("ghn", c =>
{
    c.DefaultRequestHeaders.Add("Token", builder.Configuration["GHN:Token"]);
    c.DefaultRequestHeaders.Add("ShopId", builder.Configuration["GHN:ShopId"]);
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});
// log
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console();
});
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
    options.AddPolicy(MyAllowSpecificOrigins,policy => policy.WithOrigins("http://localhost:3000","http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

//Mapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
//jwtOption setting
builder.Services.Configure<jwtOptions>(builder.Configuration.GetSection("jwtOptions"));
//mongoDb setting
var mongoDbSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSetting>();
builder.Services.Configure<MongoDbSetting>(builder.Configuration.GetSection("MongoDb"));
// Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<MongoDbSetting>(sp => sp.GetRequiredService<IOptions<MongoDbSetting>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(mongoDbSettings?.ConnectionURI));
builder.Services.AddScoped<ErrorMiddleware>();
//SignalR
builder.Services.AddSignalR().AddJsonProtocol().AddHubOptions<NotificationHub>(options =>
    {
        options.EnableDetailedErrors = true;  // Bật chế độ lỗi chi tiết khi cần
    });;
//Authentication
IdentityModelEventSource.ShowPII = true;
builder.Services.AddIdentityMongoDbProvider<User, Role, ObjectId>(
    opt =>
    {
        opt.Password.RequiredLength = 8;
        opt.User.AllowedUserNameCharacters = null!;
        opt.User.RequireUniqueEmail = true;
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
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtOptions:SecretKey"]!)),
    };
    opts.Events = new JwtBearerEvents
    {

        OnChallenge = async context =>
        {
            context.HandleResponse();
            if (context.AuthenticateFailure is SecurityTokenExpiredException expiredException)
            {
                context.Response.StatusCode = StatusCodes.Status410Gone;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    Error = "TokenExpired",
                    Message = "Your token has expired. Please refresh your token.",
                    ExpiredAt = expiredException.Expires
                });
                await context.Response.WriteAsync(result);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    Error = "Unauthorized",
                    Message = "Authentication failed."
                });
                await context.Response.WriteAsync(result);
            }
        }
    };
});

// DI for services
builder.Services.AddHostedService<ConfigureMongoDbIndexesService>();
builder.Services.AddSingleton<ICloundinaryService, CloudinaryService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICartServices, CartService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ICategoriesService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IUserDiscountService, UserDiscountService>();

//repo
builder.Services.AddScoped<IProductRepository, ProductRepo>();
builder.Services.AddScoped<ICartRepository, CartRepo>();
builder.Services.AddScoped<IOrderRepository, OrderRepo>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepo>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepo>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepo>();
builder.Services.AddScoped<IPostRepository, PostRepo>();
builder.Services.AddScoped<ITagRepository, TagRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepo>();
builder.Services.AddScoped<IUserDiscountRepository, UserVoucherRepo>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ErrorMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Course Management V1");
});
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.MapHub<NotificationHub>("/notification").RequireAuthorization();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

