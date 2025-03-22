using Ecommerce.Data;
using Ecommerce.Helper;
using Ecommerce.Models;
using Ecommerce.Repository.CartService;
using Ecommerce.Repository.GenericService;
using Ecommerce.Repository.NotificationService;
using Ecommerce.Repository.OrderService;
using Ecommerce.Repository.ProductSerivce;
using Ecommerce.Repository.RefundService;
using Ecommerce.Repository.UserService;
using Ecommerce.Repository.WishListService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.
// Add DbContext
builder.Services.AddDbContext<EcommerceDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Lockout.AllowedForNewUsers = false;
})
.AddEntityFrameworkStores<EcommerceDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddAuthorization();
// Configure JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient(typeof(IGenericBasicDataRepo<,>), typeof(GenericBasicDataRepo<,>));
builder.Services.AddTransient<ICartService, CartService>();
builder.Services.AddTransient<IWishListService, WishListService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IRefendService, RefendService>();
builder.Services.AddTransient<IProductSerivce, ProductSerivce>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSignalR();



// Add CORS policy to allow all origins, methods, and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors(x => x
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()
      .SetIsOriginAllowed(origin => true));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
