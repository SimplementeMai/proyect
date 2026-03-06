namespace JDK.Infrastructure.Models.Data;

/// <summary>
/// Modelo de datos para la tabla Cliente.
/// </summary>
public partial class CustomerEntity(int clienteId)
{
    public int ClienteID { get; init; } = clienteId;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
