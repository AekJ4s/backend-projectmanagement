using backend_ProjectManagement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace backend_ProjectManagement.Models
{
    public class ProjectMetadata
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Detail { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? IsDeleted { get; set; }

        public int? ActivitiesId { get; set; }
    }

    public class ProjectCreate
    {

        public string? Name { get; set; }

        public string? Detail { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public List<Activity> Activities { get; set; } = new List<Activity>();

    }

    public struct ProjectUpdate

    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string? Detail { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }

    

    [MetadataType(typeof(ProjectMetadata))]

    public partial class Project
    {
        public static Project Create(DatabaseContext db, Project project)
        {
            project.CreateDate = DateTime.Now;
            project.UpdateDate = DateTime.Now;
            project.IsDeleted = false;
            db.Projects.Add(project);
            db.SaveChanges();

            return project;
        }

        public static List<Project> GetAll(DatabaseContext db)
        {
            List<Project> returnThis = db.Projects.Where(q => q.IsDeleted != true).ToList();
            return returnThis;
        }


        public static Project Update(DatabaseContext db, Project project)
        {
            project.UpdateDate = DateTime.Now;
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();
            return project;
        }

        public static Project GetById(DatabaseContext db, int id)
        {
            Project? project = db.Projects
                                    .Include(p => p.Activities) // Eager loading ข้อมูลกิจกรรม
                                    .FirstOrDefault(q => q.Id == id && q.IsDeleted != true);
            return project ?? new Project(); // คืนค่าโปรเจคพร้อมกับข้อมูลกิจกรรมหรือสร้างโปรเจคใหม่หากไม่พบ
        }

        public static Project Delete(DatabaseContext db, int id)
        {
            Project project = GetById(db, id);
            
            project.IsDeleted = true;
            // db.Employees.Remove(employee); เป็นวิธีการลบแบบให้หายไปเลย
            db.Entry(project).State = EntityState.Modified; // Soft Delete
            db.SaveChanges();

            return project;
        }

        

    }






}