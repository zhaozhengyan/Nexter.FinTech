CREATE TABLE [dbo].[Transactions] (
    [Id]             BIGINT         CONSTRAINT [DF_Transactions_Id] DEFAULT ((0)) NOT NULL,
    [Memo]           NVARCHAR (128) NULL,
    [AccountId]      BIGINT         NOT NULL,
    [MemberId]       BIGINT         NOT NULL,
    [CategoryId]     BIGINT         NOT NULL,
    [Spending]       MONEY          CONSTRAINT [DF_Transactions_Spending] DEFAULT ((0)) NULL,
    [Income]         MONEY          CONSTRAINT [DF_Transactions_Income] DEFAULT ((0)) NULL,
    [Date]           DATETIME       NOT NULL,
    [CreatedAt]      DATETIME       CONSTRAINT [DF_Transaction_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [LastModifiedAt] DATETIME       CONSTRAINT [DF_Transaction_LastModifiedAt] DEFAULT (getdate()) NOT NULL,
    [BookId]         BIGINT         NULL,
    CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Transactions', @level2type = N'COLUMN', @level2name = N'CreatedAt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Transactions', @level2type = N'COLUMN', @level2name = N'Date';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收入', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Transactions', @level2type = N'COLUMN', @level2name = N'Income';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'花销', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Transactions', @level2type = N'COLUMN', @level2name = N'Spending';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帐户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Transactions', @level2type = N'COLUMN', @level2name = N'AccountId';

