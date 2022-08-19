using JwtTokenAuthentication;
using JwtTokenAuthentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using WebServiceTest.Converters;
using WebServiceTest.Data;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.MaxValidationDepth = 999;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddXmlSerializerFormatters();

builder.Services.AddDistributedMemoryCache();

//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromSeconds(1000);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//builder.Services.AddHttpContextAccessor();

//builder.Services.AddTransient<ISessionRepository, SessionRepository>();

builder.Services.AddSingleton<MyJsonToXmlConverter, JsonToXmlService>();

builder.Services.AddSingleton<MyXmlToJsonConverter, XmlToJsonService>();

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

builder.Services.Configure<JwtDescriptor>(builder.Configuration.GetSection("JwtDescriptor")); //IOptions Pattern

JwtDescriptor info = builder.Configuration.GetSection("JwtDescriptor").Get<JwtDescriptor>(); //Config dosyalarýndan direkt obje çekmek için

var publicKey = info.RsaPublicKey.ToByteArray();

using RSA rsa = RSA.Create();
rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
{
    ValidIssuer = "http://localhost",
    ValidAudience = "http://localhost",
    IssuerSigningKey = new RsaSecurityKey(rsa),
    CryptoProviderFactory = new CryptoProviderFactory()
    {
        CacheSignatureProviders = false
    },
    
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    RequireExpirationTime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = tokenValidationParameters;
});

//builder.Services.AddSingleton<TCPIPListenerBGS>();

//builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//app.UseSession();


app.MapControllers();

app.Run();
