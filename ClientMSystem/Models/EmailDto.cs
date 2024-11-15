using Microsoft.EntityFrameworkCore;

namespace ClientMSystem.Models
{
    [Keyless]
    public class EmailDto
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public string Attachments { get; set; } = string.Empty;
    }
}
