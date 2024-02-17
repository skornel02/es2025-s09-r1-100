using Backend;
using Backend.Endpoints;
using Backend.Options;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(_ =>
{

});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

builder.Services.Configure<YardOptions>(builder.Configuration.GetSection(YardOptions.SectionName));
builder.Services.Configure<RenderOptions>(builder.Configuration.GetSection(RenderOptions.SectionName));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<ContainerService>();
builder.Services.AddScoped<RenderService>();
builder.Services.AddScoped<StatisticsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapRazorPages();

app.MapGroup("/api")
    .MapContainerEndpoints()
    .MapImportEndpoints()
    .MapStatisticsEndpoints();

app.Run();
