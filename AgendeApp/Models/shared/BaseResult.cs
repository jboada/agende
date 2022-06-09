namespace AgendeApp.Models.shared
{
    public class BaseResult
    {
        public bool Result { get; set; } = false;
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

    }
}
