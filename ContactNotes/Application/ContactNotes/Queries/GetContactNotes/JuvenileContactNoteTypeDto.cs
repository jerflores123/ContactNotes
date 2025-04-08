using IJOS.Application.Common.Mappings;
using IJOS.Domain.Entities;
using System;

namespace IJOS.Application.ContactNotes.Queries.GetContactNotes
{
    public class JuvenileContactNoteTypeDto : IMapFrom<RefContactType>
    {
        public long ContactTypeId { get; set; }
        public string ContactType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
