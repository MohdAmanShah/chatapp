namespace IviMessageServer.DataModels
{
    public class Payload
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; }
        public int DestinationId { get; set; }
        public int ChatId { get; set; }
        public string Message { get; set; }
    }

}
