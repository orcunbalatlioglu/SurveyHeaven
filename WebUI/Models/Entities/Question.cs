using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.Entities
{
    public class Question
    {
        [Required(ErrorMessage = "Sorunun id'si boş bırakılamaz!")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Sorunun tipi seçilmek zorundadır!")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Soru başlığı boş bırakılamaz!")]
        public string Title { get; set; }
        public List<string>? OptionContent { get; set; }
        [Required(ErrorMessage = "Sorunun zorunlu olup olmadığı seçilmek zorundadır!")]
        public bool IsCompulsory { get; set; }
    }
}
