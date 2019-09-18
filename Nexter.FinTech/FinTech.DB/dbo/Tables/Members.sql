CREATE TABLE [dbo].[Members] (
    [Id]          BIGINT         NOT NULL,
    [GroupId]     BIGINT         CONSTRAINT [DF_Member_GroupId] DEFAULT ((0)) NOT NULL,
    [NickName]    NVARCHAR (32)  NULL,
    [Avatar]      NVARCHAR (512) NULL,
    [AccountCode] NVARCHAR (256) NULL,
    [CreatedAt]   DATETIME       CONSTRAINT [DF_Members_CreatedAt] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20190825-181759]
    ON [dbo].[Members]([AccountCode] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Members', @level2type = N'COLUMN', @level2name = N'CreatedAt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'微信Token', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Members', @level2type = N'COLUMN', @level2name = N'AccountCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'头像', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Members', @level2type = N'COLUMN', @level2name = N'Avatar';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'昵称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Members', @level2type = N'COLUMN', @level2name = N'NickName';

