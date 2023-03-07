namespace GadgetStoreASPExam.Model
{
    public class UploadFile
    {
        public int? Id { get; set; }
        public IFormFile files { get; set; }
        public string? NameImg { get; set; }

    }
}
