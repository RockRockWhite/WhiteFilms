namespace  WhiteFilms.API.Models
{
    public class Response<T>
    {
        public Response(IError error)
        {
            status = error.errorCode == 0 ? 1 : -1;
            errorCode = error.errorCode;
            errorMsg = error.errorMsg;
        }

        public int status { set; get; }
        public int errorCode { set; get; }
        public string errorMsg { set; get; }
        public T resultBody { set; get; }
    }
}