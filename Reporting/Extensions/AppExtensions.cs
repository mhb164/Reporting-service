using Tizpusoft.Auth;
using Tizpusoft.Reporting.Middleware;

namespace Tizpusoft.Reporting;

public static class AppExtensions
{
    public static IApplicationBuilder Prepare(this IApplicationBuilder app)
    {
        app.UseCors(builder => builder
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());

        // Configure the HTTP request pipeline.1
        app.UseHttpsRedirection();
        app.UseMiddleware<AuthMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }
}

