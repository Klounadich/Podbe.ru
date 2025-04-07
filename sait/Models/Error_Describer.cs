using Microsoft.AspNetCore.Identity;

namespace sait.Models
{
    public class Error_Describer : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Пароль должен содержать хотя бы одну цифру (0-9)!"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"Пароль должен быть не короче {length} символов!"
            };
        }


        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Добавьте строчную букву (a-z)!" 
            };
        }
    }
}