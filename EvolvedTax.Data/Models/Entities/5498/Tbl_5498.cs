using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities._5498;

public class Tbl_5498
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

    public decimal? Box1Amount { get; set; }

    public decimal? Box2Amount { get; set; }

    public decimal? Box3Amount { get; set; }

    public decimal? Box4Amount { get; set; }

    public decimal? Box5Amount { get; set; }

    public decimal? Box6Amount { get; set; }

    public string? Box7Checkbox1 { get; set; }

    public string? Box7Checkbox2 { get; set; }

    public string? Box7Checkbox3 { get; set; }

    public string? Box7Checkbox4 { get; set; }

    public decimal? Box8Amount { get; set; }

    public decimal? Box9Amount { get; set; }

    public decimal? Box10Amount { get; set; }

    public string? Box11Checkbox { get; set; }

    public DateTime? Box12aDate { get; set; }

    public decimal? Box12bAmount { get; set; }

    public decimal? Box13aAmount { get; set; }

    public int? Box13bYear { get; set; }

    public string? Box13cCode { get; set; }

    public decimal? Box14aAmount { get; set; }

    public string? Box14bCode { get; set; }

    public decimal? Box15aAmount { get; set; }

    public string? Box15bCode { get; set; }

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