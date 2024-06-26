


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_ProjectManagement.Data;
using backend_ProjectManagement.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace backend_ProjectManagement.Controllers;

[ApiController]
[Route("users")]

public class UserController : ControllerBase
{

    private const string TokenSecret = "welcometojwtsigninthiskeycanchanginappdotjson";

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);
    private DatabaseContext _db = new DatabaseContext();
    private readonly ILogger<UserController> _logger;
    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }


    // GEN TOKEN
    [HttpPost("token")]
    public string GenerateToken([FromBody] User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TokenSecret);
        if (user.Username != null)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name , user.Username),

        };


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = "http://localhost:5157",
                Audience = "http://localhost:5157",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            if (token != null)
            {
                var jwt = tokenHandler.WriteToken(token);
                return jwt;
            }
            else
            {
                return "Failed to create token.";
            }
        }
        return "Failed to create token.";
    }

    [HttpPost("login")]

    public ActionResult<Response> Login([FromBody] UserLogin requestUser)
    {
        User? user = _db.Users.FirstOrDefault(x => x.Username == requestUser.Username && x.IsDeleted == false);
        string bearerToken = "";
        if (user == null)
        {
            return NotFound(new Response
            {
                Code = 404,
                Message = "NOT FOUND USER",
                Data = null,
            }
                );
        };
        try
        {
            if (user.Pin == requestUser.Pin)
            {
                bearerToken = GenerateToken(user);
            }
            else
            {
                return BadRequest(new Response
                {
                    Code = 400,
                    Message = "user password or pin is wrong or all are wrong",
                    Data = user
                });
            }
        }
        catch
        {
            return StatusCode(500);
        }
        return Ok(new Response
        {
            Code = 200,
            Message = "Success",
            Data = new Dictionary<string, object>
    {
        { "BearerToken", bearerToken },
        { "UserId", user.Id },
        { "UserName", user.Username }
    }
        });

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
        if (userCreate.Username != null && userCreate.Password != null && userCreate.Pin != null)
        {
            if (userCreate.Pin.Length == 4)
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
                return new Response
                {
                    Code = 200,
                    Message = Message,
                    Data = user,
                };
            }
            else
                return BadRequest(new Response
                {
                    Code = 404,
                    Message = "Key length is four characters",
                });
        }
        return BadRequest(new Response
        {
            Code = 404,
            Message = "กรอกข้อมูลให้ครบก่อน ****",
        });


    }



    [HttpPut("User", Name = "UserUpdate")]

    public ActionResult<Response> EditUserPassword([FromBody] UserEditer NewData)
    {
        User UserData = _db.Users.Where(e => e.Username == NewData.Username).AsNoTracking().ToList().First();

        if (NewData.NewPassword != null && NewData.NewPin != null)
        {
            if (NewData.Username == UserData.Username && NewData.Password == UserData.Password && UserData.Pin == UserData.Pin && UserData.IsDeleted != true)
            {

                UserData.Password = (NewData.NewPassword == null || NewData.NewPassword == "string") ? UserData.Password : NewData.NewPassword;
                if (NewData.NewPin.Length == 4)
                {
                    UserData.Pin = NewData.NewPin;
                }
                else
                {
                    UserData.Pin = UserData.Pin;
                    return BadRequest(new Response
                    {
                        Code = 404,
                        Message = "Pin is not correct length"
                    });
                }


                backend_ProjectManagement.Models.User.EditPassword(_db, UserData);
                _db.SaveChanges();
                return new Response
                {
                    Code = 200,
                    Message = "Password changed successfully",
                };
            }
            return BadRequest(new Response
            {
                Code = 404,
                Message = "UserName password or pin incorrect",
            });
        }
        else
            return new Response
            {
                Code = 400,
                Message = "Bad Request",
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
