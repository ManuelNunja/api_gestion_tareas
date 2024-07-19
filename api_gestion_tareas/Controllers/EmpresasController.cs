using Microsoft.AspNetCore.Mvc;
using api_gestion_tareas.Models;
using api_gestion_tareas.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api_gestion_tareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        private readonly TaskManagementContext _context;

        public EmpresasController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: api/Empresas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetEmpresas()
        {
            // Listar todas las empresas registradas
            var empresas = await _context.Empresas.Where(e => e.Activo).ToListAsync();
            return empresas;
        }

        // GET: api/Empresas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empresa>> GetEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa == null || !empresa.Activo)
            {
                return NotFound();
            }

            return empresa;
        }

        // POST: api/Empresas
        [HttpPost]
        public async Task<ActionResult<Empresa>> PostEmpresa([FromBody] Empresa empresa)
        {
            // Validación de datos básicos
            if (string.IsNullOrEmpty(empresa.Nombre))
            {
                return BadRequest("El nombre de la empresa es obligatorio.");
            }

            // Registro de una nueva empresa
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmpresa", new { id = empresa.EmpresaId }, empresa);
        }

        // PUT: api/Empresas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpresa(int id, [FromBody] Empresa empresa)
        {
            if (id != empresa.EmpresaId)
            {
                return BadRequest();
            }

            var existingEmpresa = await _context.Empresas.FindAsync(id);
            if (existingEmpresa == null || !existingEmpresa.Activo)
            {
                return NotFound();
            }

            existingEmpresa.Nombre = empresa.Nombre;
            existingEmpresa.Direccion = empresa.Direccion;
            existingEmpresa.Telefono = empresa.Telefono;
            existingEmpresa.CorreoElectronico = empresa.CorreoElectronico;

            _context.Entry(existingEmpresa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(id))
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

        // DELETE: api/Empresas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null || !empresa.Activo)
            {
                return NotFound();
            }

            empresa.Activo = false;
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.EmpresaId == id);
        }
    }
}
