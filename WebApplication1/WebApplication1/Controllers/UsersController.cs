using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserApp.Dtos;
using UserApp.Models;
using UserApp.Persistance;

namespace UserApp.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(UserManager<User> userManager, IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);

            var userForReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userForReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {

            var users = await _userRepository.GetUsers();

            var mapUser = _mapper.Map<IEnumerable<UserForReturnDto>>(users);
            return Ok(mapUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user =await _userRepository.GetUser(id);
            _mapper.Map(userForUpdateDto, user);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

    }
}