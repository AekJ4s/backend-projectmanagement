﻿using System;
using System.Collections.Generic;

namespace backend_ProjectManagement.Models;

public partial class FileUpload
{
    public int Id { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<ProjectWithFile> ProjectWithFiles { get; set; } = new List<ProjectWithFile>();
}
