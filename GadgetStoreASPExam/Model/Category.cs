namespace GadgetStoreASPExam.Model
{
    public partial class Category
    {
        public int Id { get; set; }

        public string? NameGadgets { get; set; }

        public virtual ICollection<Gadget> Gadgets { get; } = new List<Gadget>();
    }
}
