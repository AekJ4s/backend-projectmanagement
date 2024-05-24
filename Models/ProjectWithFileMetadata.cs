using System.ComponentModel.DataAnnotations;
using backend_ProjectManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;

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

        public static ProjectWithFile Create(DatabaseContext db, ProjectWithFile file)
        {
            file.IsDeleted = false;
            db.ProjectWithFiles.Add(file);
            return file;
        }

        public static List<ProjectWithFile> GetAll(DatabaseContext db)
        {
            List<ProjectWithFile> returnThis = db.ProjectWithFiles.Where(q => q.IsDeleted != true).ToList();
            return returnThis;
        }

          public static List<ProjectWithFile> GetById(DatabaseContext db, int id)
        {   
            List<ProjectWithFile>? file = db.ProjectWithFiles
                                    .Where(q => q.ProjectId == id && q.IsDeleted != true).ToList();
            return file; // คืนค่าโปรเจคพร้อมกับข้อมูลกิจกรรมหรือสร้างโปรเจคใหม่หากไม่พบ
        }

         public static ProjectWithFile Delete(DatabaseContext db, ProjectWithFile file)
        {   
            file.IsDeleted = true ;
            return file; // คืนค่าโปรเจคพร้อมกับข้อมูลกิจกรรมหรือสร้างโปรเจคใหม่หากไม่พบ
        }
    }
}