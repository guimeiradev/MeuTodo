using System.ComponentModel.DataAnnotations;

namespace MeuTodo.ViewModels
{
    public class UpdateViewModel
    {
        [Required]
        public string Title { get; set; }
    }
}