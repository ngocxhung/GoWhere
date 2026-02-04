using System.ComponentModel.DataAnnotations;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class AIChatRequestDto
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }

        public string? ConversationId { get; set; }  // Để theo dõi cuộc hội thoại
    }
}
