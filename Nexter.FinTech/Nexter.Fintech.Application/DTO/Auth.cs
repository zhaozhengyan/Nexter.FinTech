namespace Nexter.Fintech.Application
{
    public class Auth
    {
        public string NickName { get; set; }
        public string Code { get; set; }
        public string Avatar { get; set; }
        /// <summary>
        /// 邀请人Token
        /// </summary>
        public string InviterId { get; set; }
    }
}