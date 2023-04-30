namespace GadgetStoreASPExam.Model
{
    public class BuyGadget
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int ProductId { get; set; }
        public string NameCard { get; set; }
        public int Month { get; set; }
        public int Years { get; set; }
        public int CVV { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
