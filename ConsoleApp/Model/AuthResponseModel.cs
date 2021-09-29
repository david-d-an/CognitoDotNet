namespace ConsoleApp.Model
{
    public class AuthResponseModel : BaseResponseModel
    {
        public string EmailAddress { get; internal set; }
        public string UserId { get; internal set; }
        internal TokenModel Tokens { get; set; }
    }
}