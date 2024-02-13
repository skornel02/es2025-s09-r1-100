using Backend.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public record Floor(int X, int Z, int XLength, int ZLength, string Color);

public class ContainersModel : PageModel
{
    public ApplicationDbContext Context { get; init; }
    public RenderService RenderService { get; init; }

    public ContainersModel(ApplicationDbContext context, RenderService renderService)
    {
        Context = context;
        RenderService = renderService;
    }

    public void OnGet()
    {
    }
}
