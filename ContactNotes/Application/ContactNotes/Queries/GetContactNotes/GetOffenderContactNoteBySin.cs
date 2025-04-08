using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Domain.Common.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ContactNotes.Queries.GetContactNotes
{
    [Authorize(Features.Contact_Notes, Privileges.Read)]
    public class GetOffenderContactNoteBySinRequest : IRequest<List<GetOffenderContactNoteBySinResponse>>
    {
        public long Sin { get; set; }
        public long Contact_number { get; set; }
    }

    public class GetOffenderContactNoteBySinResponse
    {
        public long ContactNumber { get; set; }
        public DateTime? ContactDate { get; set; }
        public string ContactType { get; set; }
        public string ContactedByStaff { get; set; }
        public string Comment { get; set; }
        public string CreatedByName { get; set; }
        public long CreatedByStaffKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AgencyName { get; set; }
    }

    public class GetOffenderContactNoteBySinHandler : IRequestHandler<GetOffenderContactNoteBySinRequest, List<GetOffenderContactNoteBySinResponse>>
    {
        private readonly IApplicationDbContext dbContext;

        public GetOffenderContactNoteBySinHandler(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<GetOffenderContactNoteBySinResponse>> Handle(GetOffenderContactNoteBySinRequest request, CancellationToken cancellationToken)
        {
            var contactData = await dbContext.ContactNotes
                .Where(x => x.Sin == request.Sin)
                .Select(x => new
                {
                    x.ContactNumber,
                    ContactedByStaffFirstName = x.ContactedByStaff.FirstName,
                    ContactedByStaffLastName = x.ContactedByStaff.LastName,
                    x.ContactDate,
                    x.ContactType.ContactType,
                    x.Comment,
                    CreatedByFirstName = x.CreatedByStaff.FirstName,
                    CreatedByLastName = x.CreatedByStaff.LastName,
                    x.CreatedBy,
                    x.CreatedDate,
                    x.Agency.AgencyName
                }).ToListAsync(cancellationToken);
            return contactData.Select(x =>
               new GetOffenderContactNoteBySinResponse
               {
                   Comment = x.Comment,
                   ContactedByStaff = x.ContactedByStaffFirstName + " " + x.ContactedByStaffLastName,
                   ContactNumber = x.ContactNumber,
                   ContactDate = x.ContactDate,
                   ContactType = x.ContactType,
                   CreatedByName = x.CreatedByFirstName + " " + x.CreatedByLastName,
                   CreatedByStaffKey = x.CreatedBy,
                   CreatedDate = x.CreatedDate,
                   AgencyName = x.AgencyName,
               }).OrderByDescending(x => x.CreatedDate).ToList();
        }
    }
}