using System.ComponentModel.DataAnnotations;
using backend_ProjectManagement.Data;

namespace backend_ProjectManagement.Models
{
    public class ProjectWithFileMetadata
    {
    public int Id { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }
    }

    public class ProjectWithFileCreate
    {
    public int Id { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }
    }

     public class ProjectWithFileRequest
    {

    public int? ProjectId { get; set; }

    public int? FileId { get; set; }

    }
    [MetadataType(typeof(ProjectWithFileMetadata))]

    public partial class ProjectWithFile 
    {

        public static ProjectWithFile Create(DatabaseContext db , ProjectWithFile file)
        {
            file.IsDeleted = false;
            db.ProjectWithFiles.Add(file);
            return file;
        }
    }
}