using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyVet.Web.Data.Entities
{
    public class Owner
    {
        public int Id { get; set; }

        public User User { get; set; }

        //[Display(Name = "Document")]
        //[MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        //public string Document { get; set; }


        //[Display(Name = "First Name")]
        //[MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        //public string FirstName { get; set; }

        //[Display(Name = "Last Name")]
        //[MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        //public string LastName { get; set; }

        //[Display(Name = "Fixed Phone")]
        //[MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //public string FixedPhone { get; set; }

        //[Display(Name = "Cell Phone")]
        //[MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        //public string CellPhone { get; set; }

        //[MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //public string Address { get; set; }


        //[Display(Name = "Owner Name")]
        //public string FullName => $"{FirstName} {LastName}";

        //[Display(Name = "Full Name With Document")]
        //public string FullNameWithDocument => $"{FirstName} - {LastName} - {Document}";

        //Relations
        public ICollection<Pet> Pets { get; set; }
        public ICollection<Agenda> Agendas { get; set; }
    }
}
