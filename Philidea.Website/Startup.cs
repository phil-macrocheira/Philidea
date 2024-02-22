using Microsoft.AspNetCore.Builder;
using Philidea.Website.Models;

namespace Philidea.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options => {
                options.Conventions.AddPageRoute("/home/Index", "");
                options.Conventions.AddPageRoute("/totk-dc/totk-calculator", "/totk-calculator");
                options.Conventions.AddPageRoute("/acgc-se/acgc-save-editor", "/acgc-save-editor");
            });

            services.AddScoped<ImportData>();
            services.AddScoped<CalculateDamage>();
            services.AddScoped<GetFusedName>();

            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddControllers();
            services.AddLogging();
            services.AddHttpContextAccessor();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
            });
        }
    }
}