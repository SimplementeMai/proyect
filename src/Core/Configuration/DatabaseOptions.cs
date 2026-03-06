using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace JDK.Core.Configuration;

/// <summary>
/// Definición de las opciones de configuración de la base de datos (POCO para AOT).
/// Utiliza atributos de validación para asegurar la integridad de los datos de conexión.
/// </summary>
public sealed class DatabaseOptions
{
    public const string DatabaseSettings = "DatabaseSettings";

    [Required(ErrorMessage = "ServerName es requerido.")]
    public string ServerName { get; init; } = string.Empty;

    [Required(ErrorMessage = "DatabaseName es requerido.")]
    public string DatabaseName { get; init; } = string.Empty;

    public string? UserId { get; init; }
    public string? Password { get; init; }

    /// <summary>
    /// Construye la cadena de conexión completa a partir de ServerName y DatabaseName.
    /// Se asume Integrated Security=True y TrustServerCertificate=True por simplicidad
    /// en el ejemplo de desarrollo. Las credenciales de usuario/contraseña deben gestionarse
    /// a través de User Secrets o variables de entorno para producción.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = DatabaseName,
                TrustServerCertificate = true,
                MultipleActiveResultSets = false,
                ApplicationName = "SQL Server Management Studio",
                CommandTimeout = 2147483647 // Max timeout
            };

            if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Password))
            {
                builder.UserID = UserId;
                builder.Password = Password;
                builder.IntegratedSecurity = false;
            }
            else
            {
                builder.IntegratedSecurity = true;
            }

            return builder.ConnectionString;
        }
    }
}
