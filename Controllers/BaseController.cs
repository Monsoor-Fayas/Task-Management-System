using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        private Guid LoggedInUserInfoId { get; set; }

        public BaseController(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            _userManager = userManager;
            ValidateUser();
        }

        private void ValidateUser()
        {
            try
            {
                if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
                {
                    var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        LoggedInUserInfoId = Guid.Parse(userIdClaim.Value);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Guid LoggedInUserId
        {
            get
            {

                return LoggedInUserInfoId;
                //return Guid.Parse("DC7A344B-3925-4694-B0FD-F4A99CD0F275");
            }
            set
            {
                LoggedInUserInfoId = value;
            }
        }
    }
}
