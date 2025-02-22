using Tizpusoft.Auth;

namespace Tizpusoft.Reporting;

public static class ApiPermissions
{
    public static readonly string Domain_Reporting = "Reporting";

    public static readonly string Scope_Sample = "sample";

    public static readonly string Permission_View = "view";
    public static readonly string Permission_Add = "add";
    public static readonly string Permission_Edit = "edit";
    public static readonly string Permission_Remove = "remove";

    public static readonly AuthMetadata Public = new(isPublic: true);
    public static readonly AuthMetadata ApiClient = new(apiClientPermitted: true);

    /// <summary>
    /// JWT should be there, nothing else is important
    /// </summary>
    public static readonly AuthMetadata TokenIsEnough = new();

    /// <summary>
    /// Domain should be match to "Reporting"
    /// </summary>
    public static readonly AuthMetadata DomainSpecific = TokenIsEnough.NewWith(domain: Domain_Reporting);

    public static readonly AuthMetadata Test = DomainSpecific.NewWith(scope: Scope_Sample, permission: Permission_View);
    public static readonly AuthMetadata TestAdd = DomainSpecific.NewWith(permission: Permission_Add);
    public static readonly AuthMetadata TestEdit = DomainSpecific.NewWith(permission: Permission_Edit);
    public static readonly AuthMetadata TestRemove = DomainSpecific.NewWith(permission: Permission_Remove);

}
