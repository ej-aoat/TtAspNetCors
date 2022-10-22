using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MyWebApiServer;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var securitykey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes("266031BF-D467-405D-89CE-564C92403479"));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        // CORS許可(SignalR用)
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// JWTベアラトークンによる認証モジュール有効
builder.Services.AddAuthentication(options =>
{
    // Identity made Cookie authentication the default.
    // However, we want JWT Bearer Auth to be the default.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Configure JWT Bearer Auth to expect our security key
    options.TokenValidationParameters = new TokenValidationParameters
    {
        LifetimeValidator = (before, expires, token, param) =>
        {
            return expires > DateTime.UtcNow;
        },
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateActor = false,
        ValidateLifetime = true,
        IssuerSigningKey = securitykey
    };

    // 必ずOnMessageReceivedをフックしてください。
    // WebSocketが起動したときにクエリ文字列からaccess_tokenを読み込むための
    // JWT認証ハンドラがサーバー送信イベントを要求します。
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // 特定のハブに対してのみ、JWT認証トークンを渡したい場合は
                // 下記のようにリクエストURLのパスをつかって検証します。
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
    };
});
// 認証認可モジュール有効
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
        policy =>
        {
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            policy.RequireClaim(ClaimTypes.NameIdentifier);
        });
});

// SignalRモジュール有効
// ・１つの受信メッセージあたりの受信可能データサイズを設定します。
// ・SignalRのJSONコンバータに、Newtonsoft.JSONを使用します。(未対応)
// ※SignalRサーバーの設定(タイムアウトやキープアライブ等)については、下記URLを参照。
//   https://docs.microsoft.com/ja-jp/aspnet/core/signalr/configuration?view=aspnetcore-6.0&tabs=dotnet
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapHub<FrontendHub>("/front");
});

app.Run();
