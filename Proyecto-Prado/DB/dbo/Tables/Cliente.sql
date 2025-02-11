CREATE TABLE [dbo].[Cliente] (
    [IdCliente]              INT            IDENTITY (1, 1) NOT NULL,
    [NombreCliente]          VARCHAR (50)   NULL,
    [PrimerApellidoCliente]  VARCHAR (50)   NULL,
    [SegundoApellidoCliente] VARCHAR (50)   NULL,
    [EmailCliente]           NVARCHAR (50)  NULL,
    [TelefonoCliente]        INT            NULL,
    [DireccionCliente]       NVARCHAR (100) NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED ([IdCliente] ASC)
);

