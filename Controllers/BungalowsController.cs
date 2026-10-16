using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Niwahana_backend.Business;
using Niwahana_backend.Models.Domain;
using Niwahana_backend.Models.Dto;
using Niwahana_backend.Services;
using System.Security.Claims;


namespace Niwahana_backend.Controllers
{
    [ApiController]
    [Route("api/bungalows")]
    [Authorize(Roles = "Admin")]
    public class BungalowsController : ControllerBase
    {
        private readonly BungalowsBusiness _bungalowsBusiness;
        private readonly BungalowReportService _bungalowReportService;

        public BungalowsController(
            BungalowsBusiness bungalowsBusiness,
            BungalowReportService bungalowReportService)
        {
            _bungalowsBusiness = bungalowsBusiness;
            _bungalowReportService = bungalowReportService;
        }


        // GET ALL
        [HttpGet("getall")]
        public IActionResult GetAllBungalows()
        {
            var bungalows = _bungalowsBusiness.GetAllBungalows();

            var result = bungalows.Select(x => new BungalowsDto
                {
                    BungalowId = x.BungalowId,
                    BungalowName = x.BungalowName,
                    BungalowCode = x.BungalowCode,
                    BungalowLocation = x.BungalowLocation,
                    IsActive = x.IsActive
                }
            ).ToList();

            return Ok(result);
        }


        // GET BY ID
        [HttpGet("get")]
        public IActionResult GetBungalows(int bungalowId)
        {
            var bungalows = _bungalowsBusiness.GetBungalowsById(bungalowId);

            if (bungalows == null)
            {
                return NotFound(new
                {
                    message = "Bungalows not found."
                });
            }

            return Ok(new BungalowsDto
            {
                BungalowId = bungalows.BungalowId,
                BungalowName = bungalows.BungalowName,
                BungalowCode = bungalows.BungalowCode,
                BungalowLocation = bungalows.BungalowLocation,
                IsActive = bungalows.IsActive
            });
        }

        [HttpPost("addbungalow")]
        public IActionResult AddBungalow([FromBody] BungalowsMl bungalow)
        {
            if (bungalow == null)
            {
                return BadRequest(new
                {
                    message = "Bungalow data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(bungalow.BungalowName))
            {
                return BadRequest(new
                {
                    message = "Bungalow name is required."
                });
            }

            if (string.IsNullOrWhiteSpace(bungalow.BungalowCode))
            {
                return BadRequest(new
                {
                    message = "Bungalow code is required."
                });
            }

            if (string.IsNullOrWhiteSpace(bungalow.BungalowLocation))
            {
                return BadRequest(new
                {
                    message = "Bungalow location is required."
                });
            }

            var created = _bungalowsBusiness.AddBungalow(bungalow);

            if (!created)
            {
                return BadRequest(new
                {
                    message = "Unable to add bungalow."
                });
            }

            return Ok(new
            {
                message = "Bungalow added successfully."
            });
        }


        // UPDATE
        [HttpPut("updatebungalow")]
        public IActionResult UpdateBungalows(int bungalowId,[FromBody] BungalowsMl bungalows)
        {
            if (bungalows == null)
            {
                return BadRequest(new
                {
                    message = "Bungalows data is required."
                });
            }

            if (bungalowId != bungalows.BungalowId)
            {
                return BadRequest(new
                {
                    message = "Bungalow ID does not match."
                });
            }

            var existing =_bungalowsBusiness.GetBungalowsById(bungalowId);

            if (existing == null)
            {
                return NotFound(new
                {
                    message = "Bungalow not found."
                });
            }

            var updated =_bungalowsBusiness.UpdateBungalows(bungalows);

            if (!updated)
            {
                return BadRequest(new
                {
                    message = "Unable to update Bungalows."
                });
            }

            return Ok(new
            {
                message = "Bungalows updated successfully."
            });
        }

        //Delete Bungalow
        [HttpDelete("deletebungalow")]
        public IActionResult DeleteBungalow(int bungalowId)
        {
            var existing = _bungalowsBusiness.GetBungalowsById(bungalowId);

            if (existing == null)
            {
                return NotFound(new
                {
                    message = "Bungalow not found."
                });
            }

            var deleted = _bungalowsBusiness.DeleteBungalow(bungalowId);

            if (!deleted)
            {
                return BadRequest(new
                {
                    message = "Unable to delete bungalow."
                });
            }

            return Ok(new
            {
                message = "Bungalow deleted successfully."
            });
        }


        [HttpGet("report")]
        public IActionResult DownloadBungalowReport()
        {
            var bungalows = _bungalowsBusiness.GetAllBungalows();

            // Read the administrator identity from the validated JWT claims.
            var downloadedBy =
                User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.FindFirst("name")?.Value
                ?? User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? "Administrator";

            var generatedAt = DateTime.Now;

            var pdfBytes = _bungalowReportService.GenerateBungalowReport(
                bungalows,
                downloadedBy,
                generatedAt);

            var fileName =$"Bunga low-Report-{generatedAt:yyyyMMdd-HHmmss}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

    }
}