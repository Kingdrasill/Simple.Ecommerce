using Microsoft.AspNetCore.Http.Features;
using Simple.Ecommerce.Api;
using Simple.Ecommerce.App;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Infra;

var builder = WebApplication.CreateBuilder(args);
{ 
    builder.Services
        .AddApp(builder.Configuration)
        .AddInfra(builder.Configuration)
        .AddPresentation(builder.Configuration)
        .AddControllers();
}

builder.Services.AddCors();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024;
});

var app = builder.Build();

var cacheUse = builder.Configuration.GetSection("UseCache").Get<UseCache>()!;
if (cacheUse.Use) 
{ 
    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<ICacheFrequencyInitializer>();
        await initializer.Initialize();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
