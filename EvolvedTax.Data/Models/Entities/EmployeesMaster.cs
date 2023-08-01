using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class EmployeesMaster
{
    public int EmpId { get; set; }

    public string? FullName { get; set; }

    public string? JobTitle { get; set; }

    public string? Email { get; set; }

    public string? BusinessContact { get; set; }

    public string? MobileContact { get; set; }

    public string? BusinessAddress { get; set; }

    public string? StateZip { get; set; }

    public string? Country { get; set; }

    public int? Supervisor { get; set; }
}
