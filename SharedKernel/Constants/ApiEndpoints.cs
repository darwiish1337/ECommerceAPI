namespace SharedKernel.Constants;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Accounts
    {
        public static class Auth
        {
            private const string Base = $"{ApiBase}/auth";

            public const string Register = $"{Base}/register";
            public const string Login = $"{Base}/login";
            public const string RefreshToken = $"{Base}/refresh-token";
            public const string Logout = $"{Base}/logout";
            public const string ChangePassword = $"{Base}/change-password";
            public const string ForgotPassword = $"{Base}/forgot-password";
            public const string VerifyEmail = $"{Base}/verify-email";
            public const string ResendVerificationEmail = $"{Base}/resend-verification-email";
            public const string ChangeEmail = $"{Base}/change-email";
        }

        public static class Users
        {
            private const string Base = $"{ApiBase}/users";

            public const string GetAll = $"{Base}";
            public const string GetById = $"{Base}/{{id:guid}}";
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";
            public const string AssignRoles = $"{Base}/{{id:guid}}/roles";
            public const string GetUserRoles = $"{Base}/{{id:guid}}/roles";
            public const string GetUserInfo = $"{Base}/me";
            public const string DeleteAccount = $"{Base}/me";
        }

        public static class Roles
        {
            private const string Base = $"{ApiBase}/roles";

            public const string GetAll = $"{Base}";
            public const string GetById = $"{Base}/{{id:guid}}";
            public const string Create = $"{Base}";
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";
            public const string AssignPermissions = $"{Base}/{{id:guid}}/permissions";
            public const string GetRolePermissions = $"{Base}/{{id:guid}}/permissions";
        }

        public static class Permissions
        {
            private const string Base = $"{ApiBase}/permissions";

            public const string GetAll = $"{Base}";
            public const string GetById = $"{Base}/{{id:guid}}";
            public const string Create = $"{Base}";
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";
        }
        
    }
}