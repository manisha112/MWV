using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

        public string MyProperty { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "The Email field is required!", AllowEmptyStrings = false)]

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "First Name field is required!")]
        public string firstName { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Last Name field is required!")]
        public string lastName { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Company Name field is required!")]
        public string name { get; set; }
        public string address { get; set; }
        public string landline { get; set; }
        public string location { get; set; }
        public string mobile { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string contactperson { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}