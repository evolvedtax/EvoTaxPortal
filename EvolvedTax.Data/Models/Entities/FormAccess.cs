using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities;

public partial class FormAccess
{
    [Key]
    public int Id { get; set; }

    public string? Form_Name { get; set; }

    public int? InstituteID { get; set; }

    public DateTime? CreatedDate { get; set; }
}
