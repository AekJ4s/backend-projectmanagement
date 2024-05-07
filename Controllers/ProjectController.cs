using backend_ProjectManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using backend_ProjectManagement.Models;
using Azure;
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


    [HttpPost("CreateProject",Name = "")]

    public ActionResult CreateProject([FromBody] ProjectCreate projectCreate)
    {
       

        Project project = new Project
        {
            Name = projectCreate.Name,
            Detail = projectCreate.Detail,
            StartDate = projectCreate.StartDate,
            EndDate = projectCreate.EndDate,
        };
        project.CreateDate = DateTime.Now;
        project.UpdateDate = DateTime.Now;
        project = Project.Create(_db, project);
        return Ok(project);
    }

   
        
    [HttpPut(Name = "ProjectUpdate")]

    public ActionResult PUT([FromBody] ProjectUpdate projectUpdate)
    {
        Project project = new Project
        {
            Name = projectUpdate.Name,
            Detail = projectUpdate.Detail,
            StartDate = projectUpdate.StartDate,
            EndDate = projectUpdate.EndDate,
        };

        bool employeeExists = _db.Projects.Any(e => e.Id == project.Id && e.IsDeleted != true);
        int IntofData = _db.Projects.Where(e => e.Id == project.Id).AsNoTracking().ToList().Count();

        
        if (IntofData > 0)
        {
            Project DateProject = _db.Projects.Where(e =>e.Id == project.Id).AsNoTracking().ToList().First();
            
             project.Name = (project.Name == null || project.Name == "string") ? DateProject.Name : project.Name;
             
             project.Detail = (project.Detail == null || project.Detail == "string") ? DateProject.Detail : project.Detail;

             project.StartDate = (project.StartDate == null ) ? DateProject.StartDate : project.StartDate;

             project.EndDate = (project.EndDate == null ) ? DateProject.EndDate : project.EndDate;




            project.IsDeleted = DateProject.IsDeleted;
            project.CreateDate = DateProject.CreateDate;
            project.UpdateDate = DateTime.Now;
            project = Project.Update(_db, project);
        }
        else
        {
            return BadRequest();
        }

        try
        {
            project = Project.Update(_db, project);
        }
        catch (Exception e)
        {
            //Return 500
            return StatusCode(500);
        }

        return Ok(StatusCode(200));
    }

    [HttpDelete(Name ="DeleteProject")]

    public ActionResult DeleteProduct(int id)
    {
        Project product = Project.Delete(_db, id);
        return Ok(product);
    }


     [HttpGet("GetBy/{id}", Name = "GetProject")]

    public ActionResult GetProductById(int id)
    {
        Project project = Project.GetById(_db, id);
        return Ok(project);
    }


}
