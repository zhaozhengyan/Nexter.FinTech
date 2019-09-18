CREATE TABLE [dbo].[Accounts] (
    [Id]       BIGINT         NOT NULL,
    [MemberId] BIGINT         CONSTRAINT [DF_Accounts_MemberId] DEFAULT ((0)) NOT NULL,
    [Name]     NVARCHAR (128) NULL,
    [Type]     NVARCHAR (50)  NULL,
    [Icon]     NVARCHAR (256) NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'现金钱包', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Accounts', @level2type = N'COLUMN', @level2name = N'Type';

