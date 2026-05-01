using System.ComponentModel.DataAnnotations;

namespace ECommerce527.ViewModels
{
    public class NewPasswordVM
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password) , Compare(nameof(Password))]
        public string ConfirmedPassword { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
