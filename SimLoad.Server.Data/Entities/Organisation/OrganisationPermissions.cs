using System.ComponentModel.DataAnnotations.Schema;

namespace SimLoad.Server.Data.Entities.Organisation;

[Table("organisation_permission")]
public class OrganisationPermissions : IPermissions<Organisation>
{
    [Column("id")] public Guid Id { get; set; }

    [Column("organisation_edit")] public bool OrganisationEdit { get; set; }

    [Column("organisation_delete")] public bool OrganisationDelete { get; set; }

    [Column("organisation_addmember")] public bool OrganisationAddMember { get; set; }

    [Column("organisation_removemember")] public bool OrganisationRemoveMember { get; set; }

    [Column("organisation_createproject")] public bool OrganisationCreateProject { get; set; }

    [Column("organisation_changepermission")]
    public bool OrganisationChangePermission { get; set; }

    [Column("loadgeneratorcredential_create")]
    public bool LoadGeneratorCredentialCreate { get; set; }

    [Column("loadgeneratorcredential_delete")]
    public bool LoadGeneratorCredentialDelete { get; set; }

    [Column("test_run")] public bool TestRun { get; set; }

    [Column("organisationmemberid")] public Guid OrganisationMemberId { get; set; }

    public OrganisationMember OrganisationMember { get; set; }
}