using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.html.simpleparser;
using EvolvedTax.Data.Models.DTOs.Request;
using Microsoft.AspNetCore.Hosting.Server;
using System.Drawing;
using System.Xml.Linq;
using SkiaSharp;
using static iTextSharp.text.Font;
using EvolvedTax.Common.Constants;
using EvolvedTax.Business.Services.GeneralQuestionareService;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using EvolvedTax.Business.Services.GeneralQuestionareEntityService;
using System.Linq.Expressions;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using EvolvedTax.Data.Models.Entities._1099;
using EvolvedTax.Data.Models.DTOs.Response;

namespace EvolvedTax.Business.Services.Form1099Services
{
    public class TrailAudit1099Service : ITrailAudit1099Service
    {
        readonly EvolvedtaxContext _evolvedtaxContext;

        public TrailAudit1099Service(EvolvedtaxContext evolvedtaxContext)
        {
            _evolvedtaxContext = evolvedtaxContext;
        }
        public async Task<bool> CheckIfRecipientRecordExist(string s, string e)
        {
            return !await _evolvedtaxContext.AuditTrail1099.AnyAsync(p => p.RecipientEmail == s && p.Token == e);
        }

        public AuditTrail1099 GetRecipientDataByEmailId(string RecipientEmail)
        {
            return _evolvedtaxContext.AuditTrail1099.First(p => p.RecipientEmail == RecipientEmail);
        }

        public async Task AddUpdateRecipientAuditDetails(AuditTrail1099 request)
        {
            if (_evolvedtaxContext.AuditTrail1099.Any(p => p.RecipientEmail == request.RecipientEmail))
            {
                var response = _evolvedtaxContext.AuditTrail1099.First(p => p.RecipientEmail == request.RecipientEmail);
                response.OTP = request.OTP;
                response.OTPExpiryTime = request.OTPExpiryTime;
                response.Description = request.Description;
                response.Token = request.Token;
                response.FormName = request.FormName;
                _evolvedtaxContext.AuditTrail1099.Update(response);
            }
            else
            {
                await _evolvedtaxContext.AddAsync(request);
            }
            await _evolvedtaxContext.SaveChangesAsync();

            if (!_evolvedtaxContext.RcpElecAcptnceStatus.Any(p => p.Rcp_Email == request.RecipientEmail && p.FormName == request.FormName))
            {
                var request2 = new RcpElecAcptnceStatus { Rcp_Email = request.RecipientEmail, FormName = request.FormName };
                await _evolvedtaxContext.RcpElecAcptnceStatus.AddAsync(request2);
                await _evolvedtaxContext.SaveChangesAsync();
            }
        }
        public async Task<AuditTrail1099> UpdateRecipientStatus(AuditTrail1099 request)
        {
            if (_evolvedtaxContext.AuditTrail1099.Any(p => p.RecipientEmail == request.RecipientEmail))
            {
                var response = _evolvedtaxContext.AuditTrail1099.First(p => p.RecipientEmail == request.RecipientEmail);
                _evolvedtaxContext.AuditTrail1099.Update(response);
                await _evolvedtaxContext.SaveChangesAsync();
                request = response;
            }
            return request;
        }
        public async Task UpdateOTPStatus(AuditTrail1099 request)
        {
            if (_evolvedtaxContext.AuditTrail1099.Any(p => p.RecipientEmail == request.RecipientEmail))
            {
                var response = _evolvedtaxContext.AuditTrail1099.First(p => p.RecipientEmail == request.RecipientEmail);
                response.OTP = request.OTP;
                response.OTPExpiryTime = request.OTPExpiryTime;
                _evolvedtaxContext.AuditTrail1099.Update(response);
                await _evolvedtaxContext.SaveChangesAsync();
            }
        }
        public IList<AuditTrail1099> GetRecipientListByEmailId(string recipientEmail)
        {
            return _evolvedtaxContext.AuditTrail1099.Where(p => p.RecipientEmail == recipientEmail).ToList();
        }
        public async Task<AuditTrail1099> UpdateRcpElecAcptnceStatusStatus(List<RcpElecAcptnceStatus> request)
        {
            foreach (var item in request)
            {

                if (_evolvedtaxContext.RcpElecAcptnceStatus.Any(p => p.Rcp_Email == item.Rcp_Email && p.FormName == item.FormName))
                {
                    var response = _evolvedtaxContext.RcpElecAcptnceStatus.First(p => p.Rcp_Email == item.Rcp_Email && p.FormName == item.FormName);
                    response.Status = item.Status;
                    _evolvedtaxContext.RcpElecAcptnceStatus.Update(response);
                }
                else
                {
                    await _evolvedtaxContext.RcpElecAcptnceStatus.AddAsync(item);
                }
                await _evolvedtaxContext.SaveChangesAsync();
            }

            return _evolvedtaxContext.AuditTrail1099.First(p => p.RecipientEmail == request.First().Rcp_Email);
        }
        public IList<RcpElecAcptnceStatus> GetRecipientStatusListByEmailId(string recipientEmail)
        {
            return _evolvedtaxContext.RcpElecAcptnceStatus.Where(p => p.Rcp_Email == recipientEmail && p.Status == 0).ToList();
        }
    }
}
