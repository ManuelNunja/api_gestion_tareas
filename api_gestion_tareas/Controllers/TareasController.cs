using Microsoft.AspNetCore.Mvc;
using api_gestion_tareas.Models;
using api_gestion_tareas.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace api_gestion_tareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TaskManagementContext _context;

        public TareasController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: api/Tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas()
        {
            var tareas = await _context.Tareas
                //.Include(t => t.Empresa)
                //.Include(t => t.Estado)
                .Where(t => t.Activo)
                .ToListAsync();
            return tareas;
        }

        // GET: api/Tareas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas
                //.Include(t => t.Empresa)
                //.Include(t => t.Estado)
                .FirstOrDefaultAsync(t => t.TareaId == id && t.Activo);

            if (tarea == null)
            {
                return NotFound();
            }

            return tarea;
        }

        // POST: api/Tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> PostTarea(Tarea tarea)
        {
            if (string.IsNullOrEmpty(tarea.Titulo) || tarea.EmpresaId <= 0)
            {
                return BadRequest("Título y Empresa son campos obligatorios.");
            }

            // Establecer el estado por defecto como "Pendiente"
            var estadoPendiente = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Pendiente");
            if (estadoPendiente == null)
            {
                return BadRequest("El estado 'Pendiente' no está definido en el sistema.");
            }
            tarea.EstadoId = estadoPendiente.EstadoId;

            tarea.FechaInicio = DateTime.Now;
            tarea.Activo = true;

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            // Guardar los archivos asociados
            if (tarea.Archivos != null && tarea.Archivos.Any())
            {
                foreach (var archivo in tarea.Archivos)
                {
                    archivo.TareaId = tarea.TareaId;
                    archivo.FechaSubida = DateTime.Now;
                    _context.Archivos.Add(archivo);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetTarea", new { id = tarea.TareaId }, tarea);
        }

        // PUT: api/Tareas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, Tarea tarea)
        {
            if (id != tarea.TareaId)
            {
                return BadRequest();
            }

            var existingTarea = await _context.Tareas.FindAsync(id);
            if (existingTarea == null || !existingTarea.Activo)
            {
                return NotFound();
            }

            var estadoEnProgresoId = (await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "En Progreso"))?.EstadoId;
            var estadoCompletadaId = (await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Completada"))?.EstadoId;

            existingTarea.Titulo = tarea.Titulo;
            existingTarea.Descripcion = tarea.Descripcion;
            existingTarea.EmpresaId = tarea.EmpresaId;
            existingTarea.FechaVencimiento = tarea.FechaVencimiento;
            existingTarea.EstadoId = tarea.EstadoId;

            if (tarea.EstadoId == estadoEnProgresoId && existingTarea.EstadoId != estadoEnProgresoId)
            {
                existingTarea.FechaInicio = DateTime.Now;
            }

            if (tarea.EstadoId == estadoCompletadaId && existingTarea.EstadoId != estadoCompletadaId)
            {
                existingTarea.FechaFin = DateTime.Now;
                existingTarea.TiempoTranscurrido = CalculateElapsedTime(existingTarea.FechaInicio, existingTarea.FechaFin);
            }

            _context.Entry(existingTarea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TareaExists(id))
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

        // DELETE: api/Tareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null || !tarea.Activo)
            {
                return NotFound();
            }

            tarea.Activo = false;
            _context.Entry(tarea).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Tareas/{id}/SubirArchivo
        [HttpPost("{id}/SubirArchivo")]
        public async Task<IActionResult> SubirArchivo(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha proporcionado ningún archivo.");
            }

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null || !tarea.Activo)
            {
                return NotFound();
            }

            var filePath = Path.Combine("uploads", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var archivo = new Archivo
            {
                TareaId = id,
                NombreArchivo = file.FileName,
                TipoArchivo = file.ContentType,
                RutaArchivo = filePath,
                FechaSubida = DateTime.Now
            };

            _context.Archivos.Add(archivo);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.TareaId == id);
        }

        private string CalculateElapsedTime(DateTime? start, DateTime? end)
        {
            if (start == null || end == null) return null;

            var timespan = end.Value - start.Value;
            return $"{timespan.Days} días, {timespan.Hours} horas, {timespan.Minutes} minutos, {timespan.Seconds} segundos";
        }
    }
}
