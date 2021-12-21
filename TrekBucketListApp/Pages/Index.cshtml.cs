using Microsoft.AspNetCore.Mvc.RazorPages;
namespace TrekBucketListApp.Pages
{

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
        }

    }

}