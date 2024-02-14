using System.ComponentModel.DataAnnotations;

namespace IviMessageServer.Data_Models
{
    public class Chat
    {

        public int CreatorId { get; set; }
        public int InvitedId { get; set; }
    }
}
