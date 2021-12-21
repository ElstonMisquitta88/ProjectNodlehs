using Microsoft.AspNetCore.Mvc.RazorPages;
using TrekBucketListApp.Model;
using TrekBucketListApp.Service;

namespace TrekBucketListApp.Pages
{
    public class TrekDisplayModel : PageModel
    {
        public ITrekService _ProductService;
        public IEnumerable<ImageList> AllTrekList { get; private set; }

        public string _TrekName { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public TrekDisplayModel(ILogger<IndexModel> logger, ITrekService productService)
        {
            _logger = logger;
            _ProductService = productService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                string page = HttpContext.Request.Query["page"];
                if (page is null)
                {
                    page = "noimg";
                }
                _TrekName = page.ToUpper();
                var shows = await _ProductService.GetTrekImages(page);
                AllTrekList = shows;
           }
            catch
            {
 
            }

        }
    }
}
