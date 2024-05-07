namespace backend_ProjectManagement.Models;

public partial class Response
{
    public int Code{get; set; }

    public string? Message{get; set; }

    public object? Data{get; set; }
}