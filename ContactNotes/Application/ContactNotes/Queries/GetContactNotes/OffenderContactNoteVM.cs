using System.Collections.Generic;

namespace IJOS.Application.ContactNotes.Queries.GetContactNotes
{
    public class OffenderContactNoteVM
    {
        public string OffenderFullName { get; set; }
        public List<JuvenileContactNoteDto> JuvenileContactNoteDtoList { get; set; }
        public JuvenileContactNoteDto JuvenileContactNoteDto { get; set; }
        public List<StaffDto> Staff { get; set; }
    }
}
