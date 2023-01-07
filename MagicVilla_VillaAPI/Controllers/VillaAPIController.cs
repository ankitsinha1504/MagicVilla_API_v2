using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db, ILogger<VillaAPIController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation("Getting All Values");
            return Ok(_db.Villas.ToList());
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Get Villa Error with Id" + id);
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK | StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> CreateVilla(VillaDTORequest villaDTORequest)
        {
            if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTORequest.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Name Already Exists!");
                return BadRequest(ModelState);
            }
            if (villaDTORequest == null)
            {
                return BadRequest(villaDTORequest);

            }
            Villa model = new()
            {
                Amenity = villaDTORequest.Amenity,
                Details = villaDTORequest.Details,
                ImageUrl = villaDTORequest.ImageUrl,
                Name = villaDTORequest.Name,
                Occupancy = villaDTORequest.Occupancy,
                Rate = villaDTORequest.Rate,
                Sqft = villaDTORequest.Sqft
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            _logger.LogInformation("Creating a new value..");
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);

        }
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, VillaDTORequest villaDTORequest)
        {
            if (villaDTORequest == null)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            villa.Amenity = villaDTORequest.Amenity;
            villa.Details = villaDTORequest.Details;
            villa.ImageUrl = villaDTORequest.ImageUrl;
            villa.Name = villaDTORequest.Name;
            villa.Occupancy = villaDTORequest.Occupancy;
            villa.Sqft = villaDTORequest.Sqft;
            villa.Rate = villaDTORequest.Rate;

            _db.Villas.Update(villa);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Id = villa.Id,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                Rate = villa.Rate,
                ImageUrl = villa.ImageUrl,
                Details = villa.Details
            };

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Details = villaDTO.Details,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(model);
        }
    }
}
