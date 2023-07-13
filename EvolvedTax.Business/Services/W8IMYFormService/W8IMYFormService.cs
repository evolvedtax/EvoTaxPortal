using Azure;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.W8IMYFormService
{
    public class W8IMYFormService : IW8IMYFormService
    {

        readonly EvolvedtaxContext _evolvedtaxContext;

        public W8IMYFormService(EvolvedtaxContext evolvedtaxContext)
        {

            _evolvedtaxContext = evolvedtaxContext;
        }
        public FormRequest? GetDataByClientEmail(string ClientEmail)
        {
            var query = from gq in _evolvedtaxContext.GeneralQuestionEntities
                        join w8 in _evolvedtaxContext.TblW8imyforms on gq.UserName equals w8.EmailAddress
                        where gq.UserName == ClientEmail
                        select new FormRequest
                        {
                            GQOrgName = gq.OrgName ?? string.Empty,
                            EntityType = gq.EntityType ?? string.Empty,
                            Ccountry = gq.Ccountry ?? string.Empty,
                            MAddress1 = gq.MailingAddress1 ?? string.Empty,
                            MAddress2 = gq.MailingAddress2 ?? string.Empty,
                            MCity = gq.MailingCity ?? string.Empty,
                            MCountry = gq.MailingCountry ?? string.Empty,
                            MProvince = gq.MailingProvince ?? string.Empty,
                            MState = gq.MailingState ?? string.Empty,
                            MZipCode = gq.MailingZip ?? string.Empty,
                            PAddress1 = gq.PermanentAddress1 ?? string.Empty,
                            PAddress2 = gq.PermanentAddress2 ?? string.Empty,
                            PCity = gq.PermanentCity ?? string.Empty,
                            PCountry = gq.PermanentCountry ?? string.Empty,
                            PProvince = gq.PermanentProvince ?? string.Empty,
                            PState = gq.PermanentState ?? string.Empty,
                            PZipCode = gq.PermanentZip ?? string.Empty,
                            UserName = gq.UserName ?? string.Empty,
                            DE = (bool)gq.De,
                            DEOwnerName = gq.DeownerName ?? string.Empty,
                            FormType = gq.FormType ?? string.Empty,
                            W8FormType = gq.W8formType ?? string.Empty,


                            // Add the fields from TblW8expforms

                            W8IMYOnBehalfName= w8.OnBehalfName,
                            CountryOfIncorporation = w8.CountryOfIncorporation,
                            FatcaStatus = w8.FatcaStatus ?? string.Empty,
                            PrintNameOfSignerW8IMY = w8.PrintNameOfSigner ?? string.Empty,
                            US_TIN_CB = w8.UsTinCb,
                            US_TIN = w8.UsTin ?? string.Empty,
                            GIN = w8.Gin,
                            LegallyRequired = (bool)w8.ForeignNumberCb,
                            ForeignTaxIdentifyingNumber = w8.ForeignTaxIdentifyingNumber,
                            Referencenumber = w8.ReferenceNumberS,

                            W8EXPFatca = w8._11fatcaCb ?? string.Empty,
                            _12_Country = w8._12Country ?? string.Empty,
                            _12Mailing_address = w8._12mailingAddress ?? string.Empty,
                            _12_City = w8._12City ?? string.Empty,
                            _12_State = w8._12State ?? string.Empty,
                            _12_Province = w8._12Province ?? string.Empty,
                            _13GIN = w8._13gin ?? string.Empty,

                            _14_CB = (bool)w8._14Cb,
                            _15a = (bool)w8._15a,
                            _15b = (bool)w8._15b,
                            _15c = (bool)w8._15c,
                            _15d = (bool)w8._15d,
                            _15e = (bool)w8._15e,
                            _15f = (bool)w8._15f,
                            _15g = (bool)w8._15g,
                            _15h = (bool)w8._15h,
                            _15i = (bool)w8._15i,
                            _16a = (bool)w8._16a,
                            _16b = w8._16b,
                            _17a = (bool)w8._17a,
                            _17b = (bool)w8._17b,
                            _17c = (bool)w8._17c,
                            _17d = (bool)w8._17d,
                            _17e = (bool)w8._17e,
                            _18a = (bool)w8._18a,
                            _18b = (bool)w8._18b,
                            _18c = (bool)w8._18c,
                            _18d = (bool)w8._18d,
                            _18e = (bool)w8._18e,
                            _18f = (bool)w8._18f,
                            _19a = (bool)w8._19a,
                            _19b = (bool)w8._19b,
                            _19c = (bool)w8._19c,
                            _19d = (bool)w8._19d,
                            _19e = (bool)w8._19e,
                            _19f = (bool)w8._19f,
                            _20 = (bool)w8._20,
                            _21a = (bool)w8._21a,
                            _21b = (bool)w8._21b,
                            _21c = (bool)w8._21c,
                            _21d = (bool)w8._21d,
                            _21e = (bool)w8._21e,
                            _21f = (bool)w8._21f,
                            _22 = (bool)w8._22,
                            _23a_text = w8._23aText,
                            _23b = (bool)w8._23b,
                            _23c = (bool)w8._23c,
                            _24a = (bool)w8._24a,
                            _24b = (bool)w8._24b,
                            _24c = (bool)w8._24c,
                            _25 = (bool)w8._25,
                            _26 = (bool)w8._26,
                            _27a_text = w8._27aText,
                            _27b = (bool)w8._27b,
                            _28 = (bool)w8._28,
                            _29 = (bool)w8._29,
                            _30a = (bool)w8._30a,
                            _30b = (bool)w8._30b,
                            _30c = (bool)w8._30c,
                            _31 = (bool)w8._31,
                            _32_CB1 = (bool)w8._32Cb1,
                            _32_Text2 = w8._32Text2 ?? string.Empty,
                            _32_Text3 = w8._32Text3 ?? string.Empty,
                            _32_CB2 = w8._32Cb2 ?? string.Empty,
                            _32_CB3 = w8._32Cb3 ?? string.Empty,
                            _33a = (bool)w8._33a,
                            _33b = (bool)w8._33b,
                            _33c = (bool)w8._33c,
                            _33d = (bool)w8._33d,
                            _33e = (bool)w8._33e,
                            _33f = (bool)w8._33f,
                            _34 = (bool)w8._34,
                            _35 = (bool)w8._35Cb,
                            _35_Text = w8._35Text,
                            _36 = (bool)w8._36,
                            _36_Text = w8._36Text,
                            _37a = (bool)w8._37aCb,
                            _37a_Text = w8._37aText ?? string.Empty,
                            _37b = (bool)w8._37bCb,
                            _37b_Text1 = w8._37bText1 ?? string.Empty,
                            _37b_Text2 = w8._37bText2 ?? string.Empty,
                            _38 = (bool)w8._38,
                            _39 = (bool)w8._39,
                            _40 = (bool)w8._40,
                            _41_Text = w8._41Text ?? string.Empty,
                            _42 = (bool)w8._42Cb,
                            EmailId = w8.EmailAddress,
                            //hard code for testing
                            activeTabIndex=w8.ActiveTabIndex,

                        };

            return query.FirstOrDefault();
        }
        public int SavePartial(FormRequest request)
        {
            if (_evolvedtaxContext.TblW8imyforms.Any(p => p.EmailAddress == request.EmailId))
            {
                return UpdatePartial(request);
            }
            var model = new TblW8imyform
            {
                NameOfOrganization = request.GQOrgName,
                CountryOfIncorporation = request.CountryOfIncorporation,
                UsTinCb = request.US_TIN_CB,
                UsTin = request.US_TIN,
                Gin = request.GIN,
                ForeignNumberCb = request.LegallyRequired,
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber,
                ReferenceNumberS = request.Referencenumber,
                De = request.DE,
                NameOfDiregardedEntity = request.DEOwnerName,
                _11fatcaCb = request.W8EXPFatca,
                _12Country = request._12_Country,
                _12mailingAddress = request._12Mailing_address,
                _12City = request._12_City,
                _12State = request._12_State,
                _12Province = request._12_Province,
                _13gin = request._13GIN,

                FatcaStatus = request.FatcaStatus,
                TypeOfEntity = request.EntityType,

                _14Cb = request._14_CB,
                _15a = request._15a,
                _15b = request._15b,
                _15c = request._15c,
                _15d = request._15d,
                _15e = request._15e,
                _15f = request._15f,
                _15g = request._15g,
                _15h = request._15h,
                _15i = request._15i,
                _16a = request._16a,
                _16b = request._16b,
                _17a = request._17a,
                _17b = request._17b,
                _17c = request._17c,
                _17d = request._17d,
                _17e = request._17e,
                _18a = request._18a,
                _18b = request._18b,
                _18c = request._18c,
                _18d = request._18d,
                _18e = request._18e,
                _18f = request._18f,
                _19a = request._19a,
                _19b = request._19b,
                _19c = request._19c,
                _19d = request._19d,
                _19e = request._19e,
                _19f = request._19f,
                _20 = request._20,
                _21a = request._21a,
                _21b = request._21b,
                _21c = request._21c,
                _21d = request._21d,
                _21e = request._21e,
                _21f = request._21f,
                _22 = request._22,
                _23aText = request._23a_text,
                _23b = request._23b,
                _23c = request._23c,
                _24a = request._24a,
                _24b = request._24b,
                _24c = request._24c,
                _25 = request._25,
                _26 = request._26,
                _27aText = request._27a_text,
                _27b = request._27b,
                _28 = request._28,
                _29 = request._29,
                _30a = request._30a,
                _30b = request._30b,
                _30c = request._30c,
                _31 = request._31,
                _32Text1 = request._32_Text1,
                _32Cb1 = request._32_CB1,
                _32Text2 = request._32_Text2,
                _32Text3 = request._32_Text3,
                _32Cb2 = request._32_CB2,
                _32Cb3 = request._32_CB3,
                _33a = request._33a,
                _33b = request._33b,
                _33c = request._33c,
                _33d = request._33d,
                _33e = request._33e,
                _33f = request._33f,
                _34 = request._34,
                _35Cb = request._35,
                _35Text = request._35_Text,
                _36 = request._36,
                _36Text = request._36_Text,
                _37aCb = request._37a,
                _37aText = request._37a_Text,
                _37bCb = request._37b,
                _37bText1 = request._37b_Text1,
                _37bText2 = request._37b_Text2,
                _38 = request._38,
                _39 = request._39,
                _40 = request._40,
                _41Text = request._41_Text,
                _42Cb = request._42,
                EmailAddress = request.EmailId,
                PrintNameOfSigner = request.PrintNameOfSignerW8IMY,
                OnBehalfName = request.W8IMYOnBehalfName,
                ActiveTabIndex = request.activeTabIndex


            };

            _evolvedtaxContext.TblW8imyforms.Add(model);
            _evolvedtaxContext.SaveChanges();

            request.W8ExpId = model.Id;
            return model.Id;
        }

        public string Save(FormRequest request)
        {

            if (_evolvedtaxContext.TblW8imyforms.Any(p => p.EmailAddress == request.EmailId))
            {
                return Update(request);
            }
            var model = new TblW8imyform
            {
                NameOfOrganization = request.GQOrgName,
                CountryOfIncorporation = request.CountryOfIncorporation,
                UsTinCb = request.US_TIN_CB,
                UsTin = request.US_TIN,
                Gin = request.GIN,
                ForeignNumberCb = request.LegallyRequired,
                ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber,
                ReferenceNumberS = request.Referencenumber,
                De = request.DE,
                NameOfDiregardedEntity = request.DEOwnerName,
                _11fatcaCb = request.W8EXPFatca,
                _12Country = request._12_Country,
                _12mailingAddress = request._12Mailing_address,
                _12City = request._12_City,
                _12State = request._12_State,
                _12Province = request._12_Province,
                _13gin = request._13GIN,

                FatcaStatus = request.FatcaStatus,
                TypeOfEntity = request.EntityType,

                _14Cb = request._14_CB,
                _15a = request._15a,
                _15b = request._15b,
                _15c = request._15c,
                _15d = request._15d,
                _15e = request._15e,
                _15f = request._15f,
                _15g = request._15g,
                _15h = request._15h,
                _15i = request._15i,
                _16a = request._16a,
                _16b = request._16b,
                _17a = request._17a,
                _17b = request._17b,
                _17c = request._17c,
                _17d = request._17d,
                _17e = request._17e,
                _18a = request._18a,
                _18b = request._18b,
                _18c = request._18c,
                _18d = request._18d,
                _18e = request._18e,
                _18f = request._18f,
                _19a = request._19a,
                _19b = request._19b,
                _19c = request._19c,
                _19d = request._19d,
                _19e = request._19e,
                _19f = request._19f,
                _20 = request._20,
                _21a = request._21a,
                _21b = request._21b,
                _21c = request._21c,
                _21d = request._21d,
                _21e = request._21e,
                _21f = request._21f,
                _22 = request._22,
                _23aText = request._23a_text,
                _23b = request._23b,
                _23c = request._23c,
                _24a = request._24a,
                _24b = request._24b,
                _24c = request._24c,
                _25 = request._25,
                _26 = request._26,
                _27aText = request._27a_text,
                _27b = request._27b,
                _28 = request._28,
                _29 = request._29,
                _30a = request._30a,
                _30b = request._30b,
                _30c = request._30c,
                _31 = request._31,
                _32Text1 = request._32_Text1,
                _32Cb1 = request._32_CB1,
                _32Text2 = request._32_Text2,
                _32Text3 = request._32_Text3,
                _32Cb2 = request._32_CB2,
                _32Cb3 = request._32_CB3,
                _33a = request._33a,
                _33b = request._33b,
                _33c = request._33c,
                _33d = request._33d,
                _33e = request._33e,
                _33f = request._33f,
                _34 = request._34,
                _35Cb = request._35,
                _35Text = request._35_Text,
                _36 = request._36,
                _36Text = request._36_Text,
                _37aCb = request._37a,
                _37aText = request._37a_Text,
                _37bCb = request._37b,
                _37bText1 = request._37b_Text1,
                _37bText2 = request._37b_Text2,
                _38 = request._38,
                _39 = request._39,
                _40 = request._40,
                _41Text = request._41_Text,
                _42Cb = request._42,
                EmailAddress = request.EmailId,
                PrintNameOfSigner = request.PrintNameOfSignerW8IMY,
                OnBehalfName=request.W8IMYOnBehalfName,
                FilePath= request.TemplateFilePath


            };

            _evolvedtaxContext.TblW8imyforms.Add(model);
            _evolvedtaxContext.SaveChanges();

            request.W8ExpId = model.Id;
            return W8IMYCreationEXP(request);




        }

        public string Update(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8imyforms.FirstOrDefault(p => p.EmailAddress == request.EmailId);
            if (response != null)
            {
                request.W8ExpId = response.Id;
                response.NameOfOrganization = request.GQOrgName;
                response.CountryOfIncorporation = request.CountryOfIncorporation;
                response.UsTinCb = request.US_TIN_CB;
                response.UsTin = request.US_TIN;
                response.Gin = request.GIN;
                response.ForeignNumberCb = request.LegallyRequired;
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber;
                response.ReferenceNumberS = request.Referencenumber;
                response.De = request.DE;
                response.NameOfDiregardedEntity = request.DEOwnerName;
                response._11fatcaCb = request.W8EXPFatca;
                response._12Country = request._12_Country;
                response._12mailingAddress = request._12Mailing_address;
                response._12City = request._12_City;
                response._12State = request._12_State;
                response._12Province = request._12_Province;
                response._13gin = request._13GIN;
                response.FatcaStatus = request.FatcaStatus;
                response.TypeOfEntity = request.EntityType;
                response._14Cb = request._14_CB;
                response._15a = request._15a;
                response._15b = request._15b;
                response._15c = request._15c;
                response._15d = request._15d;
                response._15e = request._15e;
                response._15f = request._15f;
                response._15g = request._15g;
                response._15h = request._15h;
                response._15i = request._15i;
                response._16a = request._16a;
                response._16b = request._16b;
                response._17a = request._17a;
                response._17b = request._17b;
                response._17c = request._17c;
                response._17d = request._17d;
                response._17e = request._17e;
                response._18a = request._18a;
                response._18b = request._18b;
                response._18c = request._18c;
                response._18d = request._18d;
                response._18e = request._18e;
                response._18f = request._18f;
                response._19a = request._19a;
                response._19b = request._19b;
                response._19c = request._19c;
                response._19d = request._19d;
                response._19e = request._19e;
                response._19f = request._19f;
                response._20 = request._20;
                response._21a = request._21a;
                response._21b = request._21b;
                response._21c = request._21c;
                response._21d = request._21d;
                response._21e = request._21e;
                response._21f = request._21f;
                response._22 = request._22;
                response._23aText = request._23a_text;
                response._23b = request._23b;
                response._23c = request._23c;
                response._24a = request._24a;
                response._24b = request._24b;
                response._24c = request._24c;
                response._25 = request._25;
                response._26 = request._26;
                response._27aText = request._27a_text;
                response._27b = request._27b;
                response._28 = request._28;
                response._29 = request._29;
                response._30a = request._30a;
                response._30b = request._30b;
                response._30c = request._30c;
                response._31 = request._31;
                response._32Text1 = request._32_Text1;
                response._32Cb1 = request._32_CB1;
                response._32Text2 = request._32_Text2;
                response._32Text3 = request._32_Text3;
                response._32Cb2 = request._32_CB2;
                response._32Cb3 = request._32_CB3;
                response._33a = request._33a;
                response._33b = request._33b;
                response._33c = request._33c;
                response._33d = request._33d;
                response._33e = request._33e;
                response._33f = request._33f;
                response._34 = request._34;
                response._35Cb = request._35;
                response._35Text = request._35_Text;
                response._36 = request._36;
                response._36Text = request._36_Text;
                response._37aCb = request._37a;
                response._37aText = request._37a_Text;
                response._37bCb = request._37b;
                response._37bText1 = request._37b_Text1;
                response._37bText2 = request._37b_Text2;
                response._38 = request._38;
                response._39 = request._39;
                response._40 = request._40;
                response._41Text = request._41_Text;
                response._42Cb = request._42;
                response.EmailAddress = request.EmailId;
                response.PrintNameOfSigner = request.PrintNameOfSignerW8IMY;
                response.OnBehalfName = request.W8IMYOnBehalfName;
                response.FilePath = request.TemplateFilePath;
                _evolvedtaxContext.TblW8imyforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                return W8IMYCreationEXP(request);

            }
            return Save(request);
        }

        public int UpdatePartial(FormRequest request)
        {
            var response = _evolvedtaxContext.TblW8imyforms.FirstOrDefault(p => p.EmailAddress == request.EmailId);
            if (response != null)
            {
                request.W8ExpId = response.Id;
                response.NameOfOrganization = request.GQOrgName;
                response.CountryOfIncorporation = request.CountryOfIncorporation;
                response.UsTinCb = request.US_TIN_CB;
                response.UsTin = request.US_TIN;
                response.Gin = request.GIN;
                response.ForeignNumberCb = request.LegallyRequired;
                response.ForeignTaxIdentifyingNumber = request.ForeignTaxIdentifyingNumber;
                response.ReferenceNumberS = request.Referencenumber;
                response.De = request.DE;
                response.NameOfDiregardedEntity = request.DEOwnerName;
                response._11fatcaCb = request.W8EXPFatca;
                response._12Country = request._12_Country;
                response._12mailingAddress = request._12Mailing_address;
                response._12City = request._12_City;
                response._12State = request._12_State;
                response._12Province = request._12_Province;
                response._13gin = request._13GIN;
                response.FatcaStatus = request.FatcaStatus;
                response.TypeOfEntity = request.EntityType;
                response._14Cb = request._14_CB;
                response._15a = request._15a;
                response._15b = request._15b;
                response._15c = request._15c;
                response._15d = request._15d;
                response._15e = request._15e;
                response._15f = request._15f;
                response._15g = request._15g;
                response._15h = request._15h;
                response._15i = request._15i;
                response._16a = request._16a;
                response._16b = request._16b;
                response._17a = request._17a;
                response._17b = request._17b;
                response._17c = request._17c;
                response._17d = request._17d;
                response._17e = request._17e;
                response._18a = request._18a;
                response._18b = request._18b;
                response._18c = request._18c;
                response._18d = request._18d;
                response._18e = request._18e;
                response._18f = request._18f;
                response._19a = request._19a;
                response._19b = request._19b;
                response._19c = request._19c;
                response._19d = request._19d;
                response._19e = request._19e;
                response._19f = request._19f;
                response._20 = request._20;
                response._21a = request._21a;
                response._21b = request._21b;
                response._21c = request._21c;
                response._21d = request._21d;
                response._21e = request._21e;
                response._21f = request._21f;
                response._22 = request._22;
                response._23aText = request._23a_text;
                response._23b = request._23b;
                response._23c = request._23c;
                response._24a = request._24a;
                response._24b = request._24b;
                response._24c = request._24c;
                response._25 = request._25;
                response._26 = request._26;
                response._27aText = request._27a_text;
                response._27b = request._27b;
                response._28 = request._28;
                response._29 = request._29;
                response._30a = request._30a;
                response._30b = request._30b;
                response._30c = request._30c;
                response._31 = request._31;
                response._32Text1 = request._32_Text1;
                response._32Cb1 = request._32_CB1;
                response._32Text2 = request._32_Text2;
                response._32Text3 = request._32_Text3;
                response._32Cb2 = request._32_CB2;
                response._32Cb3 = request._32_CB3;
                response._33a = request._33a;
                response._33b = request._33b;
                response._33c = request._33c;
                response._33d = request._33d;
                response._33e = request._33e;
                response._33f = request._33f;
                response._34 = request._34;
                response._35Cb = request._35;
                response._35Text = request._35_Text;
                response._36 = request._36;
                response._36Text = request._36_Text;
                response._37aCb = request._37a;
                response._37aText = request._37a_Text;
                response._37bCb = request._37b;
                response._37bText1 = request._37b_Text1;
                response._37bText2 = request._37b_Text2;
                response._38 = request._38;
                response._39 = request._39;
                response._40 = request._40;
                response._41Text = request._41_Text;
                response._42Cb = request._42;
                response.EmailAddress = request.EmailId;
                response.PrintNameOfSigner = request.PrintNameOfSignerW8IMY;
                response.OnBehalfName = request.W8IMYOnBehalfName;
                response.ActiveTabIndex = request.activeTabIndex;
                _evolvedtaxContext.TblW8imyforms.Update(response);
                _evolvedtaxContext.SaveChanges();
                return response.Id;



            }
            return SavePartial(request);
        }

        protected static string W8IMYCreationEXP(FormRequest request)
        {
            string templatefile = request.TemplateFilePath;
            string newFile1 = string.Empty;
            if (request.IndividualOrEntityStatus == AppConstants.IndividualStatus)
            {
                newFile1 = string.Concat(string.Concat(request.GQFirstName, " ", request.GQLastName).Replace(" ", "_"), "_", "Form_", AppConstants.W8EXPForm, "_", request.W8ExpId, "_temp.pdf");
            }
            else
            {
                newFile1 = string.Concat(request.GQOrgName.Replace(" ", "_"), "_", "Form_", AppConstants.W8IMYForm, "_", request.W8ExpId, "_temp.pdf");
            }
            string newFile = Path.Combine(request.BasePath, newFile1);

            PdfReader pdfReader = new PdfReader(templatefile);
            int numberOfPages = pdfReader.NumberOfPages;
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_1[0]", request.GQOrgName);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_2[0]", request.CountryOfIncorporation);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_3[0]", request.DEOwnerName);


            switch (request?.EntityType)
            {
                case "1":
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "2":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_1[4]", "0");
                    break;
                case "3":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "4":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "5":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "6":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "7":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "8":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "1");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "0");
                    break;
                case "9":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftCol[0].c1_1[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[2]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol[0].c1_1[3]", "1");
                    break;
              

            }





            switch (request?.FatcaStatus)
            {
                case "1":
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "2":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "3":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "4":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "5":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "6":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;

                case "7":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "8":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "9": // Limited liability company
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "10":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "11":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "12":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "13":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "14":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "15":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "16":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "17":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "18":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "19":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "20":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "21":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "22":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");
                    break;
                case "23":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "24":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "25":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
                case "26":
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].LeftColLn5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[0]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[1]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[2]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[3]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[4]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[5]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[6]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[7]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[8]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[9]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[10]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[11]", "0");
                    pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[12]", "0");
                    //pdfFormFields.SetField("topmostSubform[0].Page1[0].RightCol5[0].c1_2[13]", "0");

                    break;
            }

            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_4[0]", request.PAddress1);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_5[0]", request.PCity);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_6[0]", request.PCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_7[0]", request.MAddress1);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_8[0]", request.MCity);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_9[0]", request.MCountry);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[0]", request.US_TIN);
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[1]", request.GIN);
            if (!string.IsNullOrEmpty(request.ForeignTaxIdentifyingNumber))
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_10[2]", request.ForeignTaxIdentifyingNumber);
            }
            pdfFormFields.SetField("topmostSubform[0].Page1[0].f1_11[0]", request.Referencenumber);


            if (request.US_TIN_CB == "QI-EIN")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[3]", "0");
            }
            else if (request.US_TIN_CB == "WP-EIN")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[3]", "0");
            }
            else if (request.US_TIN_CB == "WT-EIN")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[3]", "0");
            }
            else if (request.US_TIN_CB == "EIN")
            {
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[2]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page1[0].c1_3[3]", "0");
            }

            if (request.W8EXPFatca == "1")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[4]", "0");
            }
            else if (request.W8EXPFatca == "2")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[4]", "0");
            }
            else if (request.W8EXPFatca == "3")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[4]", "0");
            }
            else if (request.W8EXPFatca == "4")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[2]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[4]", "0");
            }
            else if (request.W8EXPFatca == "5")
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[3]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[4]", "0");
            }
            else if (string.IsNullOrEmpty(request.W8EXPFatca))
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[2]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[3]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_1[4]", "0");
            }

            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_2[0]", request._12Mailing_address);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_3[0]", request._12_City);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_4[0]", request._12_Country);
            pdfFormFields.SetField("topmostSubform[0].Page2[0].f2_5[0]", request._13GIN);


            if (request._14_CB != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_2[0]", "0");
            }
            if (request._15a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_3[0]", "0");
            }
            if (request._15b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_4[0]", "0");
            }
            if (request._15c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_5[0]", "0");
            }
            if (request._15d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_6[0]", "0");
            }
            if (request._15e != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_7[0]", "0");
            }
            if (request._15f != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_8[0]", "0");
            }
            if (request._15g != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_9[0]", "0");
            }
            if (request._15h != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_10[0]", "0");
            }
            if (request._15i != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page2[0].c2_11[0]", "0");
            }



            if (request._16a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_1[0]", "0");
            }

            if (request._16b == "Corporation")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[2]", "0");
            }
            else if (request._16b == "Partnership")
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[2]", "0");
            }
            else if (request._16b == "DisregardedEntity")
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[1]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[2]", "0");
            }
            else if (request._16b == string.Empty)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_2[2]", "0");
            }



            if (request._17a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_5[0]", "0");
            }
            if (request._17b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_6[0]", "0");
            }
            if (request._17c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_7[0]", "0");
            }
            if (request._17d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_8[0]", "0");
            }
            if (request._17e != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_9[0]", "0");
            }


            if (request._18a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_10[0]", "0");
            }

            if (request._18b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_11[0]", "0");
            }
            if (request._18c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_11[1]", "0");
            }
            if (request._18d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_13[0]", "0");
            }
            if (request._18e != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_14[0]", "0");
            }
            if (request._18f != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_15[0]", "0");
            }


            if (request._19a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].Line19a_ReadOrder[0].c3_16[0]", "0");
            }
            if (request._19b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_17[0]", "0");
            }
            if (request._19c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_17[1]", "0");
            }
            if (request._19d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_19[0]", "0");
            }
            if (request._19e != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_20[0]", "0");
            }
            if (request._19f != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page3[0].c3_21[0]", "0");
            }


            if (request._20 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_1[0]", "0");
            }
            if (request._21a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_2[0]", "0");
            }
            if (request._21b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_3[0]", "0");
            }
            if (request._21c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_4[0]", "0");
            }
            if (request._21d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_5[0]", "0");
            }
            if (request._21e != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_6[0]", "0");
            }
            if (request._21f != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_7[0]", "0");
            }
            if (request._22 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_8[0]", "0");
            }



            pdfFormFields.SetField("topmostSubform[0].Page4[0].f4_1[0]", request._23a_text);


            if (request._23b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_9[0]", "0");
            }


            if (request._23c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_9[1]", "0");
            }

            if (request._24a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page4[0].c4_11[0]", "0");
            }
            if (request._24b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c4_4[0]", "0");
            }
            if (request._24c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c4_4[1]", "0");
            }

            if (request._25 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_1[0]", "0");
            }
            if (request._26 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_2[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page5[0].f5_1[0]", request._27a_text);


            if (request._27b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_3[0]", "0");
            }
            if (request._28 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page5[0].c5_4[0]", "0");
            }
            if (request._29 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_1[0]", "0");
            }

            if (request._30a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_2[0]", "0");
            }

            if (request._30b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_3[0]", "0");
            }
            if (request._30c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_3[1]", "0");
            }

            if (request._31 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_5[0]", "0");
            }
            if (string.IsNullOrEmpty(request._32_Text1))
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_6[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].f6_1[0]", request._32_Text1);

            if (request._32_CB2 == "Model 1 IGA")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].c6_7[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].c6_7[1]", "0");
            }
            else if (request._32_CB2 == "Model 2 IGA")
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].c6_7[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].c6_7[1]", "0");
            }
            else if (string.IsNullOrEmpty(request._32_CB2)) 
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].c6_7[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].c6_7[0]", "0");
            }


            if (request._32_CB3 == "U.S.")
            {
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].c6_8[0]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].c6_8[1]", "0");
            }
            else if (request._32_CB3 == "Foreign")
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].c6_8[0]", "0");
                //pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].c6_8[1]", "0");
            }
            else if (string.IsNullOrEmpty(request._32_CB3))
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].c6_8[1]", "0");
                pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].c6_8[0]", "0");
            }

            pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet1[0].f6_2[0]", request._32_Text2);
            pdfFormFields.SetField("topmostSubform[0].Page6[0].BulletedList3[0].Bullet2[0].f6_3[0]", request._32_Text3);



            if (request._33a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_10[0]", "0");
            }
            if (request._33b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page6[0].c6_10[1]", "0");
            }

            if (request._33c != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c6_10[0]", "0");
            }
            if (request._33d != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c6_10[1]", "0");
            }
            if (request._33e != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c6_10[2]", "0");
            }
            if (request._33f != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c6_10[3]", "0");
            }
            if (request._34 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_5[0]", "0");
            }
            if (request._35 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_6[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletedList4[0].Bullet1[0].f7_1[0]", request._35_Text?.ToString("MM-dd-yyyy"));
            if (request._36_Text == null)
            {
                pdfFormFields.SetField("topmostSubform[0].Page7[0].c7_7[0]", "0");
            }
         
            pdfFormFields.SetField("topmostSubform[0].Page7[0].BulletedList5[0].Bullet1[0].f7_2[0]", request._36_Text?.ToString("MM-dd-yyyy"));



            if (request._37a != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_1[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page8[0].BulletedList1[0].Bullet2[0].f7_3[0]", request._37a_Text);

            if (request._37b != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_1[1]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page8[0].BulletedList2[0].Bullet3[0].f7_4[0]", request._37b_Text1);
            pdfFormFields.SetField("topmostSubform[0].Page8[0].BulletedList2[0].Bullet4[0].f7_5[0]", request._37b_Text2);

            if (request._38 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_2[0]", "0");
            }
            if (request._39 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_3[0]", "0");
            }
            if (request._40 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].Line40[0].c8_4[0]", "0");
            }
            pdfFormFields.SetField("topmostSubform[0].Page8[0].f8_1[0]", request._41_Text);

            if (request._42 != true)
            {
                pdfFormFields.SetField("topmostSubform[0].Page8[0].c8_5[0]", "0");
            }






            pdfFormFields.SetField("topmostSubform[0].Page8[0].PrintName[0]", request.PrintNameOfSignerW8IMY);



            pdfFormFields.SetField("Signature", "Form_W8IMY.png");


            // string sTmp = "W8-IMY Completed for " + pdfFormFields.GetField("f1_9(0)") + " " + pdfFormFields.GetField("f1_10(0)");
            // flatten the form To remove editting options, set it to false  
            // to leave the form open to subsequent manual edits  
            pdfStamper.FormFlattening = true;
            // close the pdf  
            //    pdfStamper.Close();



            PdfContentByte overContent = pdfStamper.GetOverContent(numberOfPages);
            // iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(70, 530, 190, 550, 0);
            iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(105, 185, 320, 205, 0);
            rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent.Rectangle(rectangle);

            //for date
            PdfContentByte overContent1 = pdfStamper.GetOverContent(numberOfPages);
            //iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(450, 530, 610, 550, 0);
            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(495, 185, 575, 205, 0);
            rectangle1.BackgroundColor = BaseColor.LIGHT_GRAY;
            overContent1.Rectangle(rectangle1);

            // For pasting image of signature
            var src1 = Path.Combine(Directory.GetCurrentDirectory(), "signature-image.png");
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(src1);
            PdfImage stream1 = new PdfImage(image1, "", null);
            stream1.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));
            PdfIndirectObject ref1 = pdfStamper.Writer.AddToBody(stream1);
            // image1.SetAbsolutePosition(70, 530);
            image1.SetAbsolutePosition(105, 185);
            PdfContentByte over1 = pdfStamper.GetOverContent(numberOfPages);
            over1.AddImage(image1);


            #region Pasting Date Picture
            // Load the image file using SkiaSharp
            using (SKBitmap bitmap = SKBitmap.Decode(Path.Combine(Directory.GetCurrentDirectory(), "pictureText.bmp")))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.TextSize = 9;

                        string text = DateTime.Now.ToString("MM-dd-yyyy");
                        paint.Typeface = SKTypeface.FromFamilyName("Times New Roman");
                        paint.IsAntialias = true; // Enable anti-aliasing
                        SKRect textBounds = new SKRect();
                        paint.MeasureText(text, ref textBounds);

                        // Calculate the position to center the text horizontally and align it vertically in the middle
                        float x = (bitmap.Width - textBounds.Width) / 4;
                        float y = (bitmap.Height - textBounds.Height) / 2 + textBounds.Height;

                        // Draw the text at the calculated position
                        canvas.DrawText(text, x, y, paint);
                    }
                }

                // Save the modified image as PNG
                using (SKData encoded = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                {
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "date2.png");
                    File.WriteAllBytes(outputPath, encoded.ToArray());
                }
            }
            // For pasting date as image
            var src2 = Path.Combine(Directory.GetCurrentDirectory(), "date2.png");
            iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(src2);
            PdfImage stream2 = new PdfImage(image2, "", null);
            stream2.Put(new PdfName("ITXT_SpecialId"), new PdfName("1234567"));
            PdfIndirectObject ref2 = pdfStamper.Writer.AddToBody(stream2);
            //image2.SetAbsolutePosition(450, 530);
            image2.SetAbsolutePosition(495, 185);
            PdfContentByte over2 = pdfStamper.GetOverContent(numberOfPages);
            over2.AddImage(image2);
            #endregion
            PdfAnnotation annotation;
            annotation = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation, numberOfPages);

            PdfAnnotation annotation1;
            annotation1 = PdfAnnotation.CreateLink(pdfStamper.Writer, rectangle1, PdfAnnotation.HIGHLIGHT_INVERT, new PdfAction(Path.Combine(request.Host, "Certification", "Index")));
            pdfStamper.AddAnnotation(annotation1, numberOfPages);


            pdfStamper.Close();
            pdfReader.Close();
            return newFile1;
        }

        public async Task<bool> UpdateByClientEmailId(string ClientEmail, PdfFormDetailsRequest request)
        {
            var response = _evolvedtaxContext.TblW8imyforms.Where(p => p.EmailAddress == ClientEmail).FirstOrDefault();
            response.UploadedFile = request.FileName;
            response.SignatureDateMmDdYyyy = request.EntryDate.ToString();
            response.Status = "3";
            response.FontName = request.FontFamily;
            response.PrintNameOfSigner = request.PrintName;
            response.PrintSize = Convert.ToInt32(request.FontSize);
            _evolvedtaxContext.TblW8imyforms.Update(response);
            await _evolvedtaxContext.SaveChangesAsync();
            return true;
        }

      
    }
}
