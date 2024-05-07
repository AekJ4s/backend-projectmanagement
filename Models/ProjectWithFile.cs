using System;
using System.Collections.Generic;

namespace backend_ProjectManagement.Models;

public partial class ProjectWithFile
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? FileId { get; set; }

    public virtual ProjectWithFile? File { get; set; }

    public virtual ICollection<ProjectWithFile> InverseFile { get; set; } = new List<ProjectWithFile>();
}
