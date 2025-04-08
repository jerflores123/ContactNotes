using System;

namespace IJOS.Application.ContactNotes.Queries.GetContactNotes
{
    public class JuvenileContactNoteDto
    {
        public long ContactNumber { get; set; }
        public DateTime? ContactDate { get; set; }
        public long ContactTypeId { get; set; }
        public long? ContactedBy { get; set; }
        public string Comment { get; set; }
        public string OffenderFullName { get; set; }
        public long? AgencyId { get; set; }
        public bool IsFamilyOrGuardianInvolved { get; set; }
        public bool IsJuvenileInvolved { get; set; }
    }
}