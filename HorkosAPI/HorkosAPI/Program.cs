using Microsoft.EntityFrameworkCore;
using HorkosAPI.Auth.Controllers;
using HorkosAPI.Auth.Services;
using HorkosAPI.Badge.Controllers;
using HorkosAPI.Badge.Services;
using HorkosAPI.Comment.Controllers;
using HorkosAPI.Comment.Services;
using HorkosAPI.Database.Services;
using HorkosAPI.Email.Services;
using HorkosAPI.Entity.Controllers;
using HorkosAPI.Entity.Services;
using HorkosAPI.Fact.Controllers;
using HorkosAPI.Fact.Services;
using HorkosAPI.FactCheck.Controllers;
using HorkosAPI.FactCheck.Services;
using HorkosAPI.FeaturedItem.Controllers;
using HorkosAPI.FeaturedItem.Services;
using HorkosAPI.Global;
using HorkosAPI.Group.Controllers;
using HorkosAPI.Group.Services;
using HorkosAPI.Render.Controllers;
using HorkosAPI.Render.Services;
using HorkosAPI.Report.Controllers;
using HorkosAPI.Report.Services;
using HorkosAPI.Role.Controllers;
using HorkosAPI.Role.Services;
using HorkosAPI.Search.Controllers;
using HorkosAPI.Search.Services;
using HorkosAPI.Security.Controllers;
using HorkosAPI.Security.Middleware;
using HorkosAPI.Security.Services;
using HorkosAPI.Source.Controllers;
using HorkosAPI.Source.Services;
using HorkosAPI.User.Controllers;
using HorkosAPI.User.Services;
using HorkosAPI.UserContribution.Controllers;
using HorkosAPI.UserContribution.Services;
using HorkosAPI.Vote.Controllers;
using HorkosAPI.Vote.Services;

var AppOrigin = BaseController.AppOrigin;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["Application:DatabaseConnectionString"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AppOrigin,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                          policy.WithOrigins("https://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                          policy.WithOrigins("https://horkos-front-dev-as.azurewebsites.net").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                          policy.WithOrigins("https://horkos-front-prod-as.azurewebsites.net").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                          policy.WithOrigins("https://horkos.fr").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services
    .AddMemoryCache()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IRoleService, RoleService>()
    .AddScoped<IUserTokenService, UserTokenService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IFactService, FactService>()
    .AddScoped<ICommentService, CommentService>()
    .AddScoped<IEntityService, EntityService>()
    .AddScoped<IFactCheckService, FactCheckService>()
    .AddScoped<ISourceService, SourceService>()
    .AddScoped<IVoteService, VoteService>()
    .AddScoped<IBadgeService, BadgeService>()
    .AddScoped<IReportService, ReportService>()
    .AddScoped<IGroupService, GroupService>()
    .AddScoped<ISearchService, SearchService>()
    .AddScoped<IEmailService, EmailService>()
    .AddScoped<IFeaturedItemService, FeaturedItemService>()
    .AddScoped<IRenderService, RenderService>()
    .AddScoped<IUserContributionService, UserContributionService>()
    .AddScoped<ConfigurationService>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddSingleton(provider => new Supabase.Client(builder.Configuration["Application:SupabaseUrl"] ?? "", builder.Configuration["Application:SupabaseKey"] ?? ""))
    .AddDbContext<DatabaseContext>(options =>
        options.UseNpgsql(connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        })
    );


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.EnsureCreated();
}

Environment.SetEnvironmentVariable("IS_PRODUCTION", app.Environment.IsProduction().ToString() ?? "false");
app.UseCors(AppOrigin);

app.UseHttpsRedirection();

app.UseAuthController()
    .UseRoleController()
    .UseEntityController()
    .UseFactController()
    .UseCommentController()
    .UseGroupController()
    .UseFactCheckController()
    .UseSourceController()
    .UseUserContributionController()
    .UseUserTokenController()
    .UseUserController()
    .UseSearchController()
    .UseVoteController()
    .UseBadgeController()
    .UseFeaturedItemController()
    .UseReportController()
    .UseRenderController()
    .UseDisponibilityController();


app.UseWhen(context => context.Request.Method != "OPTIONS", appBuilder =>
{
    appBuilder.UseMiddleware<UserTokenMiddleware>().UseAuthorization();
});


app.Run();
