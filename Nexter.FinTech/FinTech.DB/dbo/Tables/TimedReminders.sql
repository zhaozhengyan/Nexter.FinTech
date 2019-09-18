CREATE TABLE [dbo].[TimedReminders] (
    [Id]             BIGINT         NOT NULL,
    [MemberId]       BIGINT         NOT NULL,
    [IsEnabled]      BIT            NOT NULL,
    [Cron]           NVARCHAR (64)  CONSTRAINT [DF_TimedReminders_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [LastReminderAt] DATETIME       CONSTRAINT [DF_TimedReminders_CreatedAt1] DEFAULT (getdate()) NULL,
    [FormId]         NVARCHAR (128) NULL,
    CONSTRAINT [PK_TimedReminders] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TimedReminders', @level2type = N'COLUMN', @level2name = N'LastReminderAt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'表达式', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TimedReminders', @level2type = N'COLUMN', @level2name = N'Cron';

