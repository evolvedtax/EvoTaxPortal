
namespace EvolvedTax.Common.Constants
{
    public static class ResponseMessageConstants
    {
        public const string SuccessSaved = @"Record Saved Successfully";
        public const string SuccessUpdate = @"Record Updated Successfully";
        public const string SuccessDelete = @"Record Deleted Successfully";
        public const string RequestCancelled = @"Request Cancelled";
        public const string RecordNotFound = @"Record not found";
        public const string ErrorSaved = @"Error while saving record";
        public const string RecordBeingUsed = @"This record is being used by other form, please inactive or delete that first.";
        public const string RecordBeingUsedByUser = @"This record is being used in other forms, you cannot delete";
        public const string RecordLock = @"This record is locked. You're not able to update.";
        public const string FormCompleted = @"This Form is completed. You're not able to update.";
        public const string SuccessEmailSend = @"Emails Sent Successfully.";
        public const string SuccessRoleChange = @"Role has been successfully changed.";

        public const int SuccessStatus = 1;
        public const int ErrorStatus = 2;
        public const int WarningStatus = 3;
    }
}