using System.ComponentModel.DataAnnotations;

namespace IviMessageServer.Data_Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int SenderId { get; set; }
        public int RecieverId { get; set; }
        public int ChatId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
