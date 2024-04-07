using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Attendees
{
    public class GetAllAttendeesByEventIdUseCase
    {
        private readonly PassInDbContext _dbContext;

        public GetAllAttendeesByEventIdUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseAllAttendeesJson Execute(Guid eventId)
        {
            //var attendees = _dbContext.Attendees
            //    .Where(at => at.Event_Id == eventId)
            //    .ToList();

            var entity = _dbContext.Events
                .Include(e => e.Attendees) // Utilizado devido a FK com Events
                .ThenInclude(at => at.CheckIn)
                .FirstOrDefault(ev => ev.Id == eventId);  // Necessário em JOIN

            if (entity is null)
                throw new NotFoundException("An event with this id does not exist.");

            return new ResponseAllAttendeesJson
            {
                Attendees = entity.Attendees.Select(at => new ResponseAttendeeJson
                {
                    Id = at.Id,
                    Name = at.Name,
                    Email = at.Email,
                    CreatedAt = at.Created_At,
                    CheckedInAt = at.CheckIn?.Created_at
                }).ToList()
            };
        }
    }
}
