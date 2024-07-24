namespace WebAPI
{
    public class ResultData
    {
        public ResultMessage resultMessage { get; set; } = new ResultMessage();

        public List<List<Dictionary<string, object>>> Data { get; set; } = new List<List<Dictionary<string, object>>>();
    }

    public class ResultMessage
    {
        public bool Msg { get; set; } = false;
        public string MsgText { get; set; } = string.Empty;
    }
}
