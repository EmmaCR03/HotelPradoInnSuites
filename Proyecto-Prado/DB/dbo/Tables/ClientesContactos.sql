CREATE TABLE [dbo].[ClientesContactos] (
    [IdContacto]    INT           IDENTITY (1, 1) NOT NULL,
    [IdCliente]     INT           NOT NULL,
    [TipoContacto]  NVARCHAR (50) DEFAULT ('Teléfono') NULL,
    [ValorContacto] NVARCHAR (50) NOT NULL,
    [EsPrincipal]   BIT           DEFAULT ((0)) NULL,
    [FechaRegistro] DATETIME      DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([IdContacto] ASC),
    CONSTRAINT [FK_ClientesContactos_Clientes] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente])
);

