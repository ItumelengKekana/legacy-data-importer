IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [LegacyCustomerId] nvarchar(10) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [SignupDate] date NOT NULL,
    [Tier] int NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);

CREATE TABLE [ImportLogs] (
    [Id] int NOT NULL IDENTITY,
    [BatchId] uniqueidentifier NOT NULL,
    [CompletedAt] datetime2 NULL,
    [TotalProcessed] int NOT NULL,
    [CreatedCount] int NOT NULL,
    [UpdatedCount] int NOT NULL,
    [FailedCount] int NOT NULL,
    CONSTRAINT [PK_ImportLogs] PRIMARY KEY ([Id])
);

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [Currency] varchar(3) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);

CREATE TABLE [ImportErrors] (
    [Id] int NOT NULL IDENTITY,
    [lineNumber] int NOT NULL,
    [rawLine] nvarchar(max) NOT NULL,
    [reason] nvarchar(max) NOT NULL,
    [ImportLogId] int NOT NULL,
    CONSTRAINT [PK_ImportErrors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ImportErrors_ImportLogs_ImportLogId] FOREIGN KEY ([ImportLogId]) REFERENCES [ImportLogs] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    [Sku] nvarchar(50) NOT NULL,
    [UnitPrice] decimal(10,2) NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Customers_LegacyCustomerId] ON [Customers] ([LegacyCustomerId]);

CREATE INDEX [IX_ImportErrors_ImportLogId] ON [ImportErrors] ([ImportLogId]);

CREATE UNIQUE INDEX [IX_ImportLogs_BatchId] ON [ImportLogs] ([BatchId]);

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260820132515_InitialCreate', N'10.0.11');

COMMIT;
GO

