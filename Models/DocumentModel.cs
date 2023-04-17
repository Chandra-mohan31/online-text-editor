namespace Online_Text_editor.Models
{
    public class DocumentModel
    {
        public int DocumentId { get; set; }

        public string DocumentName { get; set; }


        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public int UserId { get; set; }


    }
}
