namespace api_gestion_tareas.Models
{
    public class Tarea
    {
        public int TareaId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int EmpresaId { get; set; }
        //public int Empresa { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int EstadoId { get; set; }
        //public int Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string TiempoTranscurrido { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Archivo> Archivos { get; set; }
    }
}
