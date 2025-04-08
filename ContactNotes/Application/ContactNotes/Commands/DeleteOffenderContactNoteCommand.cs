using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Domain.Common.Constants;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ContactNotes.Commands
{
    [Authorize(Features.Contact_Notes, Privileges.Delete)]
    public class DeleteOffenderContactNoteCommand : IRequest
    {
        public long Offender_ContactNote_number { get; set; }
    }

    public class DeleteOffender_ContactNoteCommandHandler : IRequestHandler<DeleteOffenderContactNoteCommand>
    {
        private readonly IApplicationDbContext dbContext;

        public DeleteOffender_ContactNoteCommandHandler(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Unit> Handle(DeleteOffenderContactNoteCommand request, CancellationToken cancellationToken)
        {
            dbContext.ContactNotes.Remove(new Domain.Entities.ContactNote { ContactNumber = request.Offender_ContactNote_number });
            await dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}