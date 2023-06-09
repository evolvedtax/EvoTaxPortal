﻿using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Business.Services.UserService
{
    public interface IUserService
    {
        public string Save(UserRequest model);
        public UserRequest Login(LoginRequest model);
        bool UpdateInstituteMasterOTP(string emailId, string otp, DateTime expiryDate);
        bool UpdateInstituteClientOTP(string emailId, string otp, DateTime expiryDate);
        UserRequest GetUserbyEmailId(string emailId);
        bool ValidateSecurityQuestions(ForgetPasswordRequest request);
        bool UpdateResetToeknInfo(string emailAddress, string passwordResetToken, DateTime passwordResetTokenExpiration);
        bool ResetPassword(ForgetPasswordRequest request);
        InstituteMaster? GetSecurityQuestionsByInstituteEmail(string emailAddress);
    }
}
