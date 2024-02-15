using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public class ContainersModel : PageModel
{
    public ApplicationDbContext Context { get; init; }

    public ContainersModel(ApplicationDbContext context)
    {
        Context = context;
    }

    public void OnGet()
    {
    }
}
