using Microsoft.AspNetCore.Mvc;
using DapperPrac.Dtos;
using DapperPrac.Models;
using DapperPrac.Repositories;
using AutoMapper;

namespace DapperPrac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null) return NotFound();

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createDto)
        {
            var user = _mapper.Map<User>(createDto);

            await _userRepository.CreateUser(user);

            var userDto = _mapper.Map<UserDto>(user);
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
