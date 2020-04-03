using BooksQL.API.GraphQL;
using BooksQL.API.Repositories;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BooksQL.API
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
            services.AddControllers();

            services.AddSingleton<BooksRepository>();
            services.AddSingleton<BookReviewsRepository>();

            // register IDependencyResolver, the DI abstraction from dotnet GraphQL
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            // register the schema, resolved later by IDependencyResolver
            services.AddScoped<BooksSchema>();

            // configure and register all types for GraphQL
            services.AddGraphQL(options =>
            {
#if DEBUG
                options.ExposeExceptions = true;
#else
                options.ExposeExceptions = false;
#endif
            })
                .AddGraphTypes(ServiceLifetime.Scoped);
                //.AddUserContextBuilder(httpContext => httpContext.User)
                //.AddDataLoader();

            // kestrel workaround
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // configure Middleware (path can be specified as a parameter)
            app.UseGraphQL<BooksSchema>();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
        }
    }
}
