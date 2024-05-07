using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace backend_ProjectManagement.Models
{
    public class ActivityMetadata
    {
    public int Id { get; set; }

    public int? ActivityHeaderId { get; set; }

    public string? Name { get; set; }

    public string? Detail { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectId { get; set; }

    public virtual Activity? ActivityHeader { get; set; }

    public virtual ICollection<Activity> InverseActivityHeader { get; set; } = new List<Activity>();

    public virtual Project? Project { get; set; }

    
    }

    [MetadataType(typeof(ActivityMetadata))]
    
    public partial class Activity
    {
        public static Activity Create(DatabaseContext db, Activity activity)
        {
            activity.CreateDate = DateTime.Now;
            activity.UpdateDate = DateTime.Now;
            activity.IsDeleted = false;
            db.Activities.Add(activity);
            db.SaveChanges();

            return activity;
        }

       public static void SetActivitiesCreate(Activity activity)
       {
        activity.CreateDate = DateTime.Now;
        activity.UpdateDate = DateTime.Now;
        activity.IsDeleted = false;
        foreach (Activity subactivity in activity.InverseActivityHeader)
        {
            SetActivitiesCreate(subactivity);
        }
       }

    }
        
    
}