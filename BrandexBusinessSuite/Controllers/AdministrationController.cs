namespace BrandexBusinessSuite.Controllers
{
    using Infrastructure;
    
    [AuthorizeAdministrator]
    public abstract class AdministrationController : ApiController
    {
    
    }
}