using IJOS.Application.ContactNotes.Commands;
using IJOS.Application.ContactNotes.Queries.GetContactNotes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IJOS.WebUI.Controllers
{
    public class OffenderContactNotesController : ApiControllerBase
    {
        private readonly ILogger<OffenderContactNotesController> _logger;

        public OffenderContactNotesController(ILogger<OffenderContactNotesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{sin},{Contact_number},{agencyId}")]
        public async Task<ActionResult<OffenderContactNoteVM>> Get(long sin, long Contact_number, int agencyId)
        {
            try
            {
                var offadd = await Mediator.Send(new GetOffenderContactNoteById() { ContactNumber = Contact_number, Sin = sin, AgencyId = agencyId });
                return offadd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpGet("{sin}")]
        public async Task<ActionResult<List<GetOffenderContactNoteBySinResponse>>> GetAll(long sin)
        {
            try
            {
                var offadd = await Mediator.Send(new GetOffenderContactNoteBySinRequest() { Sin = sin });
                return offadd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateOffenderContactNoteCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest();
                }
                await Mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create(CreateOffenderContactNoteCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest();
                return await Mediator.Send(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                await Mediator.Send(new DeleteOffenderContactNoteCommand { Offender_ContactNote_number = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
