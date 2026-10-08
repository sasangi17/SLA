using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Niwahana_backend.Business;
using Niwahana_backend.Models.Domain;
using Niwahana_backend.Models.Dto;

namespace Niwahana_backend.Controllers
{
    [ApiController]
    [Route("api/bungalows")]
    [Authorize]
    public class BungalowsController : ControllerBase
    {
        private readonly BungalowsBusiness _bungalowsBusiness;
        public BungalowsController(BungalowsBusiness bungalowsBusiness)
        {
            _bungalowsBusiness = bungalowsBusiness;
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
    }
}