using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Server.Contracts;
using Services;
using Services.DTOs.Movie;

namespace MoviesWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController(MovieService movieService
        , IValidator<AddMovieContract> _addValidator,
        IValidator<UpdateMovieContract> _updateValidator) : ControllerBase
    {
        private readonly MovieService _movieService = movieService;
        

        [HttpGet("all")]
        public ActionResult<List<MovieResponseDTO>> GetAll()
        {
            return Ok(_movieService.GetMovies());
        }
        [HttpGet("{id:int:min(1)}")]
        public ActionResult<MovieResponseDTO> GetOne(int id)
        {
            var movie = _movieService.GetMovie(id);
            if (movie == null) return NotFound("No movie found");
            else return Ok(movie);
        }

        [HttpGet("search")]
        public ActionResult<List<MovieResponseDTO>> Search([FromQuery] string keyword)
        {
            return Ok(_movieService.SearchMovies(keyword));
        }

        [HttpPost("newMovie")]
        public IActionResult AddMovie([FromBody]AddMovieContract contract)
        {
            var valid = _addValidator.Validate(contract);
            if(!valid.IsValid) return BadRequest(valid.Errors);
            var dto = new MovieCreateDTO() 
            {
                Title = contract.Title,
                Description = contract.Description,
                Genre = contract.Genre,
                Rating = contract.Rating,
                ReleaseYear = contract.ReleaseYear,
                PosterUrl = contract.PosterUrl,
                MovieUrl = contract.MovieUrl,
            };
            var res = _movieService.AddMovie(dto);
            if (!res.IsSucces) return StatusCode(500,res.Message);
            else return Ok();
        }
        [HttpPost("update/{id:int:min(1)}")]
        public IActionResult Update([FromBody] UpdateMovieContract contract, [FromRoute] int id)
        {
            var valid = _updateValidator.Validate(contract);
            if (!valid.IsValid) return BadRequest(valid.Errors);
            var dto = new UpdateMovieDTO()
            {
                Id = id,
                Description = contract.Description,
                Genre = contract.Genre,
                Rating = contract.Rating,
                ReleaseYear = contract.ReleaseYear,
                PosterUrl = contract.PosterUrl,
                MovieUrl = contract.MovieUrl,
            };
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
