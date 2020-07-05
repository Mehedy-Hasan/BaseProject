using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserApp.Dtos;
using UserApp.Models;
using UserApp.Persistance;

namespace UserApp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHostingEnvironment _host;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoSettings _photoSettings;

        public AuthController(
            IConfiguration config, 
            IMapper mapper, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IOptionsSnapshot<PhotoSettings> options,
            IHostingEnvironment host,
            IUnitOfWork unitOfWork)
        {
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _host = host;
            _unitOfWork = unitOfWork;
            _photoSettings = options.Value;
        }
        // Registration Code
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]UserForRegistrationDto userForRegisterDto)
        {
            if (userForRegisterDto.Image == null) return BadRequest("Null file");
            if (userForRegisterDto.Image.Length == 0) return BadRequest("Empty file");
            if (userForRegisterDto.Image.Length > _photoSettings.MaxBytes) return BadRequest("Max file size exceeded");
            if (!_photoSettings.IsSupported(userForRegisterDto.Image.FileName)) return BadRequest("Invalid file type.");

            var uploadsFolderPath = Path.Combine(_host.WebRootPath, "uploads");
            var image = StorePhoto(uploadsFolderPath, userForRegisterDto.Image);

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            foreach (var error in result.Errors)
            {
                return Unauthorized(error.Description);
            }

            if (!result.Succeeded) return Unauthorized("Registration Failed");
            var userToReturn = _mapper.Map<UserForDetailDto>(userToCreate);
            await _unitOfWork.CompleteAsync();

            return Ok(userToReturn);

            //return CreatedAtRoute("GetUser", new { Controller = "Users", userToReturn.Id }, userToReturn);
        }

        public async Task<string> StorePhoto(string uploadsFolderPath, IFormFile file)
        {
            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        // Login Code
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDtoForSignIn userForSignInDto)
        {
            var user = await _userManager.FindByNameAsync(userForSignInDto.UserName);

            if (user == null)
                return BadRequest("User does not exist");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForSignInDto.Password, false);

            if (!result.Succeeded) return Unauthorized("username or password does not match");
            var returnUser = await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == userForSignInDto.UserName.ToUpper());
            var appUser = _mapper.Map<UserForDetailDto>(returnUser);
            return Ok(new
            {
                token = GenerateJwtToken(user).Result,
                user = appUser
            });
        }

        // Jwt Token Generator for login
        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // Change Password next implementation
        [Authorize]
        [HttpPost("{userName}")]
        public async Task<IActionResult> ChangePassword(string userName, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var result = await _userManager
                .ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded) return BadRequest("Current Password / New Password Is Wrong");

            return Ok();
        }
    }
}
