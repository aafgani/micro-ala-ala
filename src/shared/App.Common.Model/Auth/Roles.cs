using System;

namespace App.Common.Domain.Auth;

public static class Roles
{
    public const string Admin = "Admin";
    public const string TodosAdmin = "Todos:Admin";
    public const string TodosContributor = "Todos:Contributor";
    public const string TodosView = "Todos:View";
    public const string FinanceAdmin = "Finance:Admin";
    public const string FinanceContributor = "Finance:Contributor";
    public const string FinanceView = "Finance:View";
}
