using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JDK.Infrastructure.DependencyInjection; 
using System; // For Console, Convert, ArgumentNullException, etc.
using System.Threading; // For CancellationTokenSource, CancellationToken
using System.Threading.Tasks; // For Task, ValueTask, IAsyncEnumerable
using JDK.Core.Entities; // For Product, SaleStatus, etc.
using JDK.Core.UseCases; // For all Use Case interfaces
using JDK.Core.Filters; // For ProductFilter
using System.Globalization; // For CultureInfo
using System.Collections.Generic; // For List
using JDK.Core.Repositories; // For ICustomerRepository
using Microsoft.Extensions.Logging; // For ILogger

// CancellationTokenSource para gestionar la cancelación.
// En una aplicación real, esto se gestionaría a través de señales del sistema (Ctrl+C).
using CancellationTokenSource cts = new();

// Configuración del HostApplicationBuilder para .NET 10 y AOT
var builder = Host.CreateApplicationBuilder(args);

// Configurar servicios de base de datos e infraestructura
builder.Services.AddDatabaseServices(builder.Configuration);

// Configurar servicios de la capa de aplicación (casos de uso)
builder.Services.AddProductUseCases();
builder.Services.AddSaleUseCases();
builder.Services.AddCustomerUseCases();

// Configurar logging para la consola
builder.Logging.AddConsole();

var host = builder.Build();

// Iniciar la aplicación de consola con el menú interactivo
await RunInteractiveMenuAsync(host, cts.Token);

// ************************************************************
// Métodos auxiliares para la entrada de usuario
// ************************************************************
static string ReadStringInput(string prompt, bool canBeEmpty = false)
{
    Console.Write($@"{prompt}: ");
    var input = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!canBeEmpty && string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine(@"❌ La entrada no puede estar vacía. Por favor, intente de nuevo.");
        return ReadStringInput(prompt, canBeEmpty);
    }
    return input;
}

static decimal ReadDecimalInput(string prompt, bool canBeEmpty = false)
{
    Console.Write($@"{prompt}: ");
    var input = Console.ReadLine()?.Trim() ?? string.Empty;
    if (string.IsNullOrWhiteSpace(input))
    {
        if (canBeEmpty) return 0; // Or throw, depending on requirements
        Console.WriteLine(@"❌ La entrada no puede estar vacía. Por favor, intente de nuevo.");
        return ReadDecimalInput(prompt, canBeEmpty);
    }

    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
    {
        return value;
    }
    Console.WriteLine(@"❌ Formato de número inválido. Por favor, intente de nuevo con un número decimal.");
    return ReadDecimalInput(prompt, canBeEmpty);
}

static int ReadIntInput(string prompt, bool canBeEmpty = false)
{
    Console.Write($@"{prompt}: ");
    var input = Console.ReadLine()?.Trim() ?? string.Empty;
    if (string.IsNullOrWhiteSpace(input))
    {
        if (canBeEmpty) return 0; // Or throw
        Console.WriteLine(@"❌ La entrada no puede estar vacía. Por favor, intente de nuevo.");
        return ReadIntInput(prompt, canBeEmpty);
    }

    if (int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
    {
        return value;
    }
    Console.WriteLine(@"❌ Formato de número inválido. Por favor, intente de nuevo con un número entero.");
    return ReadIntInput(prompt, canBeEmpty);
}

static DateTime ReadDateInput(string prompt)
{
    Console.Write($@"{prompt} (yyyy-MM-dd): ");
    var input = Console.ReadLine()?.Trim() ?? string.Empty;
    if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
    {
        return date;
    }
    Console.WriteLine(@"❌ Formato de fecha inválido. Por favor, use yyyy-MM-dd.");
    return ReadDateInput(prompt);
}


// ************************************************************
// Lógica principal del menú interactivo
// ************************************************************
static async Task RunInteractiveMenuAsync(IHost host, CancellationToken appCancellationToken)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine($@"
╔══════════════════════════════════════════╗
║                UTMMARKET CLI             ║
╠══════════════════════════════════════════╣
║ 1. Consultar todos los productos         ║
║ 2. Consultar producto por ID             ║
║ 3. Registrar nuevo producto              ║
║ 4. Alertas de inventario crítico         ║
║ 5. Consultar ventas por fecha            ║
║ 6. Registrar nuevo cliente               ║
║ 7. Buscar cliente por email              ║
║ 8. Realizar nueva venta                  ║
║ 9. Salir                                 ║
╚══════════════════════════════════════════╝
");
        Console.Write(@"Seleccione una opción: ");
        var choice = Console.ReadLine();

        // Crear un scope para resolver los servicios de vida Scoped
        await using var scope = host.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            switch (choice)
            {
                case "1":
                    await ListAllProducts(serviceProvider, appCancellationToken);
                    break;
                case "2":
                    await GetProductById(serviceProvider, appCancellationToken);
                    break;
                case "3":
                    await CreateNewProduct(serviceProvider, appCancellationToken);
                    break;
                case "4":
                    await ShowLowStockAlerts(serviceProvider, appCancellationToken);
                    break;
                case "5":
                    await FetchSalesByDateRange(serviceProvider, appCancellationToken);
                    break;
                case "6":
                    await CreateNewCustomer(serviceProvider, appCancellationToken);
                    break;
                case "7":
                    await FindCustomerByEmail(serviceProvider, appCancellationToken);
                    break;
                case "8":
                    await CreateNewSale(serviceProvider, appCancellationToken);
                    break;
                case "9":
                    Console.WriteLine(@"Saliendo de la aplicación...");
                    return;
                default:
                    Console.WriteLine(@"Opción inválida. Por favor, intente de nuevo.");
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(@"Operación cancelada por el usuario.");
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, @"Se ha producido un error inesperado: {ErrorMessage}", ex.Message);
        }

        Console.WriteLine(@"Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}

// ************************************************************
// Métodos para las opciones del menú
// ************************************************************

static async Task ListAllProducts(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Listado de Todos los Productos ---");
    var getAllProductsUseCase = serviceProvider.GetRequiredService<IGetAllProductsUseCase>();
    
    try
    {
        var products = getAllProductsUseCase.ExecuteAsync(cancellationToken);
        int count = 0;
        await foreach (var product in products.WithCancellation(cancellationToken))
        {
            Console.WriteLine(@$"ID: {product.ProductID}, Nombre: {product.Name}, SKU: {product.SKU}, Marca: {product.Brand}, Precio: {product.Price:C}, Stock: {product.Stock}");
            count++;
        }
        Console.WriteLine($@"Total de productos: {count}");
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, @"Error al listar productos: {ErrorMessage}", ex.Message);
    }
}

static async Task GetProductById(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Consultar Producto por ID ---");
    var getProductByIdUseCase = serviceProvider.GetRequiredService<IGetProductByIdUseCase>();

    int productId = ReadIntInput(@"Ingrese el ID del producto", canBeEmpty: false);

    try
    {
        var product = await getProductByIdUseCase.ExecuteAsync(productId, cancellationToken);
        if (product != null)
        {
            Console.WriteLine(@$"Producto encontrado:");
            Console.WriteLine(@$"ID: {product.ProductID}, Nombre: {product.Name}, SKU: {product.SKU}, Marca: {product.Brand}, Precio: {product.Price:C}, Stock: {product.Stock}");
        }
        else
        {
            Console.WriteLine(@$"No se encontró ningún producto con ID {productId}.");
        }
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, @"Error al consultar producto por ID: {ErrorMessage}", ex.Message);
    }
}

static async Task CreateNewProduct(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Registrar Nuevo Producto ---");
    var createProductUseCase = serviceProvider.GetRequiredService<ICreateProductUseCase>();

    try
    {
        var name = ReadStringInput(@"Nombre del producto");
        var sku = ReadStringInput(@"SKU del producto");
        var brand = ReadStringInput(@"Marca del producto", canBeEmpty: true); // Marca puede ser opcional
        var price = ReadDecimalInput(@"Precio del producto");
        var stock = ReadIntInput(@"Stock inicial del producto");

        // Crear una nueva instancia de Product (la validación de reglas de negocio ocurre en el constructor/setters del dominio)
        var newProduct = new Product(0, sku) // ID 0 para un nuevo producto, la BD generará el real
        {
            Name = name,
            Brand = brand,
            Price = price,
            Stock = stock
        };

        var createdProduct = await createProductUseCase.ExecuteAsync(newProduct, cancellationToken);
        Console.WriteLine(@$"Producto '{createdProduct.Name}' registrado exitosamente con ID: {createdProduct.ProductID}.");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($@"❌ Error de validación: {ex.Message}");
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, @"Error al registrar nuevo producto: {ErrorMessage}", ex.Message);
    }
}

static async Task ShowLowStockAlerts(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Alertas de Inventario Crítico ---");
    var lowStockUseCase = serviceProvider.GetRequiredService<ILowStockAlertUseCase>();
    
    int threshold = ReadIntInput(@"Ingrese el umbral de stock");

    try
    {
        var products = lowStockUseCase.ExecuteAsync(threshold, cancellationToken);
        Console.WriteLine(@"----------------------------------------------------------------------");
        Console.WriteLine(@"| SKU      | Nombre                         | Stock | Precio    |");
        Console.WriteLine(@"----------------------------------------------------------------------");
        
        int count = 0;
        await foreach (var p in products.WithCancellation(cancellationToken))
        {
            Console.WriteLine(@$"| {p.SKU,-8} | {p.Name,-30} | {p.Stock,5} | {p.Price,9:C} |");
            count++;
        }
        Console.WriteLine(@"----------------------------------------------------------------------");
        Console.WriteLine($@"Total de productos críticos: {count}");
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, @"Error en alerta de stock: {ErrorMessage}", ex.Message);
    }
}

static async Task FetchSalesByDateRange(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Consultar Ventas por Fecha ---");
    var fetchSalesUseCase = serviceProvider.GetRequiredService<IFetchSalesByFilterUseCase>();

    var startDate = ReadDateInput(@"Fecha de Inicio");
    var endDate = ReadDateInput(@"Fecha de Fin");

    var filter = new SaleFilter 
    { 
        MinSaleDate = startDate, 
        MaxSaleDate = endDate.AddDays(1).AddSeconds(-1) // Hasta el final del día
    };

    try
    {
        var sales = fetchSalesUseCase.ExecuteAsync(filter, cancellationToken);
        
        Console.WriteLine(@"------------------------------------------------------------");
        Console.WriteLine(@"| Folio           | Fecha               | Total    |");
        Console.WriteLine(@"------------------------------------------------------------");

        int count = 0;
        decimal grandTotal = 0;

        await foreach (var sale in sales.WithCancellation(cancellationToken))
        {
            Console.WriteLine(@$"| {sale.Folio,-15} | {sale.SaleDate,-19:g} | {sale.TotalSale,8:C} |");
            grandTotal += sale.TotalSale;
            count++;
        }
        Console.WriteLine(@"------------------------------------------------------------");
        Console.WriteLine($@"Ventas encontradas: {count} | Total del periodo: {grandTotal:C}");
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, @"Error al consultar ventas: {ErrorMessage}", ex.Message);
    }
}

static async Task CreateNewCustomer(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Registrar Nuevo Cliente ---");
    var customerRepo = serviceProvider.GetRequiredService<ICustomerRepository>();

    try
    {
        var name = ReadStringInput(@"Nombre completo");
        var email = ReadStringInput(@"Email");

        var newCustomer = new Customer(0)
        {
            FullName = name,
            Email = email
        };

        var created = await customerRepo.AddAsync(newCustomer, cancellationToken);
        Console.WriteLine(@$"Cliente '{created.FullName}' registrado con ID: {created.CustomerId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($@"❌ Error: {ex.Message}");
    }
}

static async Task FindCustomerByEmail(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Buscar Cliente por Email ---");
    var findCustomerUseCase = serviceProvider.GetRequiredService<IFindCustomerByEmailUseCase>();

    var email = ReadStringInput(@"Ingrese el email del cliente");

    try
    {
        var customer = await findCustomerUseCase.ExecuteAsync(email, cancellationToken);
        if (customer != null)
        {
            Console.WriteLine(@$"Cliente encontrado:");
            Console.WriteLine(@$"ID: {customer.CustomerId}");
            Console.WriteLine(@$"Nombre: {customer.FullName}");
            Console.WriteLine(@$"Email: {customer.Email}");
            Console.WriteLine(@$"Estado: {(customer.IsActive ? "Activo" : "Inactivo")}");
        }
        else
        {
            Console.WriteLine(@$"No se encontró ningún cliente con el email: {email}");
        }
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, @"Error al buscar cliente: {ErrorMessage}", ex.Message);
    }
}

static async Task CreateNewSale(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    Console.WriteLine(@"--- Realizar Nueva Venta ---");
    var createSaleUseCase = serviceProvider.GetRequiredService<ICreateSaleUseCase>();
    var getProductByIdUseCase = serviceProvider.GetRequiredService<IGetProductByIdUseCase>();
    var findCustomerUseCase = serviceProvider.GetRequiredService<IFindCustomerByEmailUseCase>();

    try
    {
        // 1. Cliente opcional
        int? customerId = null;
        var associateCustomer = ReadStringInput(@"¿Desea asociar un cliente? (s/n)").ToLower() == "s";
        if (associateCustomer)
        {
            var email = ReadStringInput(@"Email del cliente");
            var customer = await findCustomerUseCase.ExecuteAsync(email, cancellationToken);
            if (customer != null)
            {
                customerId = customer.CustomerId;
                Console.WriteLine(@$"Cliente '{customer.FullName}' asociado.");
            }
            else
            {
                Console.WriteLine(@"❌ Cliente no encontrado. Se continuará sin asociar cliente.");
            }
        }

        // 2. Crear cabecera de venta
        var folio = "V-" + DateTime.Now.Ticks.ToString().Substring(10);
        var sale = new Sale(0, folio)
        {
            CustomerId = customerId,
            Status = SaleStatus.Completed
        };

        // 3. Agregar productos
        bool addMore = true;
        while (addMore)
        {
            int productId = ReadIntInput(@"ID del producto");
            var product = await getProductByIdUseCase.ExecuteAsync(productId, cancellationToken);
            
            if (product != null)
            {
                int quantity = ReadIntInput(@$"Cantidad para '{product.Name}' (Stock actual: {product.Stock})");
                if (quantity > product.Stock)
                {
                    Console.WriteLine(@"❌ Stock insuficiente.");
                }
                else
                {
                    var detail = new SaleDetail(0, product, quantity);
                    sale.AddSaleDetail(detail);
                    Console.WriteLine(@$"Producto agregado. Subtotal: {detail.TotalDetail:C}");
                }
            }
            else
            {
                Console.WriteLine(@"❌ Producto no encontrado.");
            }

            addMore = ReadStringInput(@"¿Agregar otro producto? (s/n)").ToLower() == "s";
        }

        if (sale.TotalItems == 0)
        {
            Console.WriteLine(@"❌ No se agregaron productos. Venta cancelada.");
            return;
        }

        // 4. Confirmar y Guardar
        Console.WriteLine(@$"Total de la venta ({sale.TotalItems} artículos): {sale.TotalSale:C}");
        var confirm = ReadStringInput(@"¿Confirmar venta? (s/n)").ToLower() == "s";
        
        if (confirm)
        {
            var createdSale = await createSaleUseCase.ExecuteAsync(sale, cancellationToken);
            Console.WriteLine(@$"✅ Venta registrada con éxito. Folio: {createdSale.Folio} | ID: {createdSale.SaleID}");
        }
        else
        {
            Console.WriteLine(@"Venta descartada.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($@"❌ Error: {ex.Message}");
    }
}