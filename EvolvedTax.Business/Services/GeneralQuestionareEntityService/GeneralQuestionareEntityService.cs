
using EvolvedTax.Data.Models.Entities;


namespace EvolvedTax.Business.Services.GeneralQuestionareEntityService
{
    public class GeneralQuestionareEntityService : IGeneralQuestionareEntityService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        public GeneralQuestionareEntityService(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }

        public FormRequest? GetDataByClientEmail(string ClientEmail)
        {
            return _evolvedtaxContext.GeneralQuestionEntities.Where(p => p.UserName == ClientEmail).Select(p => new FormRequest
            {
                GQOrgName = p.OrgName ?? string.Empty,
                EntityType = p.EntityType?? string.Empty,
                Ccountry = p.Ccountry ?? string.Empty,
                MAddress1 = p.MailingAddress1 ?? string.Empty,
                MAddress2 = p.MailingAddress2 ?? string.Empty,
                MCity = p.MailingCity ?? string.Empty,
                MCountry = p.MailingCountry ?? string.Empty,
                MProvince = p.MailingProvince ?? string.Empty,
                MState = p.MailingState ?? string.Empty,
                MZipCode = p.MailingZip ?? string.Empty,
                PAddress1 = p.PermanentAddress1 ?? string.Empty,
                PAddress2 = p.PermanentAddress2 ?? string.Empty,
                PCity = p.PermanentCity ?? string.Empty,
                PCountry = p.PermanentCountry ?? string.Empty,
                PProvince = p.PermanentProvince ?? string.Empty,
                PState = p.PermanentState ?? string.Empty,
                PZipCode = p.PermanentZip ?? string.Empty,
                TypeofTaxNumber = p.TypeofTaxNumber ?? string.Empty,
                UserName = p.UserName ?? string.Empty,
                Payeecode = p.Payeecode ?? string.Empty,
                Fatca = p.Fatca ?? string.Empty,
                BackupWithHolding = p.BackupWithHolding ?? string.Empty,
                Ssnitnein = p.Number
            }).FirstOrDefault();
        }

        public int Save(FormRequest request)
        {
            var model = new GeneralQuestionEntity
            {
                OrgName = request.GQOrgName,
                EntityType = request.EntityType,
                Ccountry = request.Ccountry,
                MailingAddress1 = request.MAddress1,
                MailingAddress2 = request.MAddress2,
                MailingCity = request.MCity,
                MailingCountry = request.MCountry,
                MailingProvince = request.MProvince,
                MailingState = request.MState,
                MailingZip = request.MZipCode,
                PermanentAddress1 = request.PAddress1,
                PermanentAddress2 = request.PAddress2,
                PermanentCity = request.PCity,
                PermanentCountry = request.PCountry,
                PermanentProvince = request.PProvince,
                PermanentState = request.PState,
                PermanentZip = request.PZipCode,
                TypeofTaxNumber = request.TypeofTaxNumber,
                UserName = request.UserName,
                BackupWithHolding = request.BackupWithHolding,
                Fatca = request.Fatca,
                Payeecode = request.Payeecode,
                De=request.DE,
                DeownerName = request.DEOwnerName,
                EnitityManagendOutSideUsa=request.EnitityManagendOutSideUSA,
                Uspartner=request.USPartner,
                Idtype=request.IdType,
                Idnumber=request.IdNumber,
                W8formType=request.W8FormType,
                W8expId=request.W8ExpId

            };

            _evolvedtaxContext.GeneralQuestionEntities.Add(model);
            _evolvedtaxContext.SaveChanges();
            return model.Id;
        }
        public int Update(FormRequest model)
        {
            var response = _evolvedtaxContext.GeneralQuestionEntities.FirstOrDefault(p => p.UserName == model.EmailId);
            response.OrgName = model.GQOrgName;
            response.EntityType = model.EntityType;
            response.Ccountry = model.Ccountry;
            response.MailingAddress1 = model.MAddress1;
            response.MailingAddress2 = model.MAddress2;
            response.MailingCity = model.MCity;
            response.MailingCountry = model.MCountry;
            response.MailingProvince = model.MProvince;
            response.MailingState = model.MState;
            response.MailingZip = model.MZipCode;
            response.PermanentAddress1 = model.PAddress1;
            response.PermanentAddress2 = model.PAddress2;
            response.PermanentCity = model.PCity;
            response.PermanentCountry = model.PCountry;
            response.PermanentProvince = model.PProvince;
            response.PermanentState = model.PState;
            response.PermanentZip = model.PZipCode;
            response.TypeofTaxNumber = model.TypeofTaxNumber;
            response.BackupWithHolding = model.BackupWithHolding;
            response.Fatca = model.Fatca;
            response.Payeecode = model.Payeecode;
            _evolvedtaxContext.GeneralQuestionEntities.Update(response);
            return _evolvedtaxContext.SaveChanges();
        }
        public bool IsClientAlreadyExist(string ClientEmailId)
        {
            return _evolvedtaxContext.GeneralQuestionEntities.Any(p => p.UserName == ClientEmailId);
        }
    }
}
