using System.ComponentModel.DataAnnotations;

namespace gestion.productos.domain.Dtos
{
    public class RequestProductoDto
    {
        [Required]
        public string? Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public string? Imagen { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }
    }
}
