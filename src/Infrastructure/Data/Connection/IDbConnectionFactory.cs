using System.Data;

namespace JDK.Infrastructure.Data.Connection;

/// <summary>
/// Define una interfaz para una factoría de conexiones a bases de datos,
/// permitiendo la abstracción del proveedor de la base de datos subyacente.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Crea y abre de forma asíncrona una nueva conexión a la base de datos.
    /// </summary>
    /// <returns>Una nueva instancia de una conexión a la base de datos.</returns>
    ValueTask<IDbConnection> CreateOpenConnectionAsync();
}
