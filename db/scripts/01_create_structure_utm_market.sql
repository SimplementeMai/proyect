SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Usar la base de datos especificada
USE Mai_nwn;
GO

-- Comienza una transacción para asegurar la atomicidad de la creación de tablas
BEGIN TRANSACTION;

-- Tabla Producto
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Producto](
        ProductoID INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        SKU VARCHAR(20) UNIQUE NOT NULL,
        Marca NVARCHAR(50),
        Precio DECIMAL(19,4) NOT NULL,
        Stock INT NOT NULL,
        CONSTRAINT CK_Producto_Precio CHECK (Precio >= 0),
        CONSTRAINT CK_Producto_Stock CHECK (Stock >= 0)
    );
    PRINT 'Tabla Producto creada.';
END
ELSE
BEGIN
    PRINT 'La tabla Producto ya existe.';
END;
GO

-- Tabla Venta
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Venta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Venta](
        VentaID INT IDENTITY(1,1) PRIMARY KEY,
        Folio VARCHAR(20) UNIQUE NOT NULL,
        FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
        TotalArticulos INT NOT NULL,
        TotalVenta DECIMAL(19,4) NOT NULL,
        Estatus TINYINT NOT NULL,
        ClienteID INT NULL, -- Asociación opcional a Cliente
        CONSTRAINT FK_Venta_Cliente FOREIGN KEY (ClienteID) REFERENCES Cliente(ClienteID),
        CONSTRAINT CK_Venta_Estatus CHECK (Estatus IN (1, 2, 3))
        -- 1: Pendiente, 2: Completada, 3: Cancelada
    );
    PRINT 'Tabla Venta creada.';
END
ELSE
BEGIN
    PRINT 'La tabla Venta ya existe.';
END;
GO

-- Tabla Detalle de Venta
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleVenta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DetalleVenta](
        DetalleID INT IDENTITY(1,1) PRIMARY KEY,
        VentaID INT NOT NULL,
        ProductoID INT NOT NULL,
        PrecioUnitario DECIMAL(19,4) NOT NULL,
        Cantidad INT NOT NULL,
        TotalDetalle DECIMAL(19,4) NOT NULL,
        CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (VentaID) REFERENCES Venta(VentaID),
        CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (ProductoID) REFERENCES Producto(ProductoID),
        CONSTRAINT CK_DetalleVenta_PrecioUnitario CHECK (PrecioUnitario >= 0),
        CONSTRAINT CK_DetalleVenta_Cantidad CHECK (Cantidad >= 0)
    );
    PRINT 'Tabla DetalleVenta creada.';
END
ELSE
BEGIN
    PRINT 'La tabla DetalleVenta ya existe.';
END;
GO

-- Tabla Cliente (Customer)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cliente](
        ClienteID INT IDENTITY(1,1) PRIMARY KEY,
        NombreCompleto NVARCHAR(200) NOT NULL,
        Email NVARCHAR(150) UNIQUE NOT NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
    PRINT 'Tabla Cliente creada.';
END
ELSE
BEGIN
    PRINT 'La tabla Cliente ya existe.';
END;
GO

-- Confirmar la transacción
COMMIT TRANSACTION;
PRINT 'Script de creación de estructura completado.';
