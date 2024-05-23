using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
      
