using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public class ContainersModel : PageModel
{
    public IApplicationDbContext Context { get; init; }

    public ContainersModel(IApplicationDbContext context)
    {
        Context = context;
    }

    public void OnGet()
    {
    }
}
