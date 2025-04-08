using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Domain.Common.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ContactNotes.Queries.GetContactNotes
{
    [Authorize(Features.Contact_Notes, Privileges.Read)]
    public class GetOffenderContactNoteById : IRequest<OffenderContactNoteVM>
    {
        public long ContactNumber { get; set; }
        public long Sin { get; set; }
        public int AgencyId { get; set; }
    }

    public class GetOffenderContactNoteByIdHandler : IRequestHandler<GetOffenderContactNoteById, OffenderContactNoteVM>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationDbContext dbContext;

        public GetOffenderContactNoteByIdHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            this.currentUserService = currentUserService;
            this.dbContext = dbContext;
        }

        public async Task<OffenderContactNoteVM> Handle(GetOffenderContactNoteById request, CancellationToken cancellationToken)
        {
            var offender = await dbContext.Offenders.SingleAsync(x => x.Sin == request.Sin, cancellationToken);
            var agencyId = await dbContext.Staff.Where(x => x.UserId == currentUserService.UserId).Select(x => x.AgencyId).SingleAsync(cancellationToken);
            var staffContactedByList = await dbContext.Staff
                  .Where(x => x.StaffPrivs.Any(staffPriv => staffPriv.Privilege.FeatureId == (int)Features.Contact_Notes) && x.IsActive && x.AgencyId == agencyId)
                  .Select(x => new StaffDto
                  {
                      Staff_key = x.StaffKey,
                      Agency_id = agencyId,
                      First_name = x.FirstName,
                      Last_name = x.LastName
                  }).OrderBy(x => x.Last_name).ThenBy(x => x.First_name)
                  .ToListAsync(cancellationToken);
            JuvenileContactNoteDto contactNoteDto;
            if (request.Contact_number == 0)
            {
                contactNoteDto = new JuvenileContactNoteDto
                {
                    OffenderFullName = offender.FirstName + " " + " " + offender.LastName
                };
            }
            else
            {
                contactNoteDto = await dbContext.ContactNotes.Where(x => x.ContactNumber == request.Contact_number)
                    .Select(x => new JuvenileContactNoteDto
                    {
                        OffenderFullName = offender.FirstName + " " + " " + offender.LastName,
                        Comment = x.Comment,
                        ContactDate = x.ContactDate,
                        ContactedBy = x.ContactedById,
                        ContactNumber = x.ContactNumber,
                        ContactTypeId = x.ContactTypeId,
                        AgencyId = x.AgencyId,
                        IsFamilyOrGuardianInvolved = x.IsFamilyOrGuardianInvolved,
                        IsJuvenileInvolved = x.IsJuvenileInvolved
                    }).SingleAsync(cancellationToken);
            }
            return new OffenderContactNoteVM
            {
                JuvenileContactNoteDto = contactNoteDto,
                Staff = staffContactedByList
            };
        }
    }
}
