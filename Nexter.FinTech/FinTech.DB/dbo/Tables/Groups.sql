CREATE TABLE [dbo].[Groups] (
    [Id]             BIGINT        NOT NULL,
    [Name]           NVARCHAR (32) NULL,
    [CreateMemberId] BIGINT        NULL,
    [CreatedAt]      DATETIME      CONSTRAINT [DF_Groups_CreatedAt] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Groups', @level2type = N'COLUMN', @level2name = N'CreatedAt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'我的家庭', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Groups', @level2type = N'COLUMN', @level2name = N'Name';

