CREATE TABLE [dbo].[Books] (
    [Id]   BIGINT        CONSTRAINT [DF_Books_Id] DEFAULT (NEXT VALUE FOR [Ids]) NOT NULL,
    [Type] INT           NOT NULL,
    [Name] NVARCHAR (32) NULL,
    CONSTRAINT [PK_Book] PRIMARY KEY CLUSTERED ([Id] ASC)
);

