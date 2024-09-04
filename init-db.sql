-- Crear la base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PERMISSION_TEST')
BEGIN
    CREATE DATABASE [PERMISSION_TEST];
END;
GO

-- Usar la base de datos creada
USE PERMISSION_TEST;
GO

-- Crear la tabla PermissionTypes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PermissionTypes')
BEGIN
	CREATE TABLE [dbo].[PermissionTypes] (
	    [Id]          INT  IDENTITY (1, 1) NOT NULL,
	    [Description] TEXT NOT NULL,
	    CONSTRAINT [PK_PermissionType] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
END;
GO

-- Agregar propiedades extendidas a PermissionTypes
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Permission description', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PermissionTypes', @level2type = N'COLUMN', @level2name = N'Description';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Unique ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PermissionTypes', @level2type = N'COLUMN', @level2name = N'Id';
GO

-- Crear la tabla Permissions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
CREATE TABLE [dbo].[Permissions] (
    [Id]               INT  IDENTITY (1, 1) NOT NULL,
    [EmployeeForename] TEXT NOT NULL,
    [EmployeeSurname]  TEXT NOT NULL,
    [PermissionTypeId] INT  NOT NULL,
    [PermissionDate]   DATE NOT NULL,
    CONSTRAINT [FK_Permissions_PermissionsType] FOREIGN KEY ([PermissionTypeId]) REFERENCES [dbo].[PermissionTypes] ([Id])
);
END;
GO

-- Agregar propiedades extendidas a Permissions
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Permission Type', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Permissions', @level2type = N'COLUMN', @level2name = N'PermissionTypeId';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Permission granted on Date', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Permissions', @level2type = N'COLUMN', @level2name = N'PermissionDate';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Unique ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Permissions', @level2type = N'COLUMN', @level2name = N'Id';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Employee Forename', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Permissions', @level2type = N'COLUMN', @level2name = N'EmployeeForename';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Employee Surname', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Permissions', @level2type = N'COLUMN', @level2name = N'EmployeeSurname';
GO

-- Insertar registros en PermissionTypes
INSERT INTO [dbo].[PermissionTypes] ([Description]) VALUES ('Crear');
INSERT INTO [dbo].[PermissionTypes] ([Description]) VALUES ('Modificar');
GO
