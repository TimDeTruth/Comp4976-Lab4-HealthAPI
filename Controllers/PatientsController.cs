using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthAPI.Data;
using HealthAPI.Models;
using Microsoft.AspNetCore.Cors;

namespace HealthAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("HealthPolicy")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly HealthContext _context;

        public PatientsController(HealthContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            return await _context.Patients
            .Include(p => p.Ailments)
            .Include(p => p.Medications)
            .ToListAsync();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            //var patient = await _context.Patients.FindAsync(id);
            var patient = await _context.Patients
                .Include(i => i.Ailments)
                .Include(m => m.Medications)
                .FirstOrDefaultAsync(i => i.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.PatientId)
            {
                return BadRequest();
            }

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Patients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'HealthContext.Patients'  is null.");
            }
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.PatientId }, patient);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET api/patients/3/medication
        [HttpGet("{id:int}/medication")]
        public async Task<IActionResult> GetMedications(int id)
        {
            var patient = await _context.Patients!
              .Include(m => m.Medications)
              .FirstOrDefaultAsync(i => i.PatientId == id);

            if (patient == null)
                return NotFound();

            return Ok(patient.Medications);
        }


        private bool PatientExists(int id)
        {
            return (_context.Patients?.Any(e => e.PatientId == id)).GetValueOrDefault();
        }
    }
}
