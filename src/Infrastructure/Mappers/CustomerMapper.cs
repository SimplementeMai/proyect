using JDK.Core.Entities;
using InfrastructureCustomerEntity = JDK.Infrastructure.Models.Data.CustomerEntity;
using System;

namespace JDK.Infrastructure.Mappers;

/// <summary>
/// Mapeador para la entidad Customer entre capas de dominio e infraestructura.
/// </summary>
public static partial class CustomerMapper
{
    public static Customer ToDomain(this InfrastructureCustomerEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new Customer(entity.ClienteID)
        {
            FullName = entity.NombreCompleto,
            Email = entity.Email,
            IsActive = entity.Activo
        };
    }

    public static InfrastructureCustomerEntity ToEntity(this Customer domain)
    {
        ArgumentNullException.ThrowIfNull(domain);
        return new InfrastructureCustomerEntity(domain.CustomerId)
        {
            NombreCompleto = domain.FullName,
            Email = domain.Email,
            Activo = domain.IsActive
        };
    }
}
