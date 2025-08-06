using System.Text.Json.Serialization;

namespace gestion.transacciones.domain.Models;

public partial class Producto
{
    public Guid Id { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public string? Categoria { get; set; }

    public string? Imagen { get; set; }

    public decimal? Precio { get; set; }

    public int? Stock { get; set; }

    [JsonIgnore]
    public virtual ICollection<Transaccione>? Transacciones { get; set; } = null!;
}
