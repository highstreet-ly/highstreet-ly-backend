using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Permissions.Resources
{
    [Resource("confirm-email")]
    public class ConfirmEmail : Identifiable<string>
    {
        [Attr] 
        public string UserId { get; set; }

        [Attr] 
        public string Code { get; set; }
    }
}