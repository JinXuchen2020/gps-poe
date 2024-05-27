using Authentication;
using FileService;
using MailService.Interfaces;
using MailService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using POEMgr.Api;
using POEMgr.Application;
using POEMgr.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}";
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidAudience = builder.Configuration["AzureAd:ClientId"]
        };

    }, options => { builder.Configuration.Bind("AzureAd", options); })
    .EnableTokenAcquisitionToCallDownstreamApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    })
    .AddInMemoryTokenCaches();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1",
//        new OpenApiInfo
//        {
//            Title = "POE API - V1",
//            Version = "v1"
//        }
//     );

//    var filePath = Path.Combine(System.AppContext.BaseDirectory, "POEMgr.Api.xml");
//    c.IncludeXmlComments(filePath);
//    c.SchemaFilter<SchemaFilter>();
//});

builder.Services.AddSwaggerGen(opt =>
{
    var instance = builder.Configuration["AzureAd:Instance"];
    var tenantId = builder.Configuration["AzureAd:TenantId"];
    var clientId = builder.Configuration["AzureAd:ClientId"];
    var scope = builder.Configuration["AzureAd:Scopes"];

    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "POE API - V1", Version = "v1" });
    opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.OAuth2,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{instance}{tenantId}/oauth2/v2.0/authorize"),
                Scopes = new Dictionary<string, string>{
                    { $"api://{clientId}/{scope}", "Access as user"},
                },
                TokenUrl = new Uri($"{instance}{tenantId}/oauth2/v2.0/token"),
            }
        }
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="oauth2"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddCors(options => options.AddPolicy("Cors", builder =>
{
    builder.
    AllowAnyOrigin().
    AllowAnyMethod().
    AllowAnyHeader();
}));

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile(new MappingConfiguration());
});

builder.Services.AddRepositoryServices(builder.Configuration["ConnectionStrings:DBServer"]);

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddSingleton<IPrincipalAccessor, PrincipalAccessor>()
    .AddSingleton<IClaimsAccessor, ClaimsAccessor>();

builder.Services.AddTransient<IEmailService>(provider =>
{
    var mail = builder.Configuration["NotificationEmail:Name"].ToString();
    var password = builder.Configuration["NotificationEmail:Password"].ToString();
    return new EmailServiceCore(mail, password);
});

builder.Services.AddScoped<AzureBlobProvider>(provider =>
{
    var connectStr = builder.Configuration["Storage:ConnectionString"].ToString();
    var containerName = builder.Configuration["Storage:ContainerName"].ToString();
    return new AzureBlobProvider(connectStr, containerName);
});

builder.Services.AddSingleton<Appsettings>(provider =>
{
    return new Appsettings
    {
        LoginUrl = builder.Configuration["LoginUrl"].ToString()
    };
});
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();


builder.Services.AddServices();
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("Cors");

app.MapControllers();

app.Run();
