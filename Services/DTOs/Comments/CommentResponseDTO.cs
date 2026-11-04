using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Comments
{
    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Posted { get; set; }
        public string MovieName { get; set; }
        public string UserLogin { get; set; }
    }
}
