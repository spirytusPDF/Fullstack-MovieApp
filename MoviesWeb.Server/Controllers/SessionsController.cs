using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.Movie;
using Services.DTOs.MovieSession;
using System.Text.Json;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController(SessionsService _sessionService
        , IValidator<SessionCreateContract> _addValidator,
        IValidator<SessionUpdateContract> _updateValidator,
        IMapper _mapper) : ControllerBase
    {
        [Authorize]
        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<SessionResponseDTO>> GetOneAsync(int id)
        {
            var session = await _sessionService.GetSessionAsync(id);
            if (session == null) return NotFound("No session found");
            else return Ok(session);
        }
        [Authorize]
        [HttpGet("view")]
        public async Task<ActionResult<List<SessionResponseDTO>>> GetSessionsAsync([FromQuery] SessionParamsContract parameters)
        {
            var parametersDto = _mapper.Map<SessionParamsDTO>(parameters);

            var (items, totalCount) = await _sessionService.GetSessionsAsync(parametersDto);
            var metadata = new
            {
                totalCount,
                parametersDto.Page,
                parametersDto.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            return Ok(items);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("newSession")]
        public async Task<IActionResult> AddSession([FromBody] SessionCreateContract contract, int movieId)
        {
            var valid = _addValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);
            var dto = _mapper.Map<SessionCreateDTO>(contract);
            dto.MovieId = movieId;
            var res = await _sessionService.AddSessionAsync(dto);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update/{id:int:min(1)}")]
        public async Task<IActionResult> Update([FromBody] SessionUpdateContract contract, [FromRoute] int id)
        {
            var valid = _updateValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);
            var dto = _mapper.Map<SessionUpdateDTO>(contract);
            dto.Id = id;
            var res = await _sessionService.UpdateSessionAsync(dto);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int:min(1)}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var res = await _sessionService.DeleteSession(id);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }

    }
}
