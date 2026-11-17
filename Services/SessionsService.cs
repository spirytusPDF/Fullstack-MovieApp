using AutoMapper;
using Domain;
using Repositories.MovieRepo;
using Repositories.Queries;
using Repositories.SessionsRepo;
using Services.DTOs.Movie;
using Services.DTOs.MovieSession;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class SessionsService(ISessionsRepository _sessionRepository, IMapper _mapper)
    {
        public async Task<Result> AddSessionAsync(SessionCreateDTO sessionCreateDTO)
        {
            var session = _mapper.Map<MovieSession>(sessionCreateDTO);
            session.AvailableSeats = sessionCreateDTO.AvailableSeats; 
            await _sessionRepository.AddSessionAsync(session);
            var res = await _sessionRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't add a session" };
        }

        public async Task<Result> UpdateSessionAsync(SessionUpdateDTO sessionUpdateDTO)
        {
            var session = _mapper.Map<MovieSession>(sessionUpdateDTO);
            await _sessionRepository.UpdateSessionAsync(session);
            var res = await _sessionRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't update a session" };
        }

        public async Task<Result> DeleteSession(int id)
        {
            var session = await _sessionRepository.GetOneAsync(id);
            if (session == null) return new Result() { IsSucces = false, Message = "Couldn't find session" };
            await _sessionRepository.DeleteSession(session);
            var res = await _sessionRepository.SaveChangesAsync();
            if (res) return new Result() { IsSucces = true };
            else return new Result() { IsSucces = false, Message = "Couldn't delete a session" };
        }

        public async Task<SessionResponseDTO?> GetSessionAsync(int id)
        {
            var session = await _sessionRepository.GetOneAsync(id);
            if (session is null) return null;

            return _mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<Tuple<List<SessionResponseDTO>, int>> GetSessionsAsync(SessionParamsDTO parameters)
        {

            var parametersOrg = _mapper.Map<SessionParameters>(parameters);
            if (parameters.Page == null || parameters.Page <= 0) parametersOrg.Page = 1;
            if (parameters.PageSize == null || parameters.PageSize <= 0) parametersOrg.PageSize = 10;
            if (string.IsNullOrEmpty(parametersOrg.Sort)) parametersOrg.Sort = string.Empty;


            var filter = await ExpressionFilterBuilder.BuildFilter<MovieSession, SessionParameters>(parametersOrg);
            var sessions = await _sessionRepository.GetPagedSessionsAsync(filter, parametersOrg.Page, parametersOrg.PageSize, parametersOrg.Sort);
            return new(_mapper.Map<List<SessionResponseDTO>>(sessions.Item1), sessions.Item2);
        }

        
    }
}
