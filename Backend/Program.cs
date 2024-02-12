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

builder.Services.AddOptions<YardOptions>(YardOptions.SectionName);

builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ContainerService>();

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
