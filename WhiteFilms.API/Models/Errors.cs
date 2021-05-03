namespace  WhiteFilms.API.Models
{
    public interface IError
    {
        public int errorCode { set; get; }
        public string errorMsg { set; get; }
    }

    public class Ok : IError
    {
        public Ok()
        {
            errorCode = 0;
            errorMsg = "";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }

    public class UsernameExistedError : IError
    {
        public UsernameExistedError()
        {
            errorCode = 101;
            errorMsg = "username existed.";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }

    public class UsernameOrPasswordError : IError
    {
        public UsernameOrPasswordError()
        {
            errorCode = 102;
            errorMsg = "username or password is not correct.";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }


    public class InvalidToken : IError
    {
        public InvalidToken()
        {
            errorCode = 103;
            errorMsg = "invalid token.";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }

    public class TallybookIdError : IError
    {
        public TallybookIdError()
        {
            errorCode = 201;
            errorMsg = "Tallybook is not existed.";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }

    public class PermissionDeniedError : IError
    {
        public PermissionDeniedError()
        {
            errorCode = 202;
            errorMsg = "Permission Denied.";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }

    public class TallybookMustHaveOnlyOwnerError : IError
    {
        public TallybookMustHaveOnlyOwnerError()
        {
            errorCode = 203;
            errorMsg = "Tallybook must have only owner.";
        }

        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }
    
}