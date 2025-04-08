using FluentValidation;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Domain.Common.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ContactNotes.Commands
{
    [Authorize(Features.Contact_Notes, Privileges.Modify)]
    public class UpdateOffenderContactNoteCommand : IRequest
    {
        public long ContactedById { get; set; }
        public long ContactTypeId { get; set; }
        public string Comment { get; set; }
        public DateTime? ContactDate { get; set; }
        public long ContactNoteId { get; set; }
        public bool IsFamilyOrGuardianInvolved { get; set; }
        public bool IsJuvenileInvolved { get; set; }
    }

    public class UpdateOffenderContactNoteCommandValidator : AbstractValidator<UpdateOffenderContactNoteCommand>
    {
        public UpdateOffenderContactNoteCommandValidator()
        {
            RuleFor(v => v.ContactTypeId).NotEmpty().WithMessage("Contact type is required.");
            RuleFor(v => v.ContactNoteId).NotEmpty().WithMessage("Contact note ID is required.");
            RuleFor(v => v.ContactedById).NotEmpty().WithMessage("Contacted by ID is required.");
            RuleFor(v => v.Comment).NotEmpty().WithMessage("Comment is required.");
        }
    }

    public class UpdateOffenderContactNoteCommandHandler : IRequestHandler<UpdateOffenderContactNoteCommand>
    {
        private readonly IApplicationDbContext dbContext;

        public UpdateOffenderContactNoteCommandHandler(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Unit> Handle(UpdateOffenderContactNoteCommand request, CancellationToken cancellationToken)
        {
            var contactNote = await dbContext.ContactNotes.SingleAsync(x => x.ContactNumber == request.ContactNoteId, cancellationToken);
            if (request.ContactedById != contactNote.ContactedById)
            {
                var contactedByStaff = await dbContext.Staff.Where(x => x.StaffKey == request.ContactedById).SingleAsync(cancellationToken);
                contactNote.ContactedById = request.ContactedById;
                contactNote.AgencyId = contactedByStaff.AgencyId;
            }
            contactNote.Comment = request.Comment;
            contactNote.ContactTypeId = request.ContactTypeId;
            contactNote.ContactDate = request.ContactDate;
            contactNote.IsFamilyOrGuardianInvolved = request.IsFamilyOrGuardianInvolved;
            contactNote.IsJuvenileInvolved = request.IsJuvenileInvolved;
            await dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}