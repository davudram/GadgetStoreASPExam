namespace GadgetStoreASPExam.Model
{
    public partial class Gadget
    {
        public int Id { get; set; }

        public int? IdCategory { get; set; }

        public string? IsPremium { get; set; }

        public string? Name { get; set; }

        public double? Price { get; set; }

        public string? Image { get; set; }

        public virtual Category? IdCategoryNavigation { get; set; }
    }
}
