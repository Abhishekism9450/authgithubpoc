using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Identity.ExternalClaims.Data;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using dotnet.DataContext;
using System.Linq;
using System.Threading.Tasks;
using dotnet.Service;
using dotnet.Models;
using Microsoft.Data.Sqlite;

using System.Net.Http;
using System.Net.Http.Headers;


namespace dotnet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TokenDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TokenDb")));

            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("fully permissive", configurePolicy => configurePolicy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200").AllowCredentials()); //localhost:4200 is the default port an angular runs in dev mode with ng serve
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultTokenProviders();

            services.AddDbContext<IdentityDbContext>();
            services.AddScoped<DbContext>(sp => sp.GetService<IdentityDbContext>());

            services.AddAuthentication(options =>
            {
                options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddGitHub("Github", options =>
              {
                  options.ClientId = "3fa0685e2551a20c0601";
                  options.ClientSecret = "61f60c6c756016aca9b485aba0ab4c81bbe93504";
                  options.CallbackPath = new PathString("/account/HandleExternalLogin");
                  options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                  options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                  options.ClaimsIssuer = "OAuth2-Github";
                  options.UserInformationEndpoint = "https://api.github.com/user";
                  options.SaveTokens = true;
                  options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                  options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                  options.ClaimActions.MapJsonKey("urn:github:login", "login");
                  options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                  options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
                  options.Events = new OAuthEvents
                  {
                      OnCreatingTicket = async context =>
                         {
                             var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                             request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                             var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                             response.EnsureSuccessStatusCode();

                             var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                             context.RunClaimActions(user);
                             using (var ctx = new TokenDbContext())
                             {
                                 ctx.TokenDb.Add(new Token { Email = "sanjay", access_Token = context.AccessToken });
                                 ctx.SaveChanges();
                             }
                         }
                  };
              });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseCors("fully permissive");
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
}
