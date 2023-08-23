using Azure.Core;
using Azure;
using EvolvedTax.Data.Models.Entities;
using iTextSharp.text.pdf;
using System.Diagnostics.Metrics;
using System.Globalization;
using iTextSharp.text;
using EvolvedTax.Data.Models.DTOs.Request;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Identity;
using EvolvedTax.Data.Enums;
using System.Security.Policy;
using System;

namespace EvolvedTax.Business.Services.UserService
{
    public class UserService : IUserService
    {
        readonly EvolvedtaxContext _evolvedtaxContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(EvolvedtaxContext evolvedtaxContext, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<UserRequest> Login(LoginRequest model)
        {
            var request = new UserRequest();
            //var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == model.UserName && p.Password == model.Password);
            //var repsonse = await _signInManager.SignInAsync(new User { UserName = model.UserName, Email = model.UserName }, false);
            //if (response != null)
            //{
            //    request.UserName = response.FirstName + " " + response.LastName;
            //    request.InstId = response.InstId;
            //    request.EmailId = response.EmailAddress ?? string.Empty;
            //    request.InstituteName = response.InstitutionName ?? string.Empty;
            //    request.IsLoggedIn = true;
            //    request.IsAdmin = response.IsAdmin;
            //    return request;
            //}
            return request;
        }
        public bool UpdateInstituteMasterOTP(string emailId, string otp, DateTime expiryDate)
        {
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == emailId);
            if (response != null)
            {
                response.OtpexpiryDate = expiryDate;
                response.Otp = otp;
                _evolvedtaxContext.InstituteMasters.Update(response);
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UpdateInstituteClientOTP(string emailId, string otp, DateTime expiryDate)
        {
            var response = _evolvedtaxContext.InstitutesClients.FirstOrDefault(p => p.ClientEmailId == emailId);
            if (response != null)
            {
                response.OtpexpiryDate = expiryDate;
                response.Otp = otp;
                _evolvedtaxContext.InstitutesClients.Update(response);
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<IdentityResult> Save(UserRequest request)
        {
            if (request.SURegistrationExpiryDate == null)
            {
                request.SURegistrationExpiryDate = request.SURegistrationDate.AddDays(365);
            }
            var model = new InstituteMaster
            {
                InstitutionName = request.SUInstitutionName,

                SupportEmail = string.IsNullOrEmpty(request.SupportEmailAddress) ? request.SUEmailAddress : request.SupportEmailAddress,
                Idtype = request.SUIDType,
                Idnumber = request.SUIDNumber,
                TypeofEntity = request.SUTypeofEntity,

                Mcountry = request.SUMCountry,
                Madd1 = request.SUMMAdd1,
                Madd2 = request.SUMMAdd2,
                Mcity = request.SUMCity,
                Mstate = request.SUMState,
                Mzip = request.SUMZip,
                Mprovince = request.SUMProvince,

                Pcountry = request.SUPCountry,
                Padd1 = request.SUMPAdd1,
                Padd2 = request.SUPPAdd2,
                Pcity = request.SUPCity,
                Pstate = request.SUPState,
                Pzip = request.SUPZip,
                Pprovince = request.SUPProvince,

                Ftin = request.SUFTIN,
                Gin = request.SUGIN,
                CountryOfIncorporation = request.SUCountryOfIncorporation,
                Status = "1",
                StatusDate = DateTime.Now,
                Phone = request.Phone
            };

            var userModel = new User
            {
                FirstName = request.SUFirstName,
                LastName = request.SULastName,
                Email = request.SUEmailAddress,
                UserName = request.SUEmailAddress,
                Position = request.SUPosition,
                InstituteId = model.InstId,
                IsSuperAdmin = false,
                PasswordSecuredA1 = request.SUPasswordSecuredA1,
                PasswordSecuredA2 = request.SUPasswordSecuredA2,
                PasswordSecuredA3 = request.SUPasswordSecuredA3,
                PasswordSecuredQ1 = request.SUPasswordSecuredQ1,
                PasswordSecuredQ2 = request.SUPasswordSecuredQ2,
                PasswordSecuredQ3 = request.SUPasswordSecuredQ3,
                Country = request.SUMCountry,
                Address1 = request.SUMMAdd1,
                Address2 = request.SUMMAdd2,
                City = request.SUMCity,
                State = request.SUMState,
                Zip = request.SUMZip,
                Province = request.SUMProvince,
            };

            await _evolvedtaxContext.InstituteMasters.AddAsync(model);
            await _evolvedtaxContext.SaveChangesAsync();
            var user = await _userManager.FindByEmailAsync(userModel.Email);
            var response = new IdentityResult();
            if (user == null)
            {
                response = await _userManager.CreateAsync(userModel, request.SUPassword);
                await _userManager.AddToRoleAsync(userModel, Roles.Admin.ToString());
            }

            return response;
        }
        public async Task<IdentityResult> SaveInvitedUser(UserRequest request)
        {
            var userModel = new User
            {
                FirstName = request.SUFirstName,
                LastName = request.SULastName,
                Email = request.SUEmailAddress,
                UserName = request.SUEmailAddress,
                Position = request.SUPosition,
                InstituteId = request.InstId,
                IsSuperAdmin = false,
                PasswordSecuredA1 = request.SUPasswordSecuredA1,
                PasswordSecuredA2 = request.SUPasswordSecuredA2,
                PasswordSecuredA3 = request.SUPasswordSecuredA3,
                PasswordSecuredQ1 = request.SUPasswordSecuredQ1,
                PasswordSecuredQ2 = request.SUPasswordSecuredQ2,
                PasswordSecuredQ3 = request.SUPasswordSecuredQ3,
                Country = request.SUMCountry,
                Address1 = request.SUMMAdd1,
                Address2 = request.SUMMAdd2,
                City = request.SUMCity,
                State = request.SUMState,
                Zip = request.SUMZip,
                Province = request.SUMProvince,
            };

            var user = await _userManager.FindByEmailAsync(userModel.Email);
            var response = new IdentityResult();
            if (user == null)
            {
                response = await _userManager.CreateAsync(userModel, request.SUPassword);
                await _userManager.AddToRoleAsync(userModel, Roles.CoAdmin.ToString());
            }

            return response;
        }
        public async Task<bool> AddRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.Contributor.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.Viewer.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.CoAdmin.ToString()));
            return true;
        }
        public UserRequest GetUserbyEmailId(string emailId)
        {
            var request = new UserRequest();
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == emailId);
            if (response != null)
            {
                request.UserName = response.FirstName + " " + response.LastName;
                request.InstId = response.InstId;
                request.EmailId = response.EmailAddress ?? string.Empty;
                request.InstituteName = response.InstitutionName ?? string.Empty;
                request.IsLoggedIn = true;
                request.ExpiryDate = response.OtpexpiryDate;
                request.OTP = response.OtpexpiryDate >= DateTime.Now ? response.Otp : "";
                request.InstituteLogo = response.InstituteLogo ?? string.Empty;
                return request;
            }
            return request;
        }
        public bool ValidateSecurityQuestions(ForgetPasswordRequest request)
        {
            var result = _evolvedtaxContext.InstituteMasters.Where(p => p.EmailAddress == request.EmailAddress
            && (
               (p.PasswordSecuredA1.ToLower() == request.PasswordSecuredA1.Trim().ToLower() && p.PasswordSecuredQ1 == request.PasswordSecuredQ1)
            || (p.PasswordSecuredA2.ToLower() == request.PasswordSecuredA2.Trim().ToLower() && p.PasswordSecuredQ2 == request.PasswordSecuredQ2)
            || (p.PasswordSecuredA3.ToLower() == request.PasswordSecuredA3.Trim().ToLower() && p.PasswordSecuredQ3 == request.PasswordSecuredQ3)
            ));
            return result.Any();
        }
        public bool UpdateResetToeknInfo(string emailAddress, string passwordResetToken, DateTime passwordResetTokenExpiration)
        {
            var response = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == emailAddress);
            if (response != null)
            {
                response.ResetToken = passwordResetToken;
                response.ResetTokenExpiryTime = passwordResetTokenExpiration;
                _evolvedtaxContext.Update(response);
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool ResetPassword(ForgetPasswordRequest request)
        {
            var result = _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.ResetToken == request.ResetToken);
            if (result != null && result.ResetTokenExpiryTime > DateTime.Now)
            {
                result.Password = request.Password;
                _evolvedtaxContext.Update(result);
                _evolvedtaxContext.SaveChanges();
                return true;
            }
            return false;
        }
        public InstituteMaster? GetSecurityQuestionsByInstituteEmail(string emailAddress)
        {
            return _evolvedtaxContext.InstituteMasters.FirstOrDefault(p => p.EmailAddress == emailAddress);
        }
        public bool IsEmailExist(string sUEmailAddress)
        {
            if (_userManager.FindByEmailAsync(sUEmailAddress).Result == null)
            {
                return false;
            }
            return true;
        }

        public bool IsSignupLinkExpired(string userid)
        {
            var user = _evolvedtaxContext.EntitiesUsers.FirstOrDefault(e => e.UserId == userid.ToString());

            if (user == null || user.ExpirySignupDatetime.HasValue && user.ExpirySignupDatetime.Value < DateTime.Now)
            {
                return false;
            }
           
            return true;
        }
        public async Task<bool> SaveInvitedUserForShare(string role, int entityId, string email, int instituteId, string AssignedBy)
        {
            var result = false;
            DateTime CurrentDate= DateTime.Now;
            var userModel = new User
            {
                UserName = email,
                Email = email,
                InstituteId = instituteId,
                IsSuperAdmin = false,
                TwoFactorEnabled = true,
            };

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var response = await _userManager.CreateAsync(userModel, "User@123!@#");
                result = response.Succeeded;
                await _userManager.AddToRoleAsync(userModel, role);
            }
            
            await _evolvedtaxContext.EntitiesUsers.AddAsync(new EntitiesUsers { EntityId = entityId, UserId =_userManager.FindByEmailAsync(email).Result.Id, Role = role,AssignedBy= AssignedBy, EntryDatetime = CurrentDate, ExpirySignupDatetime = CurrentDate.AddDays(5)});
            await _evolvedtaxContext.SaveChangesAsync();
            return result;
        }
        public async Task<IdentityResult> UpdateInvitedUserForShare(UserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.SUEmailAddress);
            int id = user.InstituteId;
            user.FirstName = request.SUFirstName;
            user.LastName = request.SULastName;
            user.Position = request.SUPosition;
            user.IsSuperAdmin = false;
            user.PasswordSecuredA1 = request.SUPasswordSecuredA1;
            user.PasswordSecuredA2 = request.SUPasswordSecuredA2;
            user.PasswordSecuredA3 = request.SUPasswordSecuredA3;
            user.PasswordSecuredQ1 = request.SUPasswordSecuredQ1;
            user.PasswordSecuredQ2 = request.SUPasswordSecuredQ2;
            user.PasswordSecuredQ3 = request.SUPasswordSecuredQ3;
            user.Country = request.SUMCountry;
            user.Address1 = request.SUMMAdd1;
            user.Address2 = request.SUMMAdd2;
            user.City = request.SUMCity;
            user.State = request.SUMState;
            user.Zip = request.SUMZip;
            user.Province = request.SUMProvince;
            user.EmailConfirmed = true;
            user.InstituteId = id;
            var response = new IdentityResult();
            if (user != null)
            {
                response = await _userManager.UpdateAsync(user);
                await _userManager.ChangePasswordAsync(user, "User@123!@#", request.SUPassword);
            }
            return response;
        }
    }
}
