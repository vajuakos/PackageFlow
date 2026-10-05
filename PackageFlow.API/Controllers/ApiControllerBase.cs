using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Extensions;

namespace PackageFlow.API.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int? CurrentUserId => User.GetUserId();
    }
}
