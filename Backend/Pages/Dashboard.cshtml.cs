using Backend.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public class DashboardModel : PageModel
{
    public ApplicationDbContext Context { get; init; }
    public RenderService RenderService { get; init; }
    public StatisticsService StatisticsService { get; init; }

    public DashboardModel(
        ApplicationDbContext context, 
        RenderService renderService, 
        StatisticsService statisticsService)
    {
        Context = context;
        RenderService = renderService;
        StatisticsService = statisticsService;
    }

    public void OnGet()
    {
    }
}
