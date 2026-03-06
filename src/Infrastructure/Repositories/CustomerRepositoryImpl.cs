using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Infrastructure.Data.Connection;
using JDK.Infrastructure.Mappers;
using InfrastructureCustomerEntity = JDK.Infrastructure.Models.Data.CustomerEntity;
using Microsoft.Data.SqlClient;

namespace JDK.Infrastructure.Repositories;

/// <summary>
/// Implementación de ICustomerRepository compatible con Native AOT.
/// Utiliza Primary Constructors e inyección de IDbConnectionFactory.
/// </summary>
public class CustomerRepositoryImpl(IDbConnectionFactory connectionFactory) : ICustomerRepository
{
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = "SELECT ClienteID, NombreCompleto, Email, Activo FROM Cliente WHERE Email = @Email";
        command.CommandType = CommandType.Text;
        
        command.Parameters.Add(new SqlParameter("@Email", email));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var entity = new InfrastructureCustomerEntity(reader.GetInt32(0))
            {
                NombreCompleto = reader.GetString(1),
                Email = reader.GetString(2),
                Activo = reader.GetBoolean(3)
            };
            return entity.ToDomain();
        }
        return null;
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(customer);
        
        using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var command = (SqlCommand)connection.CreateCommand();
        command.CommandText = @"INSERT INTO Cliente (NombreCompleto, Email, Activo) 
                                VALUES (@NombreCompleto, @Email, @Activo);
                                SELECT SCOPE_IDENTITY();";
        command.CommandType = CommandType.Text;

        var entity = customer.ToEntity();

        command.Parameters.Add(new SqlParameter("@NombreCompleto", entity.NombreCompleto));
        command.Parameters.Add(new SqlParameter("@Email", entity.Email));
        command.Parameters.Add(new SqlParameter("@Activo", entity.Activo));

        var newId = await command.ExecuteScalarAsync(cancellationToken);
        if (newId != null && newId != DBNull.Value)
        {
            int customerId = Convert.ToInt32(newId);
            return new Customer(customerId)
            {
                FullName = customer.FullName,
                Email = customer.Email,
                IsActive = customer.IsActive
            };
        }
        throw new InvalidOperationException("No se pudo obtener el ID del nuevo cliente tras la inserción.");
    }
}
