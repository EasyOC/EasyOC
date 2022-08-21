namespace EasyOC.OpenApi.Dto
{
    public class ResetUserPasswordtInput
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string PasswordConfirmation { get; set; }

        public string ResetToken { get; set; }
    }
}
