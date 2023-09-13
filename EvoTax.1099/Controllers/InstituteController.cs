﻿using System.Text;
using EvolvedTax.Business.MailService;
using EvolvedTax.Business.Services.CommonService;
using EvolvedTax.Business.Services.InstituteService;
using EvolvedTax.Common.Constants;
using EvolvedTax.Data.Enums;
using EvolvedTax.Data.Models.DTOs.Request;
using EvolvedTax.Data.Models.DTOs.Response;
using EvolvedTax.Data.Models.DTOs.ViewModels;
using EvolvedTax.Data.Models.Entities;
using EvolvedTax.Helpers;
using EvolvedTax.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;

namespace EvolvedTax_1099.Controllers
{
    public class InstituteController : BaseController
    {
        readonly private IInstituteService _instituteService;
        readonly private IWebHostEnvironment _webHostEnvironment;
        readonly private IMailService _emailService;
        readonly ICommonService _commonService;
        readonly RoleManager<IdentityRole> _identityRoles;
        readonly UserManager<User> _userManager;
        readonly EvolvedtaxContext _evolvedtaxContext;
        public InstituteController(IInstituteService instituteService, IMailService emailService, IWebHostEnvironment webHostEnvironment, ICommonService commonService, EvolvedtaxContext evolvedtaxContext, RoleManager<IdentityRole> identityRoles, UserManager<User> userManager)
        {
            _evolvedtaxContext = evolvedtaxContext;
            _commonService = commonService;
            _instituteService = instituteService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _identityRoles = identityRoles;
            _userManager = userManager;
        }

        #region Entities
        public IActionResult Entities(int? instituteId)
        {
            HttpContext.Session.SetInt32("EntityId", 0);
            var model = new InstituteEntityViewModel();
            var items = _evolvedtaxContext.MstrCountries.ToList();
            ViewBag.CountriesList = items.OrderBy(item => item.Favorite != "0" ? int.Parse(item.Favorite) : int.MaxValue)
                                  .ThenBy(item => item.Country).Select(p => new SelectListItem
                                  {
                                      Text = p.Country,
                                      Value = p.Country,
                                  });

            ViewBag.StatesList = _evolvedtaxContext.MasterStates.Select(p => new SelectListItem
            {
                Text = p.StateId,
                Value = p.StateId
            });
            if (instituteId != null)
            {
                ViewBag.InstituteId = instituteId;
                HttpContext.Session.SetInt32("SelectedInstitute", instituteId ?? 0);
                if (User.IsInRole("Admin") || User.IsInRole("Co-Admin"))
                {
                    model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(instituteId ?? 0);
                }
                else
                {
                    model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(instituteId ?? 0);
                    //model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstIdRole(instituteId ?? 0);
                }
                return View(model);
            }
            int InstId = HttpContext.Session.GetInt32("InstId") ?? 0;
            HttpContext.Session.SetInt32("SelectedInstitute", InstId);
            model.InstituteEntitiesResponse = _instituteService.GetEntitiesByInstId(InstId);
            return View(model);
        }
        #endregion
    }
}