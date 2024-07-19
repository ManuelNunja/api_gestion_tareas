using System.Threading;

namespace api_gestion_tareas.Models
{
    public class Archivo
    {
        public int ArchivoId { get; set; }
        public int TareaId { get; set; }
        public string NombreArchivo { get; set; }
        public string TipoArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }
}
