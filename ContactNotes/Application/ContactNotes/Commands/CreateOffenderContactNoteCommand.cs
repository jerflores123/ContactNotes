using FluentValidation;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Domain.Common.Constants;
using IJOS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ContactNotes.Commands
{
    [Authorize(Domain.Common.Constants.Features.Contact_Notes, Privileges.Create)]
    public class CreateOffenderContactNoteCommand : IRequest<long>
    {
        public long ContactedById { get; set; }
        public long Sin { get; set; }
        public long ContactTypeId { get; set; }
        public string Comment { get; set; }
        public DateTime? ContactDate { get; set; }
        public bool IsFamilyOrGuardianInvolved { get; set; }
        public bool IsJuvenileInvolved { get; set; }
    }

    public class CreateOffenderContactNoteCommandValidator : AbstractValidator<CreateOffenderContactNoteCommand>
    {
        public CreateOffenderContactNoteCommandValidator()
        {
            RuleFor(v => v.ContactTypeId).NotEmpty().WithMessage("Contact type is required.");
            RuleFor(v => v.Sin).NotEmpty().WithMessage("SIN is required.");
            RuleFor(v => v.ContactedById).NotEmpty().WithMessage("Contacted by is required.");
            RuleFor(v => v.Comment).NotEmpty().WithMessage("Comment is required.");
        }
    }

    public class CreateOffenderContactNoteCommandHandler : IRequestHandler<CreateOffenderContactNoteCommand, long>
    {
        private readonly IApplicationDbContext dbContext;

        public CreateOffenderContactNoteCommandHandler(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<long> Handle(CreateOffenderContactNoteCommand request, CancellationToken cancellationToken)
        {
            var contactedByStaff = await dbContext.Staff.Where(x => x.StaffKey == request.ContactedById).SingleAsync(cancellationToken);
            var contactNote = new ContactNote
            {
                ContactedById = request.ContactedById,
                Sin = request.Sin,
                ContactDate = request.ContactDate,
                ContactTypeId = request.ContactTypeId,
                Comment = request.Comment,
                AgencyId = contactedByStaff.AgencyId,
                IsJuvenileInvolved = request.IsJuvenileInvolved,
                IsFamilyOrGuardianInvolved = request.IsFamilyOrGuardianInvolved
            };
            dbContext.ContactNotes.Add(contactNote);
            await dbContext.SaveChangesAsync(cancellationToken);
            return contactNote.ContactNumber;
        }
    }
}