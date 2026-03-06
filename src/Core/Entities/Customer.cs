using System;

namespace JDK.Core.Entities;

/// <summary>
/// Representa a un cliente del negocio.
/// Implementa reglas de negocio mediante la palabra clave 'field' de C# 14.
/// </summary>
public partial class Customer(int customerId)
{
    public int CustomerId { get; init; } = customerId;

    public string FullName { get; set; } = string.Empty;

    public required string Email
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            {
                throw new ArgumentException("El formato del email no es válido.", nameof(Email));
            }
            field = value;
        }
    }

    public bool IsActive { get; set; } = true;
}
