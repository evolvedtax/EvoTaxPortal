using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class W8BENERequest : FormRequest
    {
        // New fileds --------------------
        public bool DECheckBox { get; set; }
        public string? _12State { get; set; }
        [MaxLength(50)]
        public string? _12Province { get; set; }
        [MaxLength(9)]
        public string? _12ZipPostCode { get; set; }

        //--------------------------------
        //public int Id { get; set; }
        [MaxLength(40)]
        public string? NameOfOrganization { get; set; }

        //public string? CountryOfIncorporation { get; set; }
        [MaxLength(40)]
        public string? NameOfDiregardedEntity { get; set; }

        //public string? TypeOfEntity { get; set; }

        //public string? FatcaStatus { get; set; }

        //public string? PermanentResidenceAddress { get; set; }

        //public string? City { get; set; }

        //public string? Country { get; set; }

        //public string? MailingAddress { get; set; }

        //public string? MCity { get; set; }

        //public string? MCountry { get; set; }
        [MaxLength(9)]
        public string? UsTin { get; set; }
        [MaxLength(20)]
        public string? Gin { get; set; }

        //public string? ForeignTaxIdentifyingNumber { get; set; }

        public bool FtinCheck { get; set; }

        //public string? ReferenceNumberS { get; set; }

        public string? _11fatcaCb { get; set; }

        [MaxLength(50)]
        public string? _12mailingAddress { get; set; }
        [MaxLength(22)]
        public string? _12City { get; set; }

        public string? _12Country { get; set; }

        public string? _13gin { get; set; }

        public bool _14aCb { get; set; }

        [MaxLength(49)]
        public string? _14aTb { get; set; }

        public bool _14bCb1 { get; set; }

        public string? _14bCb2others { get; set; }

        [MaxLength(41)]
        public string? _14bTb { get; set; }

        public bool _14cCb { get; set; }

        [MaxLength(56)]
        public string? _15cTb1 { get; set; }

        [MaxLength(15)]
        public string? _15cTb2 { get; set; }

        [MaxLength(18)]
        public string? _15cTb3 { get; set; }

        [MaxLength(256)]
        public string? _15cTb4 { get; set; }

        [MaxLength(97)]
        public string? _16tb { get; set; }

        public bool _17cb1 { get; set; }

        public bool _17cb2 { get; set; }

        public bool _18cb { get; set; }

        public bool _19cb { get; set; }

        [MaxLength(97)]
        public string? _20tb { get; set; }

        public bool _21cb { get; set; }

        public bool _22cb { get; set; }

        public bool _23cb { get; set; }

        public bool _24aCb { get; set; }

        public string? _24borcCb { get; set; }

        public bool _24dCb { get; set; }

        public bool _25aCb { get; set; }

        public string? _25bcCb { get; set; }

        public bool _26cb1 { get; set; }

        public string? _26cb2or3 { get; set; }

        public string? _26cb4or5 { get; set; }

        [MaxLength(60)]
        public string? _26tb1 { get; set; }

        [MaxLength(50)]
        public string? _26tb2 { get; set; }

        [MaxLength(31)]
        public string? _26tb3 { get; set; }

        public bool _27cb { get; set; }

        public string? _28aorbCb { get; set; }

        public string? _29cb { get; set; }

        public bool _30cb { get; set; }

        public bool _31cb { get; set; }

        public bool _32cb { get; set; }

        public bool _33cb { get; set; }

        [MaxLength(17)]
        public string? _33tb { get; set; }

        public bool _34cb { get; set; }
        [MaxLength(47)]
        public string? _34tb { get; set; }

        public bool _35cb { get; set; }

        [MaxLength(17)]
        public string? _35tb { get; set; }

        public bool _36cb { get; set; }

        public string? _37aorbCb { get; set; }

        [MaxLength(28)]
        public string? _37aTb { get; set; }

        [MaxLength(26)]
        public string? _37bTb1 { get; set; }

        [MaxLength(52)]
        public string? _37bTb2 { get; set; }

        public bool _38cb { get; set; }

        public bool _39cb { get; set; }

        public bool _40aCb { get; set; }

        public string? _40borcCb { get; set; }

        public bool _41cb { get; set; }

        [MaxLength(97)]
        public string? _42tb { get; set; }

        public bool _43cb { get; set; }

        [MaxLength(35)]
        public string? NameRow1 { get; set; }

        [MaxLength(70)]
        public string? AddressRow1 { get; set; }

        [MaxLength(10)]
        public string? Tinrow1 { get; set; }

        [MaxLength(35)]
        public string? NameRow2 { get; set; }

        [MaxLength(70)]
        public string? AddressRow2 { get; set; }

        [MaxLength(10)]
        public string? Tinrow2 { get; set; }

        [MaxLength(35)]
        public string? NameRow3 { get; set; }

        [MaxLength(70)]
        public string? AddressRow3 { get; set; }

        [MaxLength(10)]
        public string? Tinrow3 { get; set; }

        [MaxLength(35)]
        public string? NameRow4 { get; set; }

        [MaxLength(70)]
        public string? AddressRow4 { get; set; }

        [MaxLength(10)]
        public string? Tinrow4 { get; set; }

        [MaxLength(35)]
        public string? NameRow5 { get; set; }

        [MaxLength(70)]
        public string? AddressRow5 { get; set; }

        [MaxLength(10)]
        public string? Tinrow5 { get; set; }

        [MaxLength(35)]
        public string? NameRow6 { get; set; }

        [MaxLength(70)]
        public string? AddressRow6 { get; set; }

        [MaxLength(10)]
        public string? Tinrow6 { get; set; }

        [MaxLength(35)]
        public string? NameRow7 { get; set; }

        [MaxLength(70)]
        public string? AddressRow7 { get; set; }

        [MaxLength(10)]
        public string? Tin1row7 { get; set; }

        [MaxLength(35)]
        public string? NameRow8 { get; set; }

        [MaxLength(70)]
        public string? AddressRow8 { get; set; }

        [MaxLength(10)]
        public string? Tinrow8 { get; set; }

        [MaxLength(35)]
        public string? NameRow9 { get; set; }

        [MaxLength(70)]
        public string? AddressRow9 { get; set; }

        [MaxLength(10)]
        public string? Tinrow9 { get; set; }

        //public string? PrintNameOfSigner { get; set; }

        //public string? SignatureDateMmDdYyyy { get; set; }

        //public string? UploadedFile { get; set; }

        //public string? Status { get; set; }

        public string? W8beneprintName { get; set; }

        public string? W8beneprintSize { get; set; }

        public string? W8beneentryDate { get; set; }

        public string? W8benefontName { get; set; }

        public string? W8beneemailAddress { get; set; }

        public bool? W8beneonBehalfName { get; set; }
    }
}
