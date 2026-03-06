using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using JDK.Core.Configuration; // Nuevo: Para DatabaseOptions

namespace JDK.Infrastructure.Data.Connection;

/// <summary>
/// Factoría de conexiones para SQL Server, implementando IDbConnectionFactory.
/// Utiliza IOptions para obtener la cadena de conexión de forma segura.
/// </summary>
/// <param name="options">Opciones de configuración de la base de datos.</param>
public class SqlConnectionFactory(IOptions<DatabaseOptions> options) : IDbConnectionFactory
{
    private readonly string _connectionString = options.Value.ConnectionString;

    // Ya no se necesita la propiedad ConnectionString con init setter para validación aquí
    // porque la validación ocurre en DatabaseOptions con DataAnnotations y ValidateOnStart.
    // Pero si se necesitara validación adicional aquí, se haría en el constructor.

    /// <summary>
    /// Crea y abre de forma asíncrona una nueva conexión a SQL Server.
    /// </summary>
    /// <returns>Una nueva instancia de SqlConnection abierta.</returns>
    public async ValueTask<IDbConnection> CreateOpenConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
