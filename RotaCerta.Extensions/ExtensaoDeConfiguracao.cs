using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RotaCerta.Application.MotoristaHandler.Commands.CriarMotorista;
using RotaCerta.Domain.Common;
using RotaCerta.Domain.Common.Interfaces;
using RotaCerta.Domain.Services;
using RotaCerta.Domain.Services.IAuthService;
using RotaCerta.Infraestructure.Context;
using RotaCerta.Infraestructure.Context.Identity;
using RotaCerta.Infraestructure.DomainEvents;
using RotaCerta.Infraestructure.Repository;
using RotaCerta.Infraestructure.Services.AuthService;
using RotaCerta.Infraestructure.Services.AuthService.TokenGeracao;
using RotaCerta.Infrastructure.Repositories;
using RotaCerta.Infraestructure.Storage;
using RotaCerta.Infrastructure.Repository.Storage;
using System.Text;

namespace RotaCerta.Extensions;

public static class ExtensaoDeConfiguracao
{
    public static IServiceCollection AddContextAppRotaCerta(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbRotaCertaContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DbRotaCertaContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddInjectionsDepedency(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICreateUser, CreateUser>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICreateRole, CreateRole>();
        services.AddScoped<ILoginUser, LoginUser>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAddRole, AddRole>();
        services.AddScoped<IMotoristaRepository, MotoristaRepository>();
        services.AddScoped<IEntregaRepository, EntregaRepository>();
        services.AddScoped<IViagemRepository, ViagemRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<DomainEventDispatcher>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CriarMotoristaCommand).Assembly));

        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioContext, UsuarioContext>();

        return services;
    }

    public static IServiceCollection AddJwtAuthetication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new ArgumentException("SecretKey JWT não configurada.");

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.SaveToken = true;
            opt.RequireHttpsMetadata = false;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = configuration["Jwt:ValidAudience"],
                ValidIssuer = configuration["Jwt:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        return services;
    }

    public static IServiceCollection AddSupabaseStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SupabaseStorageOptions>(configuration.GetSection("SupabaseStorage"));
        services.AddHttpClient<IImagemStorageService, SupabaseImagemStorageService>();
        return services;
    }
}
