using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Schema;

namespace Backend.Pages;

public class ContainersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ContainersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {

    }

    public Task<List<ContainerSchema>> Containers => _context.GetContainersAsync().ContinueWith(_ => _.Result.ToList());
}
