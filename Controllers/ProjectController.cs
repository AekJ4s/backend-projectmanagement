using backend_ProjectManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Identity;
[ApiController]

[Route("projects")]
public class ProjectController : ControllerBase
{

    private DatabaseContext _db = new DatabaseContext();
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(ILogger<ProjectController> logger)
    {
        _logger = logger;
    }



    [HttpGet(Name = "ShowAllProjects")]

    public ActionResult GetAll()
    {
        // .OrderBy(q => q.Salary) เรียงจากน้อยไปมาก
        // .OrderByDescending(q => q.Salary) เรียงจากมากไปน้อย
        List<Project> projects = Project.GetAll(_db).OrderByDescending(q => q.Id).ToList();
        return Ok(projects);
    }

    [HttpGet("GetBy/{id}", Name = "GetProjectID")]
    public ActionResult GetProjectID(int id)
    {
        // ค้นหาโปรเจคจากฐานข้อมูลโดยใช้ Id
        Project? DataOfProject = _db.Projects.Find(id);

        // ตรวจสอบว่าโปรเจคที่ค้นหาพบหรือไม่
        if (DataOfProject != null)
        {
            // ค้นหากิจกรรมที่เกี่ยวข้องกับโปรเจค
            DataOfProject.Activities = _db.Activities
                .Where(useinit => useinit.ProjectId == id && useinit.ActivityHeaderId == null && useinit.IsDeleted != true)
                .ToList();

            // วนลูปผ่านกิจกรรมในโปรเจคและเรียกใช้เมธอด TakeActivity
            foreach (Activity activity in DataOfProject.Activities)
            {
                Activity.GetALlActivityinside(DataOfProject.Activities, _db); //ไปวนหาว่ามีลูกอีกไหม
            }
        }
        else
        {
            // หากไม่พบโปรเจคในฐานข้อมูล คืนค่า BadRequest
            return BadRequest();
        }

        try
        {
            // ส่งข้อมูลโปรเจคกลับไปยังไคลเอนต์
            return Ok(new Response
            {
                Code = 200,
                Message = "Success",
                Data = DataOfProject
            }
            );
        }
        catch (Exception e)
        {
            // หากเกิดข้อผิดพลาดในการส่งข้อมูล คืนค่า StatusCode 500 (Internal Server Error)
            return (StatusCode(500));
        }
    }



    [HttpPost("CreateProject", Name = "")]
    public ActionResult<Response> Create(ProjectCreate projectCreate)
    {
        // สร้างโปรเจ็คใหม่
        Project project = new Project
        {
            Name = projectCreate.Name,        // รับค่าชื่อจาก projectCreate.Name
            Detail = projectCreate.Detail,    // รับค่ารายละเอียดจาก projectCreate.Detail
            StartDate = projectCreate.StartDate,  // รับค่าวันที่เริ่มต้นจาก projectCreate.StartDate
            EndDate = projectCreate.EndDate       // รับค่าวันที่สิ้นสุดจาก projectCreate.EndDate
        };

        try
        {
            // สร้างกิจกรรมสำหรับโปรเจ็คใหม่

                Activity.SendActivities(project, project.Activities, projectCreate.Activities);
            

            // บันทึกโปรเจ็คลงในฐานข้อมูล
            Project.Create(_db, project);
            _db.SaveChanges();

            // สร้างข้อมูลการตอบกลับสำหรับการสร้างโปรเจ็คสำเร็จ
            return Ok(new Response
            {
                Code = 200,
                Message = "Success",
                Data = project
            });
        }
        catch
        {
            // หากเกิดข้อผิดพลาดในการสร้างโปรเจ็ค คืนค่าข้อมูลการตอบกลับสำหรับข้อผิดพลาดภายในเซิร์ฟเวอร์
            return new Response
            {
                Code = 500,
                Message = "Internal Server Error",
                Data = null
            };
        }
    }
    [HttpPut(Name = "ProjectUpdate")]

    public ActionResult<Response> UpdateProjectRequest([FromBody] Project NewData)
    {
        // ค้นหาโปรเจคที่ต้องการอัพเดทโดยใช้ ID เพื่อนำไปแก้ไข

        Project? DataOfProject = _db.Projects.Find(NewData.Id);

        if (NewData != null)
        {
            // ให้กิจกรรมที่ดึงมามีโครงสร้างลูกซ้อนถูกอัพเดท
            foreach (Activity activity in NewData.Activities)
            {
                Activity.TakeActivity(null,DataOfProject,activity, _db);
            }

            if (DataOfProject != null)
            {
                // ตรวจสอบและอัพเดทข้อมูลโปรเจค
                DataOfProject.Name = (string.IsNullOrEmpty(NewData.Name) || NewData.Name == "string") ? DataOfProject.Name : NewData.Name;
                DataOfProject.Detail = (string.IsNullOrEmpty(NewData.Detail) || NewData.Detail == "string") ? DataOfProject.Detail : NewData.Detail;
                DataOfProject.StartDate = (NewData.StartDate == null) ? DataOfProject.StartDate : NewData.StartDate;
                DataOfProject.EndDate = (NewData.EndDate == null) ? DataOfProject.EndDate : NewData.EndDate;
                DataOfProject.UpdateDate = DateTime.Now;

                // บันทึกการอัพเดทลงในฐานข้อมูล
                Project.Update(_db, DataOfProject);
                _db.SaveChanges();
                // ส่งค่าสถานะการอัพเดทเป็น 200 OK
                return new Response
                {
                    Code = 200,
                    Message = "Success",
                    Data = DataOfProject
                };
           
            }
            else
            return new Response
                {
                    Code = 400,
                    Message = "Bad Request",
                    Data = null
                };
            
           
        }
        else
        {
            {
                // หากไม่พบโปรเจคที่ต้องการอัพเดท ส่งค่า BadRequest
                return new Response
                {
                    Code = 500,
                    Message = "Not Find Project",
                    Data = null
                };
            }
        }
    }


    [HttpDelete(Name = "DeleteProject")]
    public ActionResult DeleteProduct(int id)
    {
        Project product = Project.Delete(_db, id);
        return Ok(product);
    }

}
