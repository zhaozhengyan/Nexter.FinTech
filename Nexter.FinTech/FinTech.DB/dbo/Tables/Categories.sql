CREATE TABLE [dbo].[Categories] (
    [Id]             BIGINT        NOT NULL,
    [Name]           NVARCHAR (32) NULL,
    [Icon]           NVARCHAR (64) NULL,
    [Type]           NVARCHAR (16) NULL,
    [CreateMemberId] BIGINT        NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=Spending , 2= Income', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Categories', @level2type = N'COLUMN', @level2name = N'Type';

