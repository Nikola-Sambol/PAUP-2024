using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PAUP_2024.Models
{
    public enum VrstaAktivnosti
    {
        [Display(Name = "Hodanje")]
        Hodanje = 1,

        [Display(Name = "Trčanje")]
        Trčanje = 2,

        [Display(Name = "Biciklizam")]
        Biciklizam = 3,

        [Display(Name = "Plivanje")]
        Plivanje = 4,

        [Display(Name = "Planinarenje")]
        Planinarenje = 5
    }
}