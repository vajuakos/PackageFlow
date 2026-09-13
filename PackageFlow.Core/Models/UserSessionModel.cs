namespace PackageFlow.Core.Models
{
    public class UserSessionModel
    {
        public AppUser CurrentUser { get; set; }

        public bool IsLoginSucceed { get; set; }
    }
}
