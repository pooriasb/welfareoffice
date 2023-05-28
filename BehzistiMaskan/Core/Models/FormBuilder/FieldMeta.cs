namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class FieldMeta
    {
        public int Id { get; set; }

        public int FieldId { get; set; }

        public Field Field { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}