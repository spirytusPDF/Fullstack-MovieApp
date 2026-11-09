using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.Movie;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController(MovieService movieService
        , IValidator<AddMovieContract> _addValidator,
        IValidator<UpdateMovieContract> _updateValidator,
        IMapper _mapper) : ControllerBase
    {
        private readonly MovieService _movieService = movieService;
        

        
        [HttpGet("{id:int:min(1)}")]
        public ActionResult<MovieResponseDTO> GetOne(int id)
        {
            var movie = _movieService.GetMovie(id);
            if (movie == null) return NotFound("No movie found");
            else return Ok(movie);
        }

        [HttpGet("view")]
        public ActionResult<List<MovieResponseDTO>> GetMovies([FromQuery] MovieParamsContract parameters)
        {
            var parametersDto = _mapper.Map<MovieParamsDTO>(parameters);

            var (items, totalCount) = movieService.GetMovies(parametersDto);
            var metadata = new
            {
                totalCount,
                parametersDto.Page,
                parametersDto.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            return Ok(items);

        }

        [HttpPost("newMovie")]
        public IActionResult AddMovie([FromBody]AddMovieContract contract)
        {
            var valid = _addValidator.Validate(contract);
            if(!valid.IsValid) return BadRequest(valid.Errors);
            var dto = _mapper.Map<MovieCreateDTO>(contract);
            var res = _movieService.AddMovie(dto);
            if (!res.IsSucces) return StatusCode(500,res.Message);
            else return Ok();
        }
        [HttpPost("update/{id:int:min(1)}")]
        public IActionResult Update([FromBody] UpdateMovieContract contract, [FromRoute] int id)
        {
            var valid = _updateValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);
            var dto = _mapper.Map<UpdateMovieDTO>(contract);
            dto.Id = id;
            var res = _movieService.UpdateMovie(dto);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }
        [HttpPost("delete/{id:int:min(1)}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var res = _movieService.DeleteMovie(id);
            if (!res.IsSucces) return StatusCode(500, res.Message);
            else return Ok();
        }
    }
}
