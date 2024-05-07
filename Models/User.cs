using System;
using System.Collections.Generic;

namespace backend_ProjectManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Pin { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectOwener { get; set; }

    public virtual Project? ProjectOwenerNavigation { get; set; }
}
