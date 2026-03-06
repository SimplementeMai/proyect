using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JDK.Core.Entities;
using JDK.Core.Filters;
using JDK.Core.Repositories;
using JDK.Infrastructure.Data.Connection;
using JDK.Infrastructure.Mappers; // Para los métodos de extensión ToDomain/ToEntity
using InfrastructureProductoEntity = JDK.Infrastructure.Models.Data.ProductoEntity; // Alias para la entidad de persistencia
using Microsoft.Data.SqlClient; // Necesario para SqlCommand y SqlDataReader
using System; // Para ArgumentNullException, InvalidOperationException, etc.
using System.Runtime.CompilerServices; // Para [EnumeratorCancellation]

namespace JDK.Infrastructure.Repositories;

/// <summary>
/// Implementación concreta de IProductRepository, compatible con Native AOT.
/// Utiliza mapeo manual con SqlDataReader y ProductMapper.
/// </summary>
public class ProductRepositoryImpl(IDbConnectionFactory connectionFactory) : IProductRepository
{
    /// <summary>
    /// Recupera todos los productos de forma asíncrona mediante streaming.
    /// </summary>
    public async IAsyncEnumerable<Product> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "SELECT ProductoID, Nombre, SKU, Marca, Precio, Stock FROM Producto";
        command.CommandType = CommandType.Text;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var entity = new InfrastructureProductoEntity(reader.GetInt32(0), reader.GetString(2))
            {
                Nombre = reader.GetString(1),
                Marca = reader.IsDBNull(3) ? string.Empty : reader.GetString(3), // Marca puede ser nula
                Precio = reader.GetDecimal(4),
                Stock = reader.GetInt32(5)
            };
            yield return entity.ToDomain();
        }
    }

    /// <summary>
    /// Recupera un producto específico por su identificador único.
    /// </summary>
    public async Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "SELECT ProductoID, Nombre, SKU, Marca, Precio, Stock FROM Producto WHERE ProductoID = @ProductId";
        command.CommandType = CommandType.Text;
        
        var param = command.CreateParameter();
        param.ParameterName = "@ProductId";
        param.Value = productId;
        command.Parameters.Add(param);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var entity = new InfrastructureProductoEntity(reader.GetInt32(0), reader.GetString(2))
            {
                Nombre = reader.GetString(1),
                Marca = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Precio = reader.GetDecimal(4),
                Stock = reader.GetInt32(5)
            };
            return entity.ToDomain();
        }
        return default;
    }

    /// <summary>
    /// Busca productos que coincidan con los criterios de filtro especificados.
    /// </summary>
    public async IAsyncEnumerable<Product> FindAsync(ProductFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filter);

        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        
        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>(); 

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            whereClauses.Add("Nombre LIKE @Name");
            parameters.Add(new SqlParameter("@Name", $"%{filter.Name}%"));
        }
        if (!string.IsNullOrWhiteSpace(filter.SKU))
        {
            whereClauses.Add("SKU = @SKU");
            parameters.Add(new SqlParameter("@SKU", filter.SKU));
        }
        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            whereClauses.Add("Marca LIKE @Brand");
            parameters.Add(new SqlParameter("@Brand", $"%{filter.Brand}%"));
        }
        if (filter.MinPrice.HasValue)
        {
            whereClauses.Add("Precio >= @MinPrice");
            parameters.Add(new SqlParameter("@MinPrice", filter.MinPrice.Value));
        }
        if (filter.MaxPrice.HasValue)
        {
            whereClauses.Add("Precio <= @MaxPrice");
            parameters.Add(new SqlParameter("@MaxPrice", filter.MaxPrice.Value));
        }
        if (filter.MinStock.HasValue)
        {
            whereClauses.Add("Stock >= @MinStock");
            parameters.Add(new SqlParameter("@MinStock", filter.MinStock.Value));
        }
        if (filter.MaxStock.HasValue)
        {
            whereClauses.Add("Stock <= @MaxStock");
            parameters.Add(new SqlParameter("@MaxStock", filter.MaxStock.Value));
        }

        var whereClause = whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
        command.CommandText = $"SELECT ProductoID, Nombre, SKU, Marca, Precio, Stock FROM Producto{whereClause}";
        command.CommandType = CommandType.Text;
        command.Parameters.AddRange(parameters.ToArray());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var entity = new InfrastructureProductoEntity(reader.GetInt32(0), reader.GetString(2))
            {
                Nombre = reader.GetString(1),
                Marca = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Precio = reader.GetDecimal(4),
                Stock = reader.GetInt32(5)
            };
            yield return entity.ToDomain();
        }
    }

    /// <summary>
    /// Agrega un nuevo producto al repositorio.
    /// </summary>
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = @"INSERT INTO Producto (Nombre, SKU, Marca, Precio, Stock) 
                                VALUES (@Nombre, @SKU, @Marca, @Precio, @Stock);
                                SELECT SCOPE_IDENTITY();";
        command.CommandType = CommandType.Text;

        var entity = product.ToEntity(); // Convertir a entidad de persistencia

        command.Parameters.Add(new SqlParameter("@Nombre", entity.Nombre));
        command.Parameters.Add(new SqlParameter("@SKU", entity.SKU));
        command.Parameters.Add(new SqlParameter("@Marca", (object?)entity.Marca ?? DBNull.Value)); // Manejo de nulos
        command.Parameters.Add(new SqlParameter("@Precio", entity.Precio));
        command.Parameters.Add(new SqlParameter("@Stock", entity.Stock));

        var newId = await command.ExecuteScalarAsync(cancellationToken);
        if (newId != null && newId != DBNull.Value)
        {
            int productId = Convert.ToInt32(newId);
            // Return a new Product instance with the generated ID, respecting immutability
            return new Product(productId, product.SKU)
            {
                Name = product.Name,
                Brand = product.Brand,
                Price = product.Price,
                Stock = product.Stock
            };
        }
        throw new InvalidOperationException("Failed to retrieve new product ID after insertion.");
    }

    /// <summary>
    /// Actualiza un producto existente en el repositorio.
    /// </summary>
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = @"UPDATE Producto SET Nombre = @Nombre, Marca = @Marca, 
                                Precio = @Precio, Stock = @Stock WHERE ProductoID = @ProductoID";
        command.CommandType = CommandType.Text;

        var entity = product.ToEntity();

        command.Parameters.Add(new SqlParameter("@Nombre", entity.Nombre));
        command.Parameters.Add(new SqlParameter("@Marca", (object?)entity.Marca ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Precio", entity.Precio));
        command.Parameters.Add(new SqlParameter("@Stock", entity.Stock));
        command.Parameters.Add(new SqlParameter("@ProductoID", entity.ProductoID));

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"Product with ID {product.ProductID} not found for update.");
        }
    }

    /// <summary>
    /// Actualiza la cantidad de stock de un producto específico.
    /// </summary>
    public async Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken)
    {
        if (newStock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newStock), "El nuevo stock no puede ser negativo.");
        }

        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "UPDATE Producto SET Stock = @NewStock WHERE ProductoID = @ProductId";
        command.CommandType = CommandType.Text;

        command.Parameters.Add(new SqlParameter("@NewStock", newStock));
        command.Parameters.Add(new SqlParameter("@ProductId", productId));

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"Product with ID {productId} not found for stock update.");
        }
    }
}
