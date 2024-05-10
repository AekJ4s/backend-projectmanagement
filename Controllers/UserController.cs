


using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace backend_ProjectManagement.Controllers;

[ApiController]
[Route("users")]

public class UserController : ControllerBase
{

    private DatabaseContext _db = new DatabaseContext();
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }



    [HttpGet("User", Name = "ShowAllUsers")]

    public ActionResult GetAll()
    {
        // .OrderBy(q => q.Salary) เรียงจากน้อยไปมาก
        // .OrderByDescending(q => q.Salary) เรียงจากมากไปน้อย
        List<User> users = backend_ProjectManagement.Models.User.GetAllUser(_db);
        return Ok(users);
    }


    [HttpPost("User", Name = "CreateUser")]

    public ActionResult<Response> CreateUser([FromBody] UserCreate userCreate)
    {
        User user = new User
        {
            Username = userCreate.Username,
            Password = userCreate.Password,
            Pin = userCreate.Pin,
        };
        user.CreateDate = DateTime.Now;
        user.UpdateDate = DateTime.Now;
        string Message = backend_ProjectManagement.Models.User.Create(_db, user);
        return new Response{
            Code = 400,
            Message = Message,
            Data = user,
        };
    }



    [HttpPut("User", Name = "UserUpdate")]

    public ActionResult<Response> PUT([FromBody] User NewData)
    {
        User UserData = _db.Users.Where(e => e.Id == NewData.Id).AsNoTracking().ToList().First();

        if (UserData != null)
        {
            // ตรวจสอบฟีลแต่ละฟีลและอัพเดตข้อมูล
            UserData.Username = (string.IsNullOrEmpty(NewData.Username) || NewData.Username == "string") ? UserData.Username : NewData.Username;
            UserData.Password = (string.IsNullOrEmpty(NewData.Username) || NewData.Username == "string") ? UserData.Password : NewData.Password;
            UserData.Pin = (NewData.Pin == null) ? UserData.Pin : NewData.Pin;
            UserData.UpdateDate = DateTime.Now;

            // บันทึกการอัพเดทลงในฐานข้อมูล
            backend_ProjectManagement.Models.User.UpdateUser(_db, UserData);
            _db.SaveChanges();
            // ส่งค่าสถานะการอัพเดทเป็น 200 OK
            return new Response
            {
                Code = 200,
                Message = "Success",
                Data = UserData
            };
        }
        else
            return new Response
            {
                Code = 500,
                Message = "Not Find Users ",
                Data = null
            };

    }


    [HttpDelete("User", Name = "DeleteUser")]

    public ActionResult DeleteUser(int id)
    {
        User user = backend_ProjectManagement.Models.User.Delete(_db, id);
        return Ok(user);
    }


    [HttpGet("User/{id}", Name = "User")]

    public ActionResult GetUserById(int id)
    {
        User user = backend_ProjectManagement.Models.User.GetById(_db, id);
        return Ok(user);
    }


}
