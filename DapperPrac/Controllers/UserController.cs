using Microsoft.AspNetCore.Mvc;
using DapperPrac.Dtos;
using DapperPrac.Models;
using DapperPrac.Repositories;

namespace DapperPrac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            });
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null) return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createDto)
        {
            var user = new User
            {
                Username = createDto.Username,
                Email = createDto.Email
            };

            await _userRepository.CreateUser(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateDto)
        {
            var existing = await _userRepository.GetUser(id);
            if (existing == null) return NotFound();

            existing.Username = updateDto.Username;
            existing.Email = updateDto.Email;

            await _userRepository.UpdateUser(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var existing = await _userRepository.GetUser(id);
            if (existing == null) return NotFound();

            await _userRepository.DeleteUser(id);
            return NoContent();
        }
    }
}
