using System.ComponentModel.DataAnnotations;

namespace SimLoad.Server.Projects.Submissions;

public class CreateProjectSubmission
{
    [MinLength(1)] public string Name { get; set; }
    public string? Description { get; set; }
}