using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Permissions.Resources
{
    [Resource("forgot-password")]
    public class ForgotPassword : Identifiable<string>
    {
        [Attr] public string Email { get; set; }
        [Attr] public string Redirect { get; set; }
    }
}