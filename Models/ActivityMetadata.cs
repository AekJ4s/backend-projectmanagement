using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

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
        public static Activity GetByActivityId(DatabaseContext db, int id)
        {
            Activity? returnThis = db.Activities.Where(q => q.Id == id && q.IsDeleted != true).FirstOrDefault();
            return returnThis ?? new Activity();
        }
        public static void Create(Activity parent,Project project,DatabaseContext db, Activity activity)

        {
            activity.ActivityHeader = parent;
            activity.Project = project;
            activity.CreateDate = DateTime.Now;
            activity.UpdateDate = DateTime.Now;
            activity.IsDeleted = false;
            // ยังไม่ถูกสร้าง เราจะไปทำ 1.ยัด HeaderID ให้มัน
                foreach(Activity newActivity in activity.InverseActivityHeader){
                Activity.Create(activity,project,db, newActivity);
                }
          
        } 

        public static void SendActivities(Project project, ICollection<Activity> activityOrigin, ICollection<Activity> activityRecive)
        {
            foreach (Activity Data in activityRecive)  // ไปวนหาค่าภายใน activity 
            {
                Activity HeadData = new Activity           // สร้าง activity ใหม่
                {

                    Name = Data.Name,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IsDeleted = false,
                    Project = project
                    // รับค่าชื่อ โดยอิงจาก activity.Name
                };
                //วนสร้างลูกใหม่เรื่อยๆ
                SendActivities(project, HeadData.InverseActivityHeader, Data.InverseActivityHeader);

                //สร้างลูกในแม่
                activityOrigin.Add(HeadData);

            }
            return;
        }

        public static void TakeActivity(Activity? parent,Project project,Activity activity, DatabaseContext _db)
        {
            if (activity.Id == 0)
            { // โดนสร้างยัง ?
                Activity.Create(parent,project,_db, activity);
                _db.Activities.Add(activity);                
            }
            else
            {
                // โยนไปให้หาลูกจนถึงตัวสุดท้าย
                foreach (Activity Data in activity.InverseActivityHeader)
                {
                    TakeActivity(activity,project,Data,_db);
                }
                // อัพเดตทุกอย่่างในจุดนี้
                Activity? dataUpdate = _db.Activities.Find(activity.Id);
                
                if(dataUpdate != null){
                dataUpdate.Name = activity.Name;
                dataUpdate.UpdateDate = DateTime.Now;

                _db.Update(dataUpdate);
                _db.SaveChanges();
                }
            }
        }


        public static void GetALlActivityinside(ICollection<Activity> activity,DatabaseContext _db){
            foreach(Activity Data in activity){
                
                Data.InverseActivityHeader = _db.Activities.Where(i => i.ActivityHeaderId == Data.Id && i.IsDeleted != true).AsNoTracking().ToList();
                if(Data.InverseActivityHeader.Count > 0){
                    GetALlActivityinside(Data.InverseActivityHeader,_db);
                }
            }
            return;
        }


    }


}