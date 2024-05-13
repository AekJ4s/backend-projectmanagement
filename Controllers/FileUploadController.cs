using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("FileUpload")]
[Authorize]

public class FileUploadController : ControllerBase
{

    private DatabaseContext _db = new DatabaseContext();

    private IHostEnvironment _hostingEnvironment;

    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(ILogger<FileUploadController> logger, IHostEnvironment? environment)
    {
        _logger = logger;
        _hostingEnvironment = environment;
    }

    [HttpPost(Name = "UploadFile")]
    public ActionResult UploadFile(IFormFile formFile)
    {
        if(formFile == null)
        {
            return BadRequest(new Response
            {
                Code = 400,
                Message = "File is Required"
            });
        }

        FileUpload file = new FileUpload
        {
            FileName = formFile.FileName,
            FilePath = "UploadedFile/ProfileImg/"
        };

        file = FileUpload.Create(_db,file);

        if(formFile != null && formFile.Length >0){
            string upload = Path.Combine(_hostingEnvironment.ContentRootPath, "UploadedFile/ProfileImg/" + file.Id);

            Directory.CreateDirectory(upload);
            string filePath = Path.Combine(upload,formFile.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

           
        }
         return Ok(new Response
            {
                Code = 200,
                Message = "Success",
                Data = file
            });
    }
}