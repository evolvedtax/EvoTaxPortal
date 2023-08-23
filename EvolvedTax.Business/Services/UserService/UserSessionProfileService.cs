using EvolvedTax.Data.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace EvolvedTax.Business.Services.SessionProfileUser
{
    public class UserSessionProfileService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserSessionProfileService(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
        }

        public UserSessionProfileDTO GetUserModel()
        {

            UserSessionProfileDTO ob = new UserSessionProfileDTO();
            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null && httpContextAccessor.HttpContext.User.Identity != null && httpContextAccessor.HttpContext.User.Claims != null)
            {
                ob.UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                ob.UserRole = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserRole")?.Value;
                ob.FirstName = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "FirstName")?.Value;
                ob.LastName = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "LastName")?.Value;
                ob.UserName = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserName")?.Value;
                ob.InstituteId = Convert.ToInt16(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "InstituteId")?.Value == "" ? 0.ToString() : httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "InstituteId")?.Value);
                return ob;
            }
            return ob;
        }

        public bool IsClaimExists(string claimName)
        {

            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null && httpContextAccessor.HttpContext.User.Identity != null && httpContextAccessor.HttpContext.User.Claims != null)
            {
                return httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type == claimName);
            }
            return false;
        }
    }
}
