using IJOS.Domain.Common;
using System;

namespace IJOS.Domain.Entities
{
    public class ContactNote : AuditableEntity
    {
        public long ContactNumber { get; set; }
        public long? ContactedById { get; set; }
        public long Sin { get; set; }
        public DateTime? ContactDate { get; set; }
        public long ContactTypeId { get; set; }
        public string Comment { get; set; }
        public long? AgencyId { get; set; }
        public bool IsFamilyOrGuardianInvolved { get; set; }
        public bool IsJuvenileInvolved { get; set; }

        public Agency Agency { get; set; }
        public Offender Offender { get; set; }
        public Staff ContactedByStaff { get; set; }
        public RefContactType ContactType { get; set; }
    }
}