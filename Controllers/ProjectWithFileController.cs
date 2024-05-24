using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("ProjectWithFile")]
[Authorize]

public class ProjectWithFileController : ControllerBase
{

    private DatabaseContext _db = new DatabaseContext();

    private IHostEnvironment _hostingEnvironment;

    private readonly ILogger<FileUploadController> _logger;

    public ProjectWithFileController(ILogger<FileUploadController> logger, IHostEnvironment? environment)
    {
        _logger = logger;
        _hostingEnvironment = environment;
    }

    [HttpPost(Name = "ProjectxFile")]
    public ActionResult<Response> Create(ProjectWithFileRequest data)
    {
        // สร้างโปรเจ็คใหม่
        ProjectWithFile projectxfile = new ProjectWithFile
        {
            ProjectId = data.ProjectId,
            FileId = data.FileId,
        };
        try
        {
            // บันทึกโปรเจ็คลงในฐานข้อมูล
            ProjectWithFile.Create(_db, projectxfile);
            _db.SaveChanges();

            // สร้างข้อมูลการตอบกลับสำหรับการสร้างโปรเจ็คสำเร็จ
            return Ok(new Response
            {
                Code = 200,
                Message = "Success",
                Data = projectxfile
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
    
    [HttpGet(Name = "ShowAllProjectFiles")]

    public ActionResult GetAllProjectFiles()
    {
        // .OrderBy(q => q.Salary) เรียงจากน้อยไปมาก
        // .OrderByDescending(q => q.Salary) เรียงจากมากไปน้อย
        List<ProjectWithFile> projectFiles = ProjectWithFile.GetAll(_db);
        return Ok(projectFiles);
    }

     [HttpGet("GetBy/{id}", Name = "ShowOnlyThisProjectFiles")]
    public ActionResult<Response> GetProjectFiles(int id)
    {
        try
        {
            // 1. ค้นหาโปรเจค
            // 2. ค้นหาไฟล์ที่เกี่ยวข้อง

            List<ProjectWithFile> fileOfProject = ProjectWithFile.GetById(_db, id); // ได้ไฟล์ที่เกี่ยวข้องกับโปรเจคทุกอย่างแล้ว

            // ตรวจสอบว่าโปรเจคที่ค้นหาพบหรือไม่
            if (fileOfProject == null)
            {
                return NotFound(new Response
                {
                    Code = 404,
                    Message = "Project not found or has been deleted"
                });
            }

            // ส่งข้อมูลโปรเจคกลับไปยังไคลเอนต์
            return Ok(new Response
            {
                Code = 200,
                Message = "Success",
                Data = fileOfProject
            });
        }
        catch (Exception e)
        {
            // หากเกิดข้อผิดพลาดในการส่งข้อมูล คืนค่า StatusCode 500 (Internal Server Error)
            return StatusCode(500, new Response
            {
                Code = 500,
                Message = "Internal server error: " + e.Message
            });
        }
    }

}
      
