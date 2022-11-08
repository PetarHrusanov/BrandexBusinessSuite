namespace BrandexBusinessSuite.Common;

public class Constants
{
    public const string AdministratorRoleName = "Administrator";
    public const string AccountantRoleName = "Accountant";
    public const string MarketingRoleName = "Marketing";
    public const string ViewerExecutive = "Viewer Executive";

    public const string DefaultConnection = "DefaultConnection";

    public const string UploadExcel = "UploadExcel";
    public const string UploadPdf = "UploadPdf";

    public const string CreatedOn = "CreatedOn";
    public const string ModifiedOn = "ModifiedOn";
    public const string IsDeleted = "IsDeleted";

    public class RequestConstants
    {
        public const string ApplicationJson = "application/json";
    }

    public class Errors
    {
        public const string IncorrectFileFormat = "Incorrect file format - supported format is xlsx.";
    }

}