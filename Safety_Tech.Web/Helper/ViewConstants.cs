namespace Safety_Tech.Web.Helper
{

    public class ViewConstants
    {
        public const string GETALLUSER = "GetAllUser";
        public const string ADDUSER = "AddUser";
        public const string LOGINVIEW = "loginView";
        public const string INDEX = "Index";
        public const string HOME = "Home";
        public const string AUTH = "Auth";
        public const string USER = "User";

    }

    public enum UserRoles
        {
        SuperAdmin = 1,
        Admin=2,
        User= 3,
        Manager,

        }
    public class UserSystemRoles
        {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Manager = "Manager";
        public const string AdminOrUserOrManager = Admin + "," + User + "," + Manager;
        public const string AdminOrUserOrSuperAdmin = Admin + "," + User + "," + SuperAdmin;
        public const string UserOrManager = User + "," + Manager;
        public const string UserOrSuperAdmin = User + "," + SuperAdmin;
        public const string AdminOrManager = Admin + "," + Manager;
        public const string AdminOrSuperAdmin = Admin + "," + SuperAdmin;
        public const string AdminOrUser = Admin + "," + User;
        public const string SuperAdminOrUser = SuperAdmin + "," + User;

        }

    }

