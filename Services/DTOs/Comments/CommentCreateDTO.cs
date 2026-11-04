using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Comments
{
    public class CommentCreateDTO
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }
}
