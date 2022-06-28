using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Highstreetly.Infrastructure.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        
        public static bool OwnsResource(this IHttpContextAccessor httpContextAccessor, Guid resourceId) 
        {
            if (httpContextAccessor
                .HttpContext == null)
            {
                return false;
            }
            
            if (httpContextAccessor.HttpContext.User.FindFirstValue("sub") ==resourceId.ToString())
            {
                return true;
            }

            return false;
        }
        
        public static bool OrganisesResource(this IHttpContextAccessor httpContextAccessor, Guid eventOrganiserId) 
        {
            if (httpContextAccessor
                .HttpContext == null)
            {
                return false;
            }

            if (httpContextAccessor.HttpContext.User.FindAll("member-of-eoid").Any(x=> x.Value ==  eventOrganiserId.ToString()))
            {
                return true;
            }

            return false;
        }
        
        public static bool OwnsResource<T>(this IHttpContextAccessor httpContextAccessor, T resource) where T : IHasOwner
        {
            if (httpContextAccessor
                .HttpContext == null)
            {
                return false;
            }
            
            if (httpContextAccessor.HttpContext.User.FindFirstValue("sub") == resource.OwnerId.GetValueOrDefault().ToString())
            {
                return true;
            }

            return false;
        }
        
        public static bool OrganisesResource<T>(this IHttpContextAccessor httpContextAccessor, T resource) where T : IHasEventOrganiser
        {
            if (httpContextAccessor
                .HttpContext == null)
            {
                return false;
            }

            if (httpContextAccessor.HttpContext.User.FindAll("member-of-eoid").Any(x=>x.Value == resource.EventOrganiserId.ToString()))
            {
                return true;
            }

            return false;
        }
        
        public static bool IsAdmin(this IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor
                .HttpContext == null)
            {
                return false;
            }

            var isAdmin = httpContextAccessor
                          .HttpContext
                          .User
                          .FindAll("role")
                          .FirstOrDefault(x => x.Value.ToLower() == "admin") != null;
          
            var isMachine = httpContextAccessor.HttpContext.User.FindFirstValue("access-all") != null;

            return isAdmin || isMachine;
        }
    }
}