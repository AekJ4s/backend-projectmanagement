using System;
using System.Collections.Generic;

namespace backend_ProjectManagement.Models;

public partial class ProjectWithFile
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? FileId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual FileUpload? File { get; set; }

    public virtual Project? Project { get; set; }
}
