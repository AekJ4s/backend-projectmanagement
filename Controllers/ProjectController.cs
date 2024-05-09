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
        Project? DataOfProject = _db.Projects.Find(id);

        if (DataOfProject != null)
        {

            DataOfProject.Activities = _db.Activities.Where(useinit => useinit.ProjectId == id && useinit.ActivityHeaderId == null && useinit.IsDeleted != true).ToList();
            foreach (Activity activity in DataOfProject.Activities)
            {
                Activity.TakeActivity (DataOfProject.Activities, _db);
                
            }
        }
        else
        {
            return BadRequest();
        }

        try
        {
            return StatusCode(200);
        }
        catch (Exception e)
        {
            //Return 500
            return StatusCode(500);
        }

    }


    [HttpPost("CreateProject", Name = "")]

    public ActionResult<Response> Create(ProjectCreate projectCreate)
    {
        Project project = new Project          // สร้างโปรเจ็คใหม่
        {
            Name = projectCreate.Name,     // รับค่าชื่อจาก projectCreate.Name
            Detail = projectCreate.Detail, // รับค่าชื่อจาก projectCreate.Detail
            StartDate = projectCreate.StartDate,  // รับค่าชื่อจาก projectCreate.StartDate
            EndDate = projectCreate.EndDate,     // รับค่าชื่อจาก projectCreate.EndDate
        };
        // SaveDATA
        try
        {
            foreach (Activity activity in projectCreate.Activities)
            {
                Activity.SendActivities(project, project.Activities, projectCreate.Activities);
                Project.Create(_db, project);
            }

            _db.SaveChanges();

            return new Response
            {
                Code = 200,
                Message = "Success",
                Data = project
            };
        }
        catch
        {
            return new Response
            {
                Code = 500,
                Message = "Internal Server Error",
                Data = null
            };
        }

    }



    [HttpPut(Name = "ProjectUpdate")]

    public ActionResult UpdateProject([FromBody] ProjectUpdate projectUpdate)
    {


        Project? DataOfProject = _db.Projects.Find(projectUpdate.Id);

        if (DataOfProject != null)
        {

            DataOfProject.Activities = _db.Activities.Where(useinit => useinit.ProjectId == projectUpdate.Id && useinit.ActivityHeaderId == null && useinit.IsDeleted != true).ToList();
            foreach (Activity activity in DataOfProject.Activities)
            {
                Activity.TakeActivity (DataOfProject.Activities, _db);
                
            }
            
            DataOfProject.Name = (projectUpdate.Name == null || projectUpdate.Name == "string") ? DataOfProject.Name : projectUpdate.Name;
            DataOfProject.Detail = (projectUpdate.Detail == null || projectUpdate.Detail == "string") ? DataOfProject.Name : projectUpdate.Name;
            DataOfProject.StartDate = (projectUpdate.StartDate == null) ? DataOfProject.StartDate : projectUpdate.StartDate;
            DataOfProject.EndDate = (projectUpdate.EndDate == null) ? DataOfProject.EndDate : projectUpdate.EndDate;
            DataOfProject.UpdateDate = DateTime.Now;
            DataOfProject = Project.Update(_db, DataOfProject);
        }
        else
        {
            return BadRequest();
        }

        try
        {
            return StatusCode(200);
        }
        catch (Exception e)
        {
            //Return 500
            return StatusCode(500);
        }

    }

    [HttpDelete(Name = "DeleteProject")]

    public ActionResult DeleteProduct(int id)
    {
        Project product = Project.Delete(_db, id);
        return Ok(product);
    }

}
