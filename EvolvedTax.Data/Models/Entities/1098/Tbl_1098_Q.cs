using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities._1098;

public class Tbl_1098_Q
{
    [Key]
    public int Id { get; set; }

    public string? RcpTIN { get; set; }

    public string? LastNameCompany { get; set; }

    public string? FirstName { get; set; }

    public string? NameLine2 { get; set; }

    public string? AddressType { get; set; }

    public string? AddressDelivStreet { get; set; }

    public string? AddressAptSuite { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Zip { get; set; }

    public string? Country { get; set; }

    public string? RcpAccount { get; set; }

    public string? RcpEmail { get; set; }

    public string? PlanName { get; set; }

    public string? PlanNo { get; set; }

    public string? PlanSponsorEIN { get; set; }

    public decimal? Box1aAmount { get; set; }

    public DateTime? Box1bDate { get; set; }

    public string? Box2Checkbox { get; set; }

    public decimal? Box3Amount { get; set; }

    public decimal? Box4Amount { get; set; }

    public decimal? Box5aAmount { get; set; }

    public string? Box5aDD { get; set; }

    public decimal? Box5bAmount { get; set; }

    public string? Box5bDD { get; set; }

    public decimal? Box5cAmount { get; set; }

    public string? Box5cDD { get; set; }

    public decimal? Box5dAmount { get; set; }

    public string? Box5dDD { get; set; }

    public decimal? Box5eAmount { get; set; }

    public string? Box5eDD { get; set; }

    public decimal? Box5fAmount { get; set; }

    public string? Box5fDD { get; set; }

    public decimal? Box5gAmount { get; set; }

    public string? Box5gDD { get; set; }

    public decimal? Box5hAmount { get; set; }

    public string? Box5hDD { get; set; }

    public decimal? Box5iAmount { get; set; }

    public string? Box5iDD { get; set; }

    public decimal? Box5jAmount { get; set; }

    public string? Box5jDD { get; set; }

    public decimal? Box5kAmount { get; set; }

    public string? Box5kDD { get; set; }

    public decimal? Box5lAmount { get; set; }

    public string? Box5lDD { get; set; }

    public string? FormCategory { get; set; }

    public string? FormSource { get; set; }

    public string? TaxState { get; set; }

    public string? Uploaded_File { get; set; }

    public int? Status { get; set; }

    public string? Created_By { get; set; }

    public DateTime? Created_Date { get; set; }

    public string? UserId { get; set; }

    public int? InstID { get; set; }

    public int? EntityId { get; set; }

    public bool IsDuplicated { get; set; }

    public string? Province { get; set; }

    public string? PostalCode { get; set; }

}



