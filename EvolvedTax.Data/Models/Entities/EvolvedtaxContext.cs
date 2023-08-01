using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EvolvedTax.Data.Models.Entities;

public partial class EvolvedtaxContext : DbContext
{
    public EvolvedtaxContext()
    {
    }

    public EvolvedtaxContext(DbContextOptions<EvolvedtaxContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<AvailabilityMaster> AvailabilityMasters { get; set; }

    public virtual DbSet<ContactDetail> ContactDetails { get; set; }

    public virtual DbSet<EmailDomain> EmailDomains { get; set; }

    public virtual DbSet<EmployeesMaster> EmployeesMasters { get; set; }

    public virtual DbSet<EvoClientsMaster> EvoClientsMasters { get; set; }

    public virtual DbSet<EvoProjectsMaster> EvoProjectsMasters { get; set; }

    public virtual DbSet<EvoTaskMaster> EvoTaskMasters { get; set; }

    public virtual DbSet<EvoTeam> EvoTeams { get; set; }

    public virtual DbSet<ExemptPayeeCode> ExemptPayeeCodes { get; set; }

    public virtual DbSet<FatcaCode> FatcaCodes { get; set; }

    public virtual DbSet<GeneralQuestionEntitiesBckup> GeneralQuestionEntitiesBckups { get; set; }

    public virtual DbSet<GeneralQuestionEntity> GeneralQuestionEntities { get; set; }

    public virtual DbSet<GeneralQuestionIndividual> GeneralQuestionIndividuals { get; set; }

    public virtual DbSet<InstituteEntity> InstituteEntities { get; set; }

    public virtual DbSet<InstituteMaster> InstituteMasters { get; set; }

    public virtual DbSet<InstitutesClient> InstitutesClients { get; set; }

    public virtual DbSet<MasterClientStatus> MasterClientStatuses { get; set; }

    public virtual DbSet<MasterEntityType> MasterEntityTypes { get; set; }

    public virtual DbSet<MasterPoboxWildcard> MasterPoboxWildcards { get; set; }

    public virtual DbSet<MasterState> MasterStates { get; set; }

    public virtual DbSet<MasterStatus> MasterStatuses { get; set; }

    public virtual DbSet<MasterUser> MasterUsers { get; set; }

    public virtual DbSet<MasterUserType> MasterUserTypes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MstrCountry> MstrCountries { get; set; }

    public virtual DbSet<Otphistory> Otphistories { get; set; }

    public virtual DbSet<PageChecking> PageCheckings { get; set; }

    public virtual DbSet<PasswordChangeLog> PasswordChangeLogs { get; set; }

    public virtual DbSet<PasswordSecurityQuestion> PasswordSecurityQuestions { get; set; }

    public virtual DbSet<TblCountry> TblCountries { get; set; }

    public virtual DbSet<TblLicense> TblLicenses { get; set; }

    public virtual DbSet<TblLoginHistory> TblLoginHistories { get; set; }

    public virtual DbSet<TblStatus> TblStatuses { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserGroup> TblUserGroups { get; set; }

    public virtual DbSet<TblW8benform> TblW8benforms { get; set; }

    public virtual DbSet<TblW8ebeneform> TblW8ebeneforms { get; set; }

    public virtual DbSet<TblW8ebeneformBackup> TblW8ebeneformBackups { get; set; }

    public virtual DbSet<TblW8eciform> TblW8eciforms { get; set; }

    public virtual DbSet<TblW8expform> TblW8expforms { get; set; }

    public virtual DbSet<TblW8imyform> TblW8imyforms { get; set; }

    public virtual DbSet<TblW8imyformBckup> TblW8imyformBckups { get; set; }

    public virtual DbSet<TblW9form> TblW9forms { get; set; }

    public virtual DbSet<TblW9formIndividual> TblW9formIndividuals { get; set; }

    public virtual DbSet<TblxxxU> TblxxxUs { get; set; }

    public virtual DbSet<ViewClient> ViewClients { get; set; }

    public virtual DbSet<W8bene14b> W8bene14bs { get; set; }

    public virtual DbSet<W8beneEntityType> W8beneEntityTypes { get; set; }

    public virtual DbSet<W8benefatca> W8benefatcas { get; set; }

    public virtual DbSet<W8benefatcade> W8benefatcades { get; set; }

    public virtual DbSet<W8eciEntityType> W8eciEntityTypes { get; set; }

    public virtual DbSet<W8expentity> W8expentities { get; set; }

    public virtual DbSet<W8expfatca> W8expfatcas { get; set; }

    public virtual DbSet<W8imyEntityType> W8imyEntityTypes { get; set; }

    public virtual DbSet<W8imyFatca> W8imyFatcas { get; set; }

    public virtual DbSet<W8imyfatcade> W8imyfatcades { get; set; }

    public virtual DbSet<W9> W9s { get; set; }

    public virtual DbSet<W91> W91s { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AvailabilityMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AvailabilityMaster");

            entity.Property(e => e.AvailabilityDate).HasColumnType("date");
            entity.Property(e => e.AvailabilityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AvailabilityID");
            entity.Property(e => e.EmpId).HasColumnName("EmpID");
        });

        modelBuilder.Entity<ContactDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ContactDetails");

            entity.Property(e => e.BusinessAddress)
                .IsUnicode(false)
                .HasColumnName("Business Address");
            entity.Property(e => e.BusinessContact)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Business Contact");
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .IsUnicode(false)
                .HasColumnName("Full Name");
            entity.Property(e => e.JobTitle)
                .IsUnicode(false)
                .HasColumnName("Job Title");
            entity.Property(e => e.Manager).IsUnicode(false);
            entity.Property(e => e.MobileContact)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mobile Contact");
            entity.Property(e => e.StateZip)
                .IsUnicode(false)
                .HasColumnName("State Zip");
        });

        modelBuilder.Entity<EmailDomain>(entity =>
        {
            entity.ToTable("EmailDomain");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmailDomain1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailDomain");
        });

        modelBuilder.Entity<EmployeesMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EmployeesMaster");

            entity.Property(e => e.BusinessAddress)
                .IsUnicode(false)
                .HasColumnName("Business Address");
            entity.Property(e => e.BusinessContact)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Business Contact");
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmpId).HasColumnName("EmpID");
            entity.Property(e => e.FullName)
                .IsUnicode(false)
                .HasColumnName("Full Name");
            entity.Property(e => e.JobTitle)
                .IsUnicode(false)
                .HasColumnName("Job Title");
            entity.Property(e => e.MobileContact)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mobile Contact");
            entity.Property(e => e.StateZip)
                .IsUnicode(false)
                .HasColumnName("State Zip");
        });

        modelBuilder.Entity<EvoClientsMaster>(entity =>
        {
            entity.ToTable("EvoClientsMaster");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClientAddress).IsUnicode(false);
            entity.Property(e => e.ClientCityStateZip).IsUnicode(false);
            entity.Property(e => e.ClientContactNumber).IsUnicode(false);
            entity.Property(e => e.ClientEmailAddress).IsUnicode(false);
            entity.Property(e => e.ClientId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ClientID");
            entity.Property(e => e.ClientName).IsUnicode(false);
            entity.Property(e => e.ClientNotes).IsUnicode(false);
            entity.Property(e => e.ContactPerson).IsUnicode(false);
            entity.Property(e => e.ContactPersonDesignation).IsUnicode(false);
        });

        modelBuilder.Entity<EvoProjectsMaster>(entity =>
        {
            entity.ToTable("EvoProjectsMaster");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProjectEndDate).HasColumnType("date");
            entity.Property(e => e.ProjectId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ProjectID");
            entity.Property(e => e.ProjectName).IsUnicode(false);
            entity.Property(e => e.ProjectStartDate).HasColumnType("date");
            entity.Property(e => e.ProjectStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EvoTaskMaster>(entity =>
        {
            entity.HasKey(e => e.TaskId);

            entity.ToTable("EvoTaskMaster");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.TaskName).IsUnicode(false);
        });

        modelBuilder.Entity<EvoTeam>(entity =>
        {
            entity.ToTable("EvoTeam");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TeamMember)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TeamMemberId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TeamMemberID");
        });

        modelBuilder.Entity<ExemptPayeeCode>(entity =>
        {
            entity.HasKey(e => e.ExemptId);

            entity.ToTable("ExemptPayeeCode");

            entity.Property(e => e.ExemptId).HasColumnName("ExemptID");
            entity.Property(e => e.ExemptCode).IsUnicode(false);
            entity.Property(e => e.ExemptValue)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FatcaCode>(entity =>
        {
            entity.HasKey(e => e.FatcaId);

            entity.ToTable("FATCA_CODES");

            entity.Property(e => e.FatcaId).HasColumnName("FATCA_ID");
            entity.Property(e => e.FatcaCode1)
                .IsUnicode(false)
                .HasColumnName("FATCA_Code");
            entity.Property(e => e.FatcaValue)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GeneralQuestionEntitiesBckup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GeneralQuestionEntities_bckup");

            entity.Property(e => e.BackupWithHolding)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Ccountry)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CCountry");
            entity.Property(e => e.De).HasColumnName("DE");
            entity.Property(e => e.DeownerName)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DEOwnerName");
            entity.Property(e => e.EnitityManagendOutSideUsa).HasColumnName("EnitityManagendOutSideUSA");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fatca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FATCA");
            entity.Property(e => e.FormType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Idtype)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDType");
            entity.Property(e => e.MailingAddress1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingAddress2)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MailingCity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingCountry)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingProvince)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingState)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingZip)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrgName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Payeecode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAYEECODE");
            entity.Property(e => e.PermanentAddress1)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddress2)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentCity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentCountry)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentProvince)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentState)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentZip)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeofTaxNumber)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Uspartner)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("USPartner");
            entity.Property(e => e.W8expId).HasColumnName("W8ExpId");
            entity.Property(e => e.W8formType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W8FormType");
        });

        modelBuilder.Entity<GeneralQuestionEntity>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BackupWithHolding)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Ccountry)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CCountry");
            entity.Property(e => e.De).HasColumnName("DE");
            entity.Property(e => e.DeownerName)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DEOwnerName");
            entity.Property(e => e.EnitityManagendOutSideUsa).HasColumnName("EnitityManagendOutSideUSA");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fatca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FATCA");
            entity.Property(e => e.FormType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Idtype)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDType");
            entity.Property(e => e.MailingAddress1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingAddress2)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MailingCity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingCountry)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingProvince)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingState)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingZip)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrgName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Payeecode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAYEECODE");
            entity.Property(e => e.PermanentAddress1)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddress2)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentCity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentCountry)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentProvince)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentState)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentZip)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeofTaxNumber)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Uspartner)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("USPartner");
            entity.Property(e => e.W8expId).HasColumnName("W8ExpId");
            entity.Property(e => e.W8formType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W8FormType");
        });

        modelBuilder.Entity<GeneralQuestionIndividual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_GQ_1");

            entity.ToTable("GeneralQuestionIndividual");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryofCitizenship)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingAddress1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingAddress2)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MailingCity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingCountry)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingProvince)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingState)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MailingZip)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddress1)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddress2)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PermanentCity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentCountry)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentProvince)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentState)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentZip)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RetirementPlan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SocialSecurityNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeofTaxNumber)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Us1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("US1");
            entity.Property(e => e.Uscitizen)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USCitizen");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<InstituteEntity>(entity =>
        {
            entity.HasKey(e => new { e.EntityId, e.InstituteId });

            entity.Property(e => e.EntityId)
                .ValueGeneratedOnAdd()
                .HasComment("EntityID")
                .HasColumnName("EntityID");
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.Address1).IsUnicode(false);
            entity.Property(e => e.Address2).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.Ein)
                .IsUnicode(false)
                .HasColumnName("EIN");
            entity.Property(e => e.EntityName).IsUnicode(false);
            entity.Property(e => e.EntityRegistrationDate).HasColumnType("date");
            entity.Property(e => e.InActiveDate).HasColumnType("date");
            entity.Property(e => e.InstituteName)
                .IsUnicode(false)
                .HasColumnName("Institute_Name");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("date");
            entity.Property(e => e.Province).IsUnicode(false);
            entity.Property(e => e.State).IsUnicode(false);
            entity.Property(e => e.Zip).IsUnicode(false);
        });

        modelBuilder.Entity<InstituteMaster>(entity =>
        {
            entity.HasKey(e => e.InstId);

            entity.ToTable("InstituteMaster", tb => tb.HasTrigger("trg_UpdateOTP"));

            entity.Property(e => e.InstId).HasColumnName("InstID");
            entity.Property(e => e.ApprovedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.CountryOfIncorporation).IsUnicode(false);
            entity.Property(e => e.DateFormat)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EmailAddress).IsUnicode(false);
            entity.Property(e => e.FirstName).IsUnicode(false);
            entity.Property(e => e.Ftin)
                .IsUnicode(false)
                .HasColumnName("FTIN");
            entity.Property(e => e.Gin)
                .IsUnicode(false)
                .HasColumnName("GIN");
            entity.Property(e => e.Idnumber)
                .IsUnicode(false)
                .HasColumnName("IDNumber");
            entity.Property(e => e.Idtype)
                .IsUnicode(false)
                .HasColumnName("IDType");
            entity.Property(e => e.InstituteLogo).IsUnicode(false);
            entity.Property(e => e.InstitutionName).IsUnicode(false);
            entity.Property(e => e.LastName).IsUnicode(false);
            entity.Property(e => e.Madd1)
                .IsUnicode(false)
                .HasColumnName("MAdd1");
            entity.Property(e => e.Madd2)
                .IsUnicode(false)
                .HasColumnName("MAdd2");
            entity.Property(e => e.Mcity)
                .IsUnicode(false)
                .HasColumnName("MCity");
            entity.Property(e => e.Mcountry)
                .IsUnicode(false)
                .HasColumnName("MCountry");
            entity.Property(e => e.Mprovince)
                .IsUnicode(false)
                .HasColumnName("MProvince");
            entity.Property(e => e.Mstate)
                .IsUnicode(false)
                .HasColumnName("MState");
            entity.Property(e => e.Mzip)
                .IsUnicode(false)
                .HasColumnName("MZip");
            entity.Property(e => e.Otp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OTP");
            entity.Property(e => e.OtpexpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("OTPExpiryDate");
            entity.Property(e => e.Padd1)
                .IsUnicode(false)
                .HasColumnName("PAdd1");
            entity.Property(e => e.Padd2)
                .IsUnicode(false)
                .HasColumnName("PAdd2");
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.PasswordSecuredA1).IsUnicode(false);
            entity.Property(e => e.PasswordSecuredA2).IsUnicode(false);
            entity.Property(e => e.PasswordSecuredA3).IsUnicode(false);
            entity.Property(e => e.PasswordSecuredQ1).IsUnicode(false);
            entity.Property(e => e.PasswordSecuredQ2).IsUnicode(false);
            entity.Property(e => e.PasswordSecuredQ3).IsUnicode(false);
            entity.Property(e => e.Pcity)
                .IsUnicode(false)
                .HasColumnName("PCity");
            entity.Property(e => e.Pcountry)
                .IsUnicode(false)
                .HasColumnName("PCountry");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pprovince)
                .IsUnicode(false)
                .HasColumnName("PProvince");
            entity.Property(e => e.Pstate)
                .IsUnicode(false)
                .HasColumnName("PState");
            entity.Property(e => e.Pzip)
                .IsUnicode(false)
                .HasColumnName("PZip");
            entity.Property(e => e.RegistrationDate).HasColumnType("date");
            entity.Property(e => e.RegistrationExpiryDate).HasColumnType("date");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestIp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RequestIP");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ResetTokenExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.Status).IsUnicode(false);
            entity.Property(e => e.StatusDate).HasColumnType("date");
            entity.Property(e => e.SupportEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeofEntity).IsUnicode(false);
        });

        modelBuilder.Entity<InstitutesClient>(entity =>
        {
            entity.HasKey(e => e.ClientId);

            entity.ToTable("InstitutesClient", tb => tb.HasTrigger("trg_UpdateOTPClient"));

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Address1)
                .IsUnicode(false)
                .HasColumnName("Address 1");
            entity.Property(e => e.Address2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Address 2");
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.ClientEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ClientEmailID");
            entity.Property(e => e.ClientStatusDate).HasColumnType("datetime");
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityName).IsUnicode(false);
            entity.Property(e => e.FileName).IsUnicode(false);
            entity.Property(e => e.FormName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InActiveDate).HasColumnType("date");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Otp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OTP");
            entity.Property(e => e.OtpexpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("OTPExpiryDate");
            entity.Property(e => e.PartnerName1)
                .IsUnicode(false)
                .HasComment("Partner Name 1")
                .HasColumnName("Partner Name 1");
            entity.Property(e => e.PartnerName2)
                .IsUnicode(false)
                .HasColumnName("Partner Name 2");
            entity.Property(e => e.PhoneNumber)
                .IsUnicode(false)
                .HasColumnName("Phone Number");
            entity.Property(e => e.Province).IsUnicode(false);
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestIp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RequestIP");
            entity.Property(e => e.State).IsUnicode(false);
            entity.Property(e => e.Zip).IsUnicode(false);
        });

        modelBuilder.Entity<MasterClientStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK_MasterCStatus");

            entity.ToTable("Master_ClientStatus");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MasterEntityType>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Master_EntityType");

            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityType).IsUnicode(false);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<MasterPoboxWildcard>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Master_POBox_Wildcard");

            entity.Property(e => e.WildCard)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MasterState>(entity =>
        {
            entity.ToTable("Master_State");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StateId)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("StateID");
        });

        modelBuilder.Entity<MasterStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK_MasterStatus");

            entity.ToTable("Master_Status");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MasterUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Master_User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Email_ID");
            entity.Property(e => e.ExpiryDate).HasColumnType("date");
            entity.Property(e => e.Justification).IsUnicode(false);
            entity.Property(e => e.NameofEntity).IsUnicode(false);
            entity.Property(e => e.NameofIndividual1).IsUnicode(false);
            entity.Property(e => e.NameofIndividual2).IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.RequestIp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RequestIP");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ZipPostalCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ZIP_PostalCode");
        });

        modelBuilder.Entity<MasterUserType>(entity =>
        {
            entity.HasKey(e => e.TypeId);

            entity.ToTable("Master_UserType");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.IconClass)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Link)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MstrCountry>(entity =>
        {
            entity.ToTable("MSTR_Country");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryId)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CountryID");
            entity.Property(e => e.Favorite)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Otphistory>(entity =>
        {
            entity.ToTable("OTPHistory");

            entity.Property(e => e.EmailAddress).HasMaxLength(50);
            entity.Property(e => e.EntryDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Otp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OTP");
            entity.Property(e => e.OtpexpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("OTPExpiryDate");
            entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PageChecking>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PageChecking");

            entity.Property(e => e.CurrentPage).IsUnicode(false);
            entity.Property(e => e.PreviousPage).IsUnicode(false);
        });

        modelBuilder.Entity<PasswordChangeLog>(entity =>
        {
            entity.HasKey(e => new { e.Password, e.EmailId }).HasName("PK_PasswordChangeLog_1");

            entity.ToTable("PasswordChangeLog");

            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.PassId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PassID");
            entity.Property(e => e.PasswordChangedDate).HasColumnType("date");
        });

        modelBuilder.Entity<PasswordSecurityQuestion>(entity =>
        {
            entity.Property(e => e.PasswordSecurityQuestionId).HasColumnName("PasswordSecurityQuestionID");
            entity.Property(e => e.SecurityQuestion).IsUnicode(false);
        });

        modelBuilder.Entity<TblCountry>(entity =>
        {
            entity.HasKey(e => e.CountryId);

            entity.ToTable("tblCountries");

            entity.Property(e => e.CountryId)
                .ValueGeneratedNever()
                .HasColumnName("CountryID");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CountryName).IsUnicode(false);
            entity.Property(e => e.SortOrder)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<TblLicense>(entity =>
        {
            entity.HasKey(e => e.LicenseId);

            entity.ToTable("tblLicense");

            entity.Property(e => e.LicenseId)
                .ValueGeneratedNever()
                .HasColumnName("LicenseID");
            entity.Property(e => e.ActivationId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('00000-00000-00000-00000-00000')")
                .HasColumnName("ActivationID");
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.AddressOther)
                .HasColumnType("text")
                .HasColumnName("Address_Other");
            entity.Property(e => e.City).HasMaxLength(30);
            entity.Property(e => e.ClientName).HasMaxLength(50);
            entity.Property(e => e.CountryName).HasMaxLength(50);
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasDefaultValueSql("(N'none')");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.EntryDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.ExpiryDate)
                .HasDefaultValueSql("(dateadd(year,(1),getdate()))")
                .HasColumnType("date");
            entity.Property(e => e.IsLicensed)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isLicensed");
            entity.Property(e => e.PhoneNo).HasMaxLength(50);
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ProductID");
            entity.Property(e => e.Province)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.StateId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StateID");
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("((2))")
                .HasColumnName("StatusID");
            entity.Property(e => e.Version)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('0.0.0')");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<TblLoginHistory>(entity =>
        {
            entity.HasKey(e => e.TrNo);

            entity.ToTable("tblLoginHistory");

            entity.Property(e => e.LoginDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.LoginUserId).HasColumnName("LoginUserID");
            entity.Property(e => e.LoginUserIp)
                .HasMaxLength(50)
                .HasColumnName("LoginUserIP");
            entity.Property(e => e.LoginUserLocation).HasMaxLength(50);
            entity.Property(e => e.LoginUserQuery).HasColumnType("text");

            entity.HasOne(d => d.LoginUser).WithMany(p => p.TblLoginHistories)
                .HasForeignKey(d => d.LoginUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblLoginHistory_tblUsers");
        });

        modelBuilder.Entity<TblStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("tblStatus");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(15)
                .IsFixedLength();
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_tblMyUser");

            entity.ToTable("tblUsers");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("((0))")
                .HasColumnName("StatusID");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Group).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_tblUsers_tblGroups");
        });

        modelBuilder.Entity<TblUserGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK_syGroups");

            entity.ToTable("tblUserGroups");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.GroupName).HasMaxLength(25);
        });

        modelBuilder.Entity<TblW8benform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W8_BEN");

            entity.ToTable("tblW8BENForm");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArticleAndParagraph)
                .IsUnicode(false)
                .HasColumnName("Article and paragraph");
            entity.Property(e => e.CheckIfFtinNotLegallyRequiredYN).HasColumnName("Check if FTIN not legally required (Y/N)");
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfCitizenship)
                .IsUnicode(false)
                .HasColumnName("Country of citizenship");
            entity.Property(e => e.DateOfBirthMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Date of birth (MM-DD-YYYY)");
            entity.Property(e => e.EligibleForTheRateOfWithholding)
                .IsUnicode(false)
                .HasColumnName("eligible for the rate of withholding");
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MCountry)
                .IsUnicode(false)
                .HasColumnName("M_Country");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.NameOfIndividual)
                .IsUnicode(false)
                .HasColumnName("Name of Individual");
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.Rate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("% rate");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.ResidentCertification)
                .IsUnicode(false)
                .HasColumnName("Resident_Certification");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.SpecifyTypeOfIncome)
                .IsUnicode(false)
                .HasColumnName("specify type of income");
            entity.Property(e => e.SsnOrItin)
                .IsUnicode(false)
                .HasColumnName("SSN or ITIN");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .IsFixedLength();
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.W8benemailAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W8BENEmailAddress");
            entity.Property(e => e.W8benentryDate)
                .HasColumnType("date")
                .HasColumnName("W8BENEntryDate");
            entity.Property(e => e.W8benfontName)
                .IsUnicode(false)
                .HasColumnName("W8BENFontName");
            entity.Property(e => e.W8benonBehalfName).HasColumnName("W8BENOnBehalfName");
            entity.Property(e => e.W8benprintName)
                .IsUnicode(false)
                .HasColumnName("W8BENPrintName");
            entity.Property(e => e.W8benprintSize).HasColumnName("W8BENPrintSize");
        });

        modelBuilder.Entity<TblW8ebeneform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W8_EBENE");

            entity.ToTable("tblW8EBENEForm");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActiveTabIndex)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddressRow1).IsUnicode(false);
            entity.Property(e => e.AddressRow2).IsUnicode(false);
            entity.Property(e => e.AddressRow3).IsUnicode(false);
            entity.Property(e => e.AddressRow4).IsUnicode(false);
            entity.Property(e => e.AddressRow5).IsUnicode(false);
            entity.Property(e => e.AddressRow6).IsUnicode(false);
            entity.Property(e => e.AddressRow7).IsUnicode(false);
            entity.Property(e => e.AddressRow8).IsUnicode(false);
            entity.Property(e => e.AddressRow9).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfIncorporation)
                .IsUnicode(false)
                .HasColumnName("Country of incorporation");
            entity.Property(e => e.FatcaStatus)
                .IsUnicode(false)
                .HasColumnName("FATCA Status");
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.FtinCheck).HasColumnName("FTIN_CHECK");
            entity.Property(e => e.Gin)
                .IsUnicode(false)
                .HasColumnName("GIN");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MCountry)
                .IsUnicode(false)
                .HasColumnName("M_Country");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.NameOfDiregardedEntity)
                .IsUnicode(false)
                .HasColumnName("Name of Diregarded Entity");
            entity.Property(e => e.NameOfOrganization)
                .IsUnicode(false)
                .HasColumnName("Name of Organization");
            entity.Property(e => e.NameRow1).IsUnicode(false);
            entity.Property(e => e.NameRow2).IsUnicode(false);
            entity.Property(e => e.NameRow3).IsUnicode(false);
            entity.Property(e => e.NameRow4).IsUnicode(false);
            entity.Property(e => e.NameRow5).IsUnicode(false);
            entity.Property(e => e.NameRow6).IsUnicode(false);
            entity.Property(e => e.NameRow7).IsUnicode(false);
            entity.Property(e => e.NameRow8).IsUnicode(false);
            entity.Property(e => e.NameRow9).IsUnicode(false);
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .IsFixedLength();
            entity.Property(e => e.Tin1row7)
                .IsUnicode(false)
                .HasColumnName("TIN1Row7");
            entity.Property(e => e.Tinrow1)
                .IsUnicode(false)
                .HasColumnName("TINRow1");
            entity.Property(e => e.Tinrow2)
                .IsUnicode(false)
                .HasColumnName("TINRow2");
            entity.Property(e => e.Tinrow3)
                .IsUnicode(false)
                .HasColumnName("TINRow3");
            entity.Property(e => e.Tinrow4)
                .IsUnicode(false)
                .HasColumnName("TINRow4");
            entity.Property(e => e.Tinrow5)
                .IsUnicode(false)
                .HasColumnName("TINRow5");
            entity.Property(e => e.Tinrow6)
                .IsUnicode(false)
                .HasColumnName("TINRow6");
            entity.Property(e => e.Tinrow8)
                .IsUnicode(false)
                .HasColumnName("TINRow8");
            entity.Property(e => e.Tinrow9)
                .IsUnicode(false)
                .HasColumnName("TINRow9");
            entity.Property(e => e.TypeOfEntity)
                .IsUnicode(false)
                .HasColumnName("Type of Entity");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.UsTin)
                .IsUnicode(false)
                .HasColumnName("US TIN");
            entity.Property(e => e.W8beneemailAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("W8BENEEmailAddress");
            entity.Property(e => e.W8beneentryDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W8BENEEntryDate");
            entity.Property(e => e.W8benefontName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("W8BENEFontName");
            entity.Property(e => e.W8beneonBehalfName).HasColumnName("W8BENEOnBehalfName");
            entity.Property(e => e.W8beneprintName)
                .IsUnicode(false)
                .HasColumnName("W8BENEPrintName");
            entity.Property(e => e.W8beneprintSize)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W8BENEPrintSize");
            entity.Property(e => e._11fatcaCb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("11FATCA_CB");
            entity.Property(e => e._12City)
                .IsUnicode(false)
                .HasColumnName("12_City");
            entity.Property(e => e._12Country)
                .IsUnicode(false)
                .HasColumnName("12_Country");
            entity.Property(e => e._12mailingAddress)
                .IsUnicode(false)
                .HasColumnName("12Mailing address");
            entity.Property(e => e._13gin)
                .IsUnicode(false)
                .HasColumnName("13GIN");
            entity.Property(e => e._14aCb).HasColumnName("14aCB");
            entity.Property(e => e._14aTb)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("14aTB");
            entity.Property(e => e._14bCb1).HasColumnName("14bCB1");
            entity.Property(e => e._14bCb2others)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("14bCB2others");
            entity.Property(e => e._14bTb)
                .IsUnicode(false)
                .HasColumnName("14bTB");
            entity.Property(e => e._14cCb).HasColumnName("14cCB");
            entity.Property(e => e._15cTb1)
                .IsUnicode(false)
                .HasColumnName("15cTB1");
            entity.Property(e => e._15cTb2)
                .IsUnicode(false)
                .HasColumnName("15cTB2");
            entity.Property(e => e._15cTb3)
                .IsUnicode(false)
                .HasColumnName("15cTB3");
            entity.Property(e => e._15cTb4)
                .IsUnicode(false)
                .HasColumnName("15cTB4");
            entity.Property(e => e._16tb)
                .IsUnicode(false)
                .HasColumnName("16TB");
            entity.Property(e => e._17cb1).HasColumnName("17CB1");
            entity.Property(e => e._17cb2).HasColumnName("17CB2");
            entity.Property(e => e._18cb).HasColumnName("18CB");
            entity.Property(e => e._19cb).HasColumnName("19CB");
            entity.Property(e => e._20tb)
                .IsUnicode(false)
                .HasColumnName("20TB");
            entity.Property(e => e._21cb).HasColumnName("21CB");
            entity.Property(e => e._22cb).HasColumnName("22CB");
            entity.Property(e => e._23cb).HasColumnName("23CB");
            entity.Property(e => e._24aCb).HasColumnName("24aCB");
            entity.Property(e => e._24borcCb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("24borcCB");
            entity.Property(e => e._24dCb).HasColumnName("24dCB");
            entity.Property(e => e._25aCb).HasColumnName("25aCB");
            entity.Property(e => e._25bcCb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("25bcCB");
            entity.Property(e => e._26cb1).HasColumnName("26CB1");
            entity.Property(e => e._26cb2or3)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("26CB2or3");
            entity.Property(e => e._26cb4or5)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("26CB4or5");
            entity.Property(e => e._26tb1)
                .IsUnicode(false)
                .HasColumnName("26TB1");
            entity.Property(e => e._26tb2)
                .IsUnicode(false)
                .HasColumnName("26TB2");
            entity.Property(e => e._26tb3)
                .IsUnicode(false)
                .HasColumnName("26TB3");
            entity.Property(e => e._27cb).HasColumnName("27CB");
            entity.Property(e => e._28aorbCb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("28aorbCB");
            entity.Property(e => e._29cb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("29CB");
            entity.Property(e => e._30cb).HasColumnName("30CB");
            entity.Property(e => e._31cb).HasColumnName("31CB");
            entity.Property(e => e._32cb).HasColumnName("32CB");
            entity.Property(e => e._33cb).HasColumnName("33CB");
            entity.Property(e => e._33tb)
                .IsUnicode(false)
                .HasColumnName("33TB");
            entity.Property(e => e._34cb).HasColumnName("34CB");
            entity.Property(e => e._34tb)
                .IsUnicode(false)
                .HasColumnName("34TB");
            entity.Property(e => e._35cb).HasColumnName("35CB");
            entity.Property(e => e._35tb)
                .IsUnicode(false)
                .HasColumnName("35TB");
            entity.Property(e => e._36cb).HasColumnName("36CB");
            entity.Property(e => e._37aTb)
                .IsUnicode(false)
                .HasColumnName("37aTB");
            entity.Property(e => e._37aorbCb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("37aorbCB");
            entity.Property(e => e._37bTb1)
                .IsUnicode(false)
                .HasColumnName("37bTB1");
            entity.Property(e => e._37bTb2)
                .IsUnicode(false)
                .HasColumnName("37bTB2");
            entity.Property(e => e._38cb).HasColumnName("38CB");
            entity.Property(e => e._39cb).HasColumnName("39CB");
            entity.Property(e => e._40aCb).HasColumnName("40aCB");
            entity.Property(e => e._40borcCb)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("40borcCB");
            entity.Property(e => e._41cb).HasColumnName("41CB");
            entity.Property(e => e._42tb)
                .IsUnicode(false)
                .HasColumnName("42TB");
            entity.Property(e => e._43cb).HasColumnName("43CB");
        });

        modelBuilder.Entity<TblW8ebeneformBackup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W8_EBENE_BACKUP");

            entity.ToTable("tblW8EBENEForm_backup");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address1).IsUnicode(false);
            entity.Property(e => e.Address11).IsUnicode(false);
            entity.Property(e => e.Address111).IsUnicode(false);
            entity.Property(e => e.Address12).IsUnicode(false);
            entity.Property(e => e.Address121).IsUnicode(false);
            entity.Property(e => e.Address13).IsUnicode(false);
            entity.Property(e => e.Address131).IsUnicode(false);
            entity.Property(e => e.Address1311).IsUnicode(false);
            entity.Property(e => e.Address14).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfIncorporation)
                .IsUnicode(false)
                .HasColumnName("Country of incorporation");
            entity.Property(e => e.FatcaStatus)
                .IsUnicode(false)
                .HasColumnName("FATCA Status");
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.FtinCheck)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTIN_CHECK");
            entity.Property(e => e.Gin)
                .IsUnicode(false)
                .HasColumnName("GIN");
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MCountry)
                .IsUnicode(false)
                .HasColumnName("M_Country");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.Name1).IsUnicode(false);
            entity.Property(e => e.Name11).IsUnicode(false);
            entity.Property(e => e.Name111).IsUnicode(false);
            entity.Property(e => e.Name12).IsUnicode(false);
            entity.Property(e => e.Name121).IsUnicode(false);
            entity.Property(e => e.Name13).IsUnicode(false);
            entity.Property(e => e.Name131).IsUnicode(false);
            entity.Property(e => e.Name1311).IsUnicode(false);
            entity.Property(e => e.Name14).IsUnicode(false);
            entity.Property(e => e.NameOfDiregardedEntity)
                .IsUnicode(false)
                .HasColumnName("Name of Diregarded Entity");
            entity.Property(e => e.NameOfOrganization)
                .IsUnicode(false)
                .HasColumnName("Name of Organization");
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Tin1)
                .IsUnicode(false)
                .HasColumnName("TIN1");
            entity.Property(e => e.Tin11)
                .IsUnicode(false)
                .HasColumnName("TIN11");
            entity.Property(e => e.Tin111)
                .IsUnicode(false)
                .HasColumnName("TIN111");
            entity.Property(e => e.Tin12)
                .IsUnicode(false)
                .HasColumnName("TIN12");
            entity.Property(e => e.Tin121)
                .IsUnicode(false)
                .HasColumnName("TIN121");
            entity.Property(e => e.Tin13)
                .IsUnicode(false)
                .HasColumnName("TIN13");
            entity.Property(e => e.Tin131)
                .IsUnicode(false)
                .HasColumnName("TIN131");
            entity.Property(e => e.Tin1311)
                .IsUnicode(false)
                .HasColumnName("TIN1311");
            entity.Property(e => e.Tin14)
                .IsUnicode(false)
                .HasColumnName("TIN14");
            entity.Property(e => e.TypeOfEntity)
                .IsUnicode(false)
                .HasColumnName("Type of Entity");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.UsTin)
                .IsUnicode(false)
                .HasColumnName("US TIN");
            entity.Property(e => e._11fatcaCb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("11FATCA_CB");
            entity.Property(e => e._12City)
                .IsUnicode(false)
                .HasColumnName("12_City");
            entity.Property(e => e._12Country)
                .IsUnicode(false)
                .HasColumnName("12_Country");
            entity.Property(e => e._12mailingAddress)
                .IsUnicode(false)
                .HasColumnName("12Mailing address");
            entity.Property(e => e._13gin)
                .IsUnicode(false)
                .HasColumnName("13GIN");
            entity.Property(e => e._14aTextWithCb)
                .IsUnicode(false)
                .HasColumnName("14a_Text with CB");
            entity.Property(e => e._14b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("14b");
            entity.Property(e => e._14c)
                .IsUnicode(false)
                .HasColumnName("14c");
            entity.Property(e => e._15a)
                .IsUnicode(false)
                .HasColumnName("15a");
            entity.Property(e => e._15b)
                .IsUnicode(false)
                .HasColumnName("15b");
            entity.Property(e => e._15c)
                .IsUnicode(false)
                .HasColumnName("15c");
            entity.Property(e => e._15d)
                .IsUnicode(false)
                .HasColumnName("15d");
            entity.Property(e => e._15e)
                .IsUnicode(false)
                .HasColumnName("15e");
            entity.Property(e => e._16)
                .IsUnicode(false)
                .HasColumnName("16");
            entity.Property(e => e._17a)
                .IsUnicode(false)
                .HasColumnName("17a");
            entity.Property(e => e._17b)
                .IsUnicode(false)
                .HasColumnName("17b");
            entity.Property(e => e._18)
                .IsUnicode(false)
                .HasColumnName("18");
            entity.Property(e => e._19)
                .IsUnicode(false)
                .HasColumnName("19");
            entity.Property(e => e._20Text)
                .IsUnicode(false)
                .HasColumnName("20_Text");
            entity.Property(e => e._21)
                .IsUnicode(false)
                .HasColumnName("21");
            entity.Property(e => e._22)
                .IsUnicode(false)
                .HasColumnName("22");
            entity.Property(e => e._23)
                .IsUnicode(false)
                .HasColumnName("23");
            entity.Property(e => e._24a)
                .IsUnicode(false)
                .HasColumnName("24a");
            entity.Property(e => e._24b)
                .IsUnicode(false)
                .HasColumnName("24b");
            entity.Property(e => e._24c)
                .IsUnicode(false)
                .HasColumnName("24c");
            entity.Property(e => e._24d)
                .IsUnicode(false)
                .HasColumnName("24d");
            entity.Property(e => e._25a)
                .IsUnicode(false)
                .HasColumnName("25a");
            entity.Property(e => e._25b)
                .IsUnicode(false)
                .HasColumnName("25b");
            entity.Property(e => e._25c)
                .IsUnicode(false)
                .HasColumnName("25c");
            entity.Property(e => e._26)
                .IsUnicode(false)
                .HasColumnName("26");
            entity.Property(e => e._26Cb)
                .IsUnicode(false)
                .HasColumnName("26_CB");
            entity.Property(e => e._26Cb2)
                .IsUnicode(false)
                .HasColumnName("26_CB2");
            entity.Property(e => e._26Text)
                .IsUnicode(false)
                .HasColumnName("26_Text");
            entity.Property(e => e._26Text2)
                .IsUnicode(false)
                .HasColumnName("26_Text2");
            entity.Property(e => e._26Text3)
                .IsUnicode(false)
                .HasColumnName("26_Text3");
            entity.Property(e => e._27)
                .IsUnicode(false)
                .HasColumnName("27");
            entity.Property(e => e._28a)
                .IsUnicode(false)
                .HasColumnName("28a");
            entity.Property(e => e._28b)
                .IsUnicode(false)
                .HasColumnName("28b");
            entity.Property(e => e._29a)
                .IsUnicode(false)
                .HasColumnName("29a");
            entity.Property(e => e._29b)
                .IsUnicode(false)
                .HasColumnName("29b");
            entity.Property(e => e._29c)
                .IsUnicode(false)
                .HasColumnName("29c");
            entity.Property(e => e._29d)
                .IsUnicode(false)
                .HasColumnName("29d");
            entity.Property(e => e._29e)
                .IsUnicode(false)
                .HasColumnName("29e");
            entity.Property(e => e._29f)
                .IsUnicode(false)
                .HasColumnName("29f");
            entity.Property(e => e._30)
                .IsUnicode(false)
                .HasColumnName("30");
            entity.Property(e => e._31)
                .IsUnicode(false)
                .HasColumnName("31");
            entity.Property(e => e._32)
                .IsUnicode(false)
                .HasColumnName("32");
            entity.Property(e => e._33)
                .IsUnicode(false)
                .HasColumnName("33");
            entity.Property(e => e._34TextWithCb)
                .IsUnicode(false)
                .HasColumnName("34 Text with CB");
            entity.Property(e => e._35TextWithCb)
                .IsUnicode(false)
                .HasColumnName("35 Text with CB");
            entity.Property(e => e._36)
                .IsUnicode(false)
                .HasColumnName("36");
            entity.Property(e => e._37aTextWithCb)
                .IsUnicode(false)
                .HasColumnName("37a Text with CB");
            entity.Property(e => e._37bTextWithCb)
                .IsUnicode(false)
                .HasColumnName("37b Text with CB");
            entity.Property(e => e._38)
                .IsUnicode(false)
                .HasColumnName("38");
            entity.Property(e => e._39)
                .IsUnicode(false)
                .HasColumnName("39");
            entity.Property(e => e._40a)
                .IsUnicode(false)
                .HasColumnName("40a");
            entity.Property(e => e._40b)
                .IsUnicode(false)
                .HasColumnName("40b");
            entity.Property(e => e._40c)
                .IsUnicode(false)
                .HasColumnName("40c");
            entity.Property(e => e._41)
                .IsUnicode(false)
                .HasColumnName("41");
            entity.Property(e => e._42Text)
                .IsUnicode(false)
                .HasColumnName("42_Text");
            entity.Property(e => e._43)
                .IsUnicode(false)
                .HasColumnName("43");
        });

        modelBuilder.Entity<TblW8eciform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W8_ECI");

            entity.ToTable("tblW8ECIForm");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActiveTabIndex)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CheckIfFtinNotLegallyRequiredYN).HasColumnName("Check if FTIN not legally required (Y/N)");
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfIncorporation)
                .IsUnicode(false)
                .HasColumnName("Country of incorporation");
            entity.Property(e => e.DateOfBirthMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Date of birth (MM-DD-YYYY)");
            entity.Property(e => e.DisregardedEntity)
                .IsUnicode(false)
                .HasColumnName("Disregarded Entity");
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Items).IsUnicode(false);
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.NameOfIndividual)
                .IsUnicode(false)
                .HasColumnName("Name of Individual");
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.SsnOrItin)
                .IsUnicode(false)
                .HasColumnName("SSN or ITIN");
            entity.Property(e => e.Ssnitnein)
                .IsUnicode(false)
                .HasColumnName("SSNITNEIN");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .IsFixedLength();
            entity.Property(e => e.TypeOfEntity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Type Of Entity");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.W8eciemailAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W8ECIEmailAddress");
            entity.Property(e => e.W8ecientryDate)
                .HasColumnType("date")
                .HasColumnName("W8ECIEntryDate");
            entity.Property(e => e.W8ecifontName)
                .IsUnicode(false)
                .HasColumnName("W8ECIFontName");
            entity.Property(e => e.W8ecionBehalfName).HasColumnName("W8ECIOnBehalfName");
            entity.Property(e => e.W8eciprintName)
                .IsUnicode(false)
                .HasColumnName("W8ECIPrintName");
            entity.Property(e => e.W8eciprintSize).HasColumnName("W8ECIPrintSize");
        });

        modelBuilder.Entity<TblW8expform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W8_EXP");

            entity.ToTable("tblW8EXPForm");

            entity.HasIndex(e => e.EmailAddress, "idx_W8EXpEmailAddress");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActiveTabIndex)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CheckIfFtinNotLegallyRequiredYN).HasColumnName("Check if FTIN not legally required (Y/N)");
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfIncorporation)
                .IsUnicode(false)
                .HasColumnName("Country of incorporation");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FatcaStatus)
                .IsUnicode(false)
                .HasColumnName("FATCA Status");
            entity.Property(e => e.FontName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.Gin)
                .IsUnicode(false)
                .HasColumnName("GIN");
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MCountry)
                .IsUnicode(false)
                .HasColumnName("M_Country");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.NameOfOrganization)
                .IsUnicode(false)
                .HasColumnName("Name of Organization");
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.SsnOrItin)
                .IsUnicode(false)
                .HasColumnName("SSN or ITIN");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .IsFixedLength();
            entity.Property(e => e.TypeOfEntity)
                .IsUnicode(false)
                .HasColumnName("Type of Entity");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e._10a).HasColumnName("10a");
            entity.Property(e => e._10b).HasColumnName("10b");
            entity.Property(e => e._10bText)
                .IsUnicode(false)
                .HasColumnName("10b_Text");
            entity.Property(e => e._10c).HasColumnName("10c");
            entity.Property(e => e._10cText)
                .IsUnicode(false)
                .HasColumnName("10c_Text");
            entity.Property(e => e._11).HasColumnName("11");
            entity.Property(e => e._12).HasColumnName("12");
            entity.Property(e => e._13a).HasColumnName("13a");
            entity.Property(e => e._13aText)
                .IsUnicode(false)
                .HasColumnName("13a_Text");
            entity.Property(e => e._13b).HasColumnName("13b");
            entity.Property(e => e._13c).HasColumnName("13c");
            entity.Property(e => e._13d).HasColumnName("13d");
            entity.Property(e => e._14).HasColumnName("14");
            entity.Property(e => e._15).HasColumnName("15");
            entity.Property(e => e._15Text1)
                .IsUnicode(false)
                .HasColumnName("15_Text1");
            entity.Property(e => e._15Text2)
                .IsUnicode(false)
                .HasColumnName("15_Text2");
            entity.Property(e => e._15Text3)
                .IsUnicode(false)
                .HasColumnName("15_Text3");
            entity.Property(e => e._16).HasColumnName("16");
            entity.Property(e => e._17).HasColumnName("17");
            entity.Property(e => e._18).HasColumnName("18");
            entity.Property(e => e._19).HasColumnName("19");
            entity.Property(e => e._20a).HasColumnName("20a");
            entity.Property(e => e._20b).HasColumnName("20b");
            entity.Property(e => e._20c).HasColumnName("20c");
            entity.Property(e => e._21).HasColumnName("21");
            entity.Property(e => e._21Text)
                .IsUnicode(false)
                .HasColumnName("21_Text");
        });

        modelBuilder.Entity<TblW8imyform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W8_IMY");

            entity.ToTable("tblW8IMYForm");

            entity.HasIndex(e => e.EmailAddress, "idx_ImyEmailAddress");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActiveTabIndex).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfIncorporation)
                .IsUnicode(false)
                .HasColumnName("Country of incorporation");
            entity.Property(e => e.De).HasColumnName("DE");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FatcaStatus)
                .IsUnicode(false)
                .HasColumnName("FATCA Status");
            entity.Property(e => e.FilePath).IsUnicode(false);
            entity.Property(e => e.FontName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ForeignNumberCb).HasColumnName("ForeignNumber_CB");
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.Gin)
                .IsUnicode(false)
                .HasColumnName("GIN");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MCountry)
                .IsUnicode(false)
                .HasColumnName("M_Country");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.NameOfDiregardedEntity)
                .IsUnicode(false)
                .HasColumnName("Name of Diregarded Entity");
            entity.Property(e => e.NameOfOrganization)
                .IsUnicode(false)
                .HasColumnName("Name of Organization");
            entity.Property(e => e.OnBehalfName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .IsFixedLength();
            entity.Property(e => e.TypeOfEntity)
                .IsUnicode(false)
                .HasColumnName("Type of Entity");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.UsTin)
                .IsUnicode(false)
                .HasColumnName("US TIN");
            entity.Property(e => e.UsTinCb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("US_TIN_CB");
            entity.Property(e => e._11fatcaCb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("11FATCA_CB");
            entity.Property(e => e._12City)
                .IsUnicode(false)
                .HasColumnName("12_City");
            entity.Property(e => e._12Country)
                .IsUnicode(false)
                .HasColumnName("12_Country");
            entity.Property(e => e._12Province)
                .IsUnicode(false)
                .HasColumnName("12_Province");
            entity.Property(e => e._12State)
                .IsUnicode(false)
                .HasColumnName("12_State");
            entity.Property(e => e._12mailingAddress)
                .IsUnicode(false)
                .HasColumnName("12Mailing address");
            entity.Property(e => e._13gin)
                .IsUnicode(false)
                .HasColumnName("13GIN");
            entity.Property(e => e._14Cb).HasColumnName("14_CB");
            entity.Property(e => e._15a).HasColumnName("15a");
            entity.Property(e => e._15b).HasColumnName("15b");
            entity.Property(e => e._15c).HasColumnName("15c");
            entity.Property(e => e._15d).HasColumnName("15d");
            entity.Property(e => e._15e).HasColumnName("15e");
            entity.Property(e => e._15f).HasColumnName("15f");
            entity.Property(e => e._15g).HasColumnName("15g");
            entity.Property(e => e._15h).HasColumnName("15h");
            entity.Property(e => e._15i).HasColumnName("15i");
            entity.Property(e => e._16a).HasColumnName("16a");
            entity.Property(e => e._16b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("16b");
            entity.Property(e => e._17a).HasColumnName("17a");
            entity.Property(e => e._17b).HasColumnName("17b");
            entity.Property(e => e._17c).HasColumnName("17c");
            entity.Property(e => e._17d).HasColumnName("17d");
            entity.Property(e => e._17e).HasColumnName("17e");
            entity.Property(e => e._18a).HasColumnName("18a");
            entity.Property(e => e._18b).HasColumnName("18b");
            entity.Property(e => e._18c).HasColumnName("18c");
            entity.Property(e => e._18d).HasColumnName("18d");
            entity.Property(e => e._18e).HasColumnName("18e");
            entity.Property(e => e._18f).HasColumnName("18f");
            entity.Property(e => e._19a).HasColumnName("19a");
            entity.Property(e => e._19b).HasColumnName("19b");
            entity.Property(e => e._19c).HasColumnName("19c");
            entity.Property(e => e._19d).HasColumnName("19d");
            entity.Property(e => e._19e).HasColumnName("19e");
            entity.Property(e => e._19f).HasColumnName("19f");
            entity.Property(e => e._20).HasColumnName("20");
            entity.Property(e => e._21a).HasColumnName("21a");
            entity.Property(e => e._21b).HasColumnName("21b");
            entity.Property(e => e._21c).HasColumnName("21c");
            entity.Property(e => e._21d).HasColumnName("21d");
            entity.Property(e => e._21e).HasColumnName("21e");
            entity.Property(e => e._21f).HasColumnName("21f");
            entity.Property(e => e._22).HasColumnName("22");
            entity.Property(e => e._23aText)
                .IsUnicode(false)
                .HasColumnName("23a_Text");
            entity.Property(e => e._23b).HasColumnName("23b");
            entity.Property(e => e._23c).HasColumnName("23c");
            entity.Property(e => e._24a).HasColumnName("24a");
            entity.Property(e => e._24b).HasColumnName("24b");
            entity.Property(e => e._24c).HasColumnName("24c");
            entity.Property(e => e._25).HasColumnName("25");
            entity.Property(e => e._26).HasColumnName("26");
            entity.Property(e => e._27aText)
                .IsUnicode(false)
                .HasColumnName("27a_Text");
            entity.Property(e => e._27b).HasColumnName("27b");
            entity.Property(e => e._28).HasColumnName("28");
            entity.Property(e => e._29).HasColumnName("29");
            entity.Property(e => e._30a).HasColumnName("30a");
            entity.Property(e => e._30b).HasColumnName("30b");
            entity.Property(e => e._30c).HasColumnName("30c");
            entity.Property(e => e._31).HasColumnName("31");
            entity.Property(e => e._32Cb1).HasColumnName("32_CB1");
            entity.Property(e => e._32Cb2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("32_CB2");
            entity.Property(e => e._32Cb3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("32_CB3");
            entity.Property(e => e._32Text1)
                .IsUnicode(false)
                .HasColumnName("32_Text1");
            entity.Property(e => e._32Text2)
                .IsUnicode(false)
                .HasColumnName("32_Text2");
            entity.Property(e => e._32Text3)
                .IsUnicode(false)
                .HasColumnName("32_Text3");
            entity.Property(e => e._33a).HasColumnName("33a");
            entity.Property(e => e._33b).HasColumnName("33b");
            entity.Property(e => e._33c).HasColumnName("33c");
            entity.Property(e => e._33d).HasColumnName("33d");
            entity.Property(e => e._33e).HasColumnName("33e");
            entity.Property(e => e._33f).HasColumnName("33f");
            entity.Property(e => e._34).HasColumnName("34");
            entity.Property(e => e._35Cb).HasColumnName("35_CB");
            entity.Property(e => e._35Text)
                .HasColumnType("datetime")
                .HasColumnName("35_Text");
            entity.Property(e => e._36).HasColumnName("36");
            entity.Property(e => e._36Text)
                .HasColumnType("datetime")
                .HasColumnName("36_Text");
            entity.Property(e => e._37aCb).HasColumnName("37a_CB");
            entity.Property(e => e._37aText)
                .IsUnicode(false)
                .HasColumnName("37a_Text");
            entity.Property(e => e._37bCb).HasColumnName("37b_CB");
            entity.Property(e => e._37bText1)
                .IsUnicode(false)
                .HasColumnName("37b_Text1");
            entity.Property(e => e._37bText2)
                .IsUnicode(false)
                .HasColumnName("37b_Text2");
            entity.Property(e => e._38).HasColumnName("38");
            entity.Property(e => e._39).HasColumnName("39");
            entity.Property(e => e._40).HasColumnName("40");
            entity.Property(e => e._41Text)
                .IsUnicode(false)
                .HasColumnName("41_Text");
            entity.Property(e => e._42Cb).HasColumnName("42_CB");
        });

        modelBuilder.Entity<TblW8imyformBckup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblW8IMYForm_bckup");

            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryOfIncorporation)
                .IsUnicode(false)
                .HasColumnName("Country of incorporation");
            entity.Property(e => e.FatcaStatus)
                .IsUnicode(false)
                .HasColumnName("FATCA Status");
            entity.Property(e => e.ForeignTaxIdentifyingNumber)
                .IsUnicode(false)
                .HasColumnName("Foreign tax identifying number");
            entity.Property(e => e.Gin)
                .IsUnicode(false)
                .HasColumnName("GIN");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MCity)
                .IsUnicode(false)
                .HasColumnName("M_City");
            entity.Property(e => e.MCountry)
                .IsUnicode(false)
                .HasColumnName("M_Country");
            entity.Property(e => e.MailingAddress)
                .IsUnicode(false)
                .HasColumnName("Mailing address");
            entity.Property(e => e.NameOfDiregardedEntity)
                .IsUnicode(false)
                .HasColumnName("Name of Diregarded Entity");
            entity.Property(e => e.NameOfOrganization)
                .IsUnicode(false)
                .HasColumnName("Name of Organization");
            entity.Property(e => e.PermanentResidenceAddress)
                .IsUnicode(false)
                .HasColumnName("Permanent residence address");
            entity.Property(e => e.PrintNameOfSigner)
                .IsUnicode(false)
                .HasColumnName("Print name of signer");
            entity.Property(e => e.ReferenceNumberS)
                .IsUnicode(false)
                .HasColumnName("Reference number(s)");
            entity.Property(e => e.SignatureDateMmDdYyyy)
                .IsUnicode(false)
                .HasColumnName("Signature Date (MM-DD-YYYY)");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TypeOfEntity)
                .IsUnicode(false)
                .HasColumnName("Type of Entity");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.UsTin)
                .IsUnicode(false)
                .HasColumnName("US TIN");
            entity.Property(e => e.UsTinCb)
                .IsUnicode(false)
                .HasColumnName("US_TIN_CB");
            entity.Property(e => e._11fatcaCb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("11FATCA_CB");
            entity.Property(e => e._12City)
                .IsUnicode(false)
                .HasColumnName("12_City");
            entity.Property(e => e._12Country)
                .IsUnicode(false)
                .HasColumnName("12_Country");
            entity.Property(e => e._12mailingAddress)
                .IsUnicode(false)
                .HasColumnName("12Mailing address");
            entity.Property(e => e._13gin)
                .IsUnicode(false)
                .HasColumnName("13GIN");
            entity.Property(e => e._14Cb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("14_CB");
            entity.Property(e => e._15a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15a");
            entity.Property(e => e._15b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15b");
            entity.Property(e => e._15c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15c");
            entity.Property(e => e._15d)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15d");
            entity.Property(e => e._15e)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15e");
            entity.Property(e => e._15f)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15f");
            entity.Property(e => e._15g)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15g");
            entity.Property(e => e._15h)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15h");
            entity.Property(e => e._15i)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("15i");
            entity.Property(e => e._16a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("16a");
            entity.Property(e => e._16b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("16b");
            entity.Property(e => e._17a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("17a");
            entity.Property(e => e._17b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("17b");
            entity.Property(e => e._17c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("17c");
            entity.Property(e => e._17d)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("17d");
            entity.Property(e => e._17e)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("17e");
            entity.Property(e => e._18a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("18a");
            entity.Property(e => e._18b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("18b");
            entity.Property(e => e._18c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("18c");
            entity.Property(e => e._18d)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("18d");
            entity.Property(e => e._18e)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("18e");
            entity.Property(e => e._18f)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("18f");
            entity.Property(e => e._19a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("19a");
            entity.Property(e => e._19b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("19b");
            entity.Property(e => e._19c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("19c");
            entity.Property(e => e._19d)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("19d");
            entity.Property(e => e._19e)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("19e");
            entity.Property(e => e._19f)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("19f");
            entity.Property(e => e._20)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("20");
            entity.Property(e => e._21a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("21a");
            entity.Property(e => e._21b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("21b");
            entity.Property(e => e._21c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("21c");
            entity.Property(e => e._21d)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("21d");
            entity.Property(e => e._21e)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("21e");
            entity.Property(e => e._21f)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("21f");
            entity.Property(e => e._22)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("22");
            entity.Property(e => e._23aText)
                .IsUnicode(false)
                .HasColumnName("23a_Text");
            entity.Property(e => e._23b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("23b");
            entity.Property(e => e._23c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("23c");
            entity.Property(e => e._24a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("24a");
            entity.Property(e => e._24b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("24b");
            entity.Property(e => e._24c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("24c");
            entity.Property(e => e._25)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("25");
            entity.Property(e => e._26)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("26");
            entity.Property(e => e._27aText)
                .IsUnicode(false)
                .HasColumnName("27a_Text");
            entity.Property(e => e._27b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("27b");
            entity.Property(e => e._28)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("28");
            entity.Property(e => e._29)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("29");
            entity.Property(e => e._30a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("30a");
            entity.Property(e => e._30b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("30b");
            entity.Property(e => e._30c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("30c");
            entity.Property(e => e._31)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("31");
            entity.Property(e => e._32Cb1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("32_CB1");
            entity.Property(e => e._32Cb2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("32_CB2");
            entity.Property(e => e._32Text1)
                .IsUnicode(false)
                .HasColumnName("32_Text1");
            entity.Property(e => e._32Text2)
                .IsUnicode(false)
                .HasColumnName("32_Text2");
            entity.Property(e => e._32Text3)
                .IsUnicode(false)
                .HasColumnName("32_Text3");
            entity.Property(e => e._33a)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("33a");
            entity.Property(e => e._33b)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("33b");
            entity.Property(e => e._33c)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("33c");
            entity.Property(e => e._33d)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("33d");
            entity.Property(e => e._33e)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("33e");
            entity.Property(e => e._33f)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("33f");
            entity.Property(e => e._34)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("34");
            entity.Property(e => e._35Cb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("35_CB");
            entity.Property(e => e._35Text)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("35_Text");
            entity.Property(e => e._36)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("36");
            entity.Property(e => e._37aTextWithCb)
                .IsUnicode(false)
                .HasColumnName("37a_Text with CB");
            entity.Property(e => e._37bText1WithCb)
                .IsUnicode(false)
                .HasColumnName("37b_Text1 with CB");
            entity.Property(e => e._37bText2WithCb)
                .IsUnicode(false)
                .HasColumnName("37b_Text2 with CB");
            entity.Property(e => e._38)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("38");
            entity.Property(e => e._39)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("39");
            entity.Property(e => e._40)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("40");
            entity.Property(e => e._41Text)
                .IsUnicode(false)
                .HasColumnName("41_Text");
            entity.Property(e => e._42Cb)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("42_CB");
        });

        modelBuilder.Entity<TblW9form>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W9_2");

            entity.ToTable("tblW9Form");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.Address1).IsUnicode(false);
            entity.Property(e => e.BusinessEntity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.City1).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.Country1).IsUnicode(false);
            entity.Property(e => e.Exemptions)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fatca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FATCA");
            entity.Property(e => e.FederalTaxClassification)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ListofAccounts)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SsnTin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SSN_TIN");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .IsFixedLength();
            entity.Property(e => e.UploadedFile)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.W9emailAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("W9EmailAddress");
            entity.Property(e => e.W9entryDate)
                .HasColumnType("date")
                .HasColumnName("W9EntryDate");
            entity.Property(e => e.W9fontName)
                .IsUnicode(false)
                .HasColumnName("W9FontName");
            entity.Property(e => e.W9printName)
                .IsUnicode(false)
                .HasColumnName("W9PrintName");
            entity.Property(e => e.W9printSize).HasColumnName("W9PrintSize");
        });

        modelBuilder.Entity<TblW9formIndividual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_W9_I");

            entity.ToTable("tblW9Form_Individual");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BusinessEntity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Exemptions)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fatca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FATCA");
            entity.Property(e => e.FederalTaxClassification)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdatedon).HasColumnType("datetime");
            entity.Property(e => e.ListofAccounts)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SsnTin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SSN_TIN");
            entity.Property(e => e.UploadedFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Uploaded_File");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblxxxU>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblxxxU");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<ViewClient>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_Clients");

            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.ClientEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ClientEmailID");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.ClientStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClientStatusDate).HasColumnType("datetime");
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityName).IsUnicode(false);
            entity.Property(e => e.FileName).IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.PartnerName)
                .IsUnicode(false)
                .HasColumnName("Partner Name");
            entity.Property(e => e.PhoneNumber)
                .IsUnicode(false)
                .HasColumnName("Phone Number");
            entity.Property(e => e.Province).IsUnicode(false);
            entity.Property(e => e.State).IsUnicode(false);
            entity.Property(e => e.Zip).IsUnicode(false);
        });

        modelBuilder.Entity<W8bene14b>(entity =>
        {
            entity.HasKey(e => e._14bId);

            entity.ToTable("W8BENE14B");

            entity.Property(e => e._14bId).HasColumnName("14B_ID");
            entity.Property(e => e._14b)
                .IsUnicode(false)
                .HasColumnName("14B");
        });

        modelBuilder.Entity<W8beneEntityType>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("W8BENE_EntityType");

            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityType).IsUnicode(false);
        });

        modelBuilder.Entity<W8benefatca>(entity =>
        {
            entity.HasKey(e => e.FatcaId);

            entity.ToTable("W8BENEFATCA");

            entity.Property(e => e.FatcaId).HasColumnName("FATCA_ID");
            entity.Property(e => e.Fatca)
                .IsUnicode(false)
                .HasColumnName("FATCA");
        });

        modelBuilder.Entity<W8benefatcade>(entity =>
        {
            entity.HasKey(e => e.FatcaId);

            entity.ToTable("W8BENEFATCADE");

            entity.Property(e => e.FatcaId).HasColumnName("FATCA_ID");
            entity.Property(e => e.Fatca)
                .IsUnicode(false)
                .HasColumnName("FATCA");
        });

        modelBuilder.Entity<W8eciEntityType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("W8ECI_EntityType");

            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityType).IsUnicode(false);
        });

        modelBuilder.Entity<W8expentity>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("W8EXPEntity");

            entity.Property(e => e.EntityId)
                .ValueGeneratedNever()
                .HasColumnName("EntityID");
            entity.Property(e => e.EntityName).IsUnicode(false);
        });

        modelBuilder.Entity<W8expfatca>(entity =>
        {
            entity.HasKey(e => e.FatcaId);

            entity.ToTable("W8EXPFATCA");

            entity.Property(e => e.FatcaId).HasColumnName("FATCA_ID");
            entity.Property(e => e.Fatca)
                .IsUnicode(false)
                .HasColumnName("FATCA");
        });

        modelBuilder.Entity<W8imyEntityType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("W8IMY_EntityType");

            entity.Property(e => e.EntityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("EntityID");
            entity.Property(e => e.EntityType).IsUnicode(false);
        });

        modelBuilder.Entity<W8imyFatca>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("W8IMY_FATCA");

            entity.Property(e => e.Fatca)
                .IsUnicode(false)
                .HasColumnName("FATCA");
            entity.Property(e => e.FatcaId)
                .ValueGeneratedOnAdd()
                .HasColumnName("FATCA_ID");
        });

        modelBuilder.Entity<W8imyfatcade>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("W8IMYFATCADE");

            entity.Property(e => e.Fatca)
                .IsUnicode(false)
                .HasColumnName("FATCA");
            entity.Property(e => e.FatcaId)
                .ValueGeneratedOnAdd()
                .HasColumnName("FATCA_ID");
        });

        modelBuilder.Entity<W9>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("'W-9$'");

            entity.Property(e => e.Column1).HasMaxLength(255);
            entity.Property(e => e.F10).HasMaxLength(255);
            entity.Property(e => e.F11).HasMaxLength(255);
            entity.Property(e => e.F2).HasMaxLength(255);
            entity.Property(e => e.F3).HasMaxLength(255);
            entity.Property(e => e.F5).HasMaxLength(255);
            entity.Property(e => e.F6).HasMaxLength(255);
            entity.Property(e => e.F7).HasMaxLength(255);
        });

        modelBuilder.Entity<W91>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("W9-1");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
