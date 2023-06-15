using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CloudCorp.Models
{
    public class ContactModel
    {
        [Required, Display(Name = "Sender Name")]
        public string SenderName { get; set; }
        [Required, Display(Name = "Sender Email"), EmailAddress]
        public string SenderEmail { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
