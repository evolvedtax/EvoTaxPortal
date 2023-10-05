using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities;

public partial class FormName
{
    [Key]
    public int Id { get; set; }
    public string? Form_Name { get; set; }
}
