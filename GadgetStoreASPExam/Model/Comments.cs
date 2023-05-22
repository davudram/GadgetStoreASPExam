namespace GadgetStoreASPExam.Model
{
    public class Comments
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Stars { get; set; }
        public DateTime CreateAt { get; set; }

    }
}
