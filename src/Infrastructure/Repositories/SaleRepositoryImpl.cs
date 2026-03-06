using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JDK.Core.Entities;
using JDK.Core.Filters;
using JDK.Core.Repositories;
using JDK.Infrastructure.Data.Connection;
using JDK.Infrastructure.Mappers;
using InfrastructureEntities = JDK.Infrastructure.Models.Data;
using Microsoft.Data.SqlClient;

namespace JDK.Infrastructure.Repositories;

/// <summary>
/// Implementación de ISaleRepository compatible con Native AOT.
/// </summary>
public class SaleRepositoryImpl(IDbConnectionFactory connectionFactory, IProductRepository productRepository) : ISaleRepository
{
    public async IAsyncEnumerable<Sale> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Por simplicidad y evitar N+1, usamos una implementación básica para este ejercicio.
        // En un escenario real, se usaría un JOIN o Dapper Multi-Mapping.
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "SELECT VentaID, Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus FROM Venta";
        
        var sales = new List<InfrastructureEntities.VentaEntity>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            sales.Add(new InfrastructureEntities.VentaEntity(reader.GetInt32(0), reader.GetString(1))
            {
                FechaVenta = reader.GetDateTime(2),
                TotalArticulos = reader.GetInt32(3),
                TotalVenta = reader.GetDecimal(4),
                Estatus = reader.GetByte(5)
            });
        }
        await reader.CloseAsync();

        foreach (var saleEntity in sales)
        {
            var details = await GetDetailsBySaleIdAsync(saleEntity.VentaID, cancellationToken);
            yield return saleEntity.ToDomain(details, id => productRepository.GetByIdAsync(id, cancellationToken).Result!);
        }
    }

    public async Task<Sale?> GetByIdAsync(int saleId, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "SELECT VentaID, Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus FROM Venta WHERE VentaID = @SaleId";
        command.Parameters.Add(new SqlParameter("@SaleId", saleId));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var saleEntity = new InfrastructureEntities.VentaEntity(reader.GetInt32(0), reader.GetString(1))
            {
                FechaVenta = reader.GetDateTime(2),
                TotalArticulos = reader.GetInt32(3),
                TotalVenta = reader.GetDecimal(4),
                Estatus = reader.GetByte(5)
            };
            await reader.CloseAsync();
            var details = await GetDetailsBySaleIdAsync(saleId, cancellationToken);
            return saleEntity.ToDomain(details, id => productRepository.GetByIdAsync(id, cancellationToken).Result!);
        }
        return null;
    }

    public async IAsyncEnumerable<Sale> FindAsync(SaleFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filter);

        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        
        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter.Folio))
        {
            whereClauses.Add("Folio LIKE @Folio");
            parameters.Add(new SqlParameter("@Folio", $"%{filter.Folio}%"));
        }
        if (filter.MinSaleDate.HasValue)
        {
            whereClauses.Add("FechaVenta >= @MinDate");
            parameters.Add(new SqlParameter("@MinDate", filter.MinSaleDate.Value));
        }
        if (filter.MaxSaleDate.HasValue)
        {
            whereClauses.Add("FechaVenta <= @MaxDate");
            parameters.Add(new SqlParameter("@MaxDate", filter.MaxSaleDate.Value));
        }
        if (filter.Status.HasValue)
        {
            whereClauses.Add("Estatus = @Status");
            parameters.Add(new SqlParameter("@Status", (byte)filter.Status.Value));
        }

        var whereClause = whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
        command.CommandText = $"SELECT VentaID, Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus FROM Venta{whereClause}";
        command.Parameters.AddRange(parameters.ToArray());

        var sales = new List<InfrastructureEntities.VentaEntity>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            sales.Add(new InfrastructureEntities.VentaEntity(reader.GetInt32(0), reader.GetString(1))
            {
                FechaVenta = reader.GetDateTime(2),
                TotalArticulos = reader.GetInt32(3),
                TotalVenta = reader.GetDecimal(4),
                Estatus = reader.GetByte(5)
            });
        }
        await reader.CloseAsync();

        foreach (var saleEntity in sales)
        {
            var details = await GetDetailsBySaleIdAsync(saleEntity.VentaID, cancellationToken);
            // Nota: .Result en un entorno asíncrono no es ideal, pero para este ejercicio de consola y Native AOT simplifica.
            yield return saleEntity.ToDomain(details, id => productRepository.GetByIdAsync(id, cancellationToken).GetAwaiter().GetResult()!);
        }
    }

    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sale);

        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            using var saleCommand = (SqlCommand)connection.CreateCommand();
            saleCommand.Transaction = (SqlTransaction)transaction;
            saleCommand.CommandText = @"INSERT INTO Venta (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus, ClienteID) 
                                        VALUES (@Folio, @FechaVenta, @TotalArticulos, @TotalVenta, @Estatus, @ClienteID);
                                        SELECT SCOPE_IDENTITY();";
            
            var saleEntity = sale.ToEntity();
            saleCommand.Parameters.Add(new SqlParameter("@Folio", saleEntity.Folio));
            saleCommand.Parameters.Add(new SqlParameter("@FechaVenta", saleEntity.FechaVenta));
            saleCommand.Parameters.Add(new SqlParameter("@TotalArticulos", saleEntity.TotalArticulos));
            saleCommand.Parameters.Add(new SqlParameter("@TotalVenta", saleEntity.TotalVenta));
            saleCommand.Parameters.Add(new SqlParameter("@Estatus", saleEntity.Estatus));
            saleCommand.Parameters.Add(new SqlParameter("@ClienteID", (object?)saleEntity.ClienteID ?? DBNull.Value));

            var newSaleId = Convert.ToInt32(await saleCommand.ExecuteScalarAsync(cancellationToken));

            foreach (var detail in sale.SaleDetails)
            {
                using var detailCommand = (SqlCommand)connection.CreateCommand();
                detailCommand.Transaction = (SqlTransaction)transaction;
                detailCommand.CommandText = @"INSERT INTO DetalleVenta (VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle)
                                            VALUES (@VentaID, @ProductoID, @PrecioUnitario, @Cantidad, @TotalDetalle)";
                
                var detailEntity = detail.ToEntity(newSaleId);
                detailCommand.Parameters.Add(new SqlParameter("@VentaID", detailEntity.VentaID));
                detailCommand.Parameters.Add(new SqlParameter("@ProductoID", detailEntity.ProductoID));
                detailCommand.Parameters.Add(new SqlParameter("@PrecioUnitario", detailEntity.PrecioUnitario));
                detailCommand.Parameters.Add(new SqlParameter("@Cantidad", detailEntity.Cantidad));
                detailCommand.Parameters.Add(new SqlParameter("@TotalDetalle", detailEntity.TotalDetalle));

                await detailCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            transaction.Commit();

            return await GetByIdAsync(newSaleId, cancellationToken) ?? throw new InvalidOperationException("Error al recuperar la venta recién creada.");
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("UpdateAsync no es requerido para este ejercicio de expansión.");
    }

    private async Task<List<InfrastructureEntities.DetalleVentaEntity>> GetDetailsBySaleIdAsync(int saleId, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "SELECT DetalleID, VentaID, ProductoID, PrecioUnitario, Cantidad, TotalDetalle FROM DetalleVenta WHERE VentaID = @SaleId";
        command.Parameters.Add(new SqlParameter("@SaleId", saleId));

        var details = new List<InfrastructureEntities.DetalleVentaEntity>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            details.Add(new InfrastructureEntities.DetalleVentaEntity(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2))
            {
                PrecioUnitario = reader.GetDecimal(3),
                Cantidad = reader.GetInt32(4),
                TotalDetalle = reader.GetDecimal(5)
            });
        }
        return details;
    }
}
