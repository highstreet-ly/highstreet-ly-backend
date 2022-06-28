using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Permissions.Resources
{
    [Resource("reset-password")]
    public class ResetPassword : Identifiable<string>
    {
        [Attr] 
        public string UserId { get; set; }

        [Attr] 
        public string Code { get; set; }

        [Attr] 
        public string Password { get; set; }
    }
}