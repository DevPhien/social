var builder = WebApplication.CreateBuilder(args);

#region Config host
builder.Services.AddCors(options =>
{
    //options.AddDefaultPolicy(builder =>
    //{
    //    builder.AllowAnyOrigin()
    //           .AllowAnyHeader()
    //           .AllowAnyMethod();
    //});
    options.AddPolicy("DevelopmentPolicy", builder =>
    {
        builder
           .WithOrigins("http://localhost:6789", "http://localhost:8080")
           .AllowCredentials()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder
           .WithOrigins("http://localhost:6789", "http://localhost:8080")
           .AllowCredentials()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
#endregion

#region Swagger
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });
    x.SupportNonNullableReferenceTypes();
    x.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Social dotnet", Version = "v1" });
    x.CustomSchemaIds(y => y.FullName);
    x.DocInclusionPredicate((version, apiDescription) => true);
    x.TagActionsBy(y => new List<string>
    {
        y.GroupName ?? "Orther"
    });
});
#endregion

Type[] types = {
    typeof(ISocialHandler),
    typeof(IFacebookHandler),
    typeof(IInstagramHandler),
    typeof(IYoutubeHandler),
};

builder.Services.Scan(scan => scan
                .FromAssembliesOf(types)
                .AddClasses(classes => classes.AssignableToAny(types))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

#region connect database
builder.Services.AddDbContext<SocialContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:SocialMSSql"]));
#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetService<SocialContext>();
    context?.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    //app.UseCors();
    app.UseCors("DevelopmentPolicy");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
    app.UseCors("ProductionPolicy");
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Portals")),
    RequestPath = "/Portals",
    EnableDirectoryBrowsing = false,
});

app.Run();
