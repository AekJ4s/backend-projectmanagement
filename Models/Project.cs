﻿using System;
using System.Collections.Generic;

namespace backend_ProjectManagement.Models;

public partial class Project
{
    public int Id { get; set; }

    public int? OwnerId { get; set; }

    public string? Name { get; set; }

    public string? Detail { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<ProjectWithFile> ProjectWithFiles { get; set; } = new List<ProjectWithFile>();
}
