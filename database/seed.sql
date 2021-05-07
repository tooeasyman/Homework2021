USE [Mouladb]

go

SET ansi_nulls ON

go

SET quoted_identifier ON

go

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = Object_id(N'[dbo].[Payments]')
                  AND type IN ( N'U' ))
  DROP TABLE [dbo].[payments]

go

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = Object_id(N'[dbo].[PaymentStatuses]')
                  AND type IN ( N'U' ))
  DROP TABLE [dbo].[paymentstatuses]

go

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = Object_id(N'[dbo].[UserTokens]')
                  AND type IN ( N'U' ))
  DROP TABLE [dbo].[usertokens]

go

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = Object_id(N'[dbo].[UserBalances]')
                  AND type IN ( N'U' ))
  DROP TABLE [dbo].[userbalances]

go

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = Object_id(N'[dbo].[Users]')
                  AND type IN ( N'U' ))
  DROP TABLE [dbo].[users]

go

IF Object_id('MakePayment', 'P') IS NOT NULL
  DROP PROCEDURE [dbo].[MakePayment]

go

CREATE TABLE [dbo].[users]
  (
     [userid]       [UNIQUEIDENTIFIER] NOT NULL,
     [username]     [NVARCHAR](50) NOT NULL,
     [firstname]    [NVARCHAR](50) NOT NULL,
     [lastname]     [NVARCHAR](50) NULL,
     [passwordhash] [NVARCHAR](max) NOT NULL,
     [passwordsalt] [NVARCHAR](max) NOT NULL,
     CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ( [userid] ASC )WITH (pad_index
     = OFF, statistics_norecompute = OFF, ignore_dup_key = OFF, allow_row_locks
     = on, allow_page_locks = on) ON [PRIMARY]
  )
ON [PRIMARY]
textimage_on [PRIMARY]

go

CREATE TABLE [dbo].[usertokens]
  (
     [usertokenid]     [UNIQUEIDENTIFIER] NOT NULL,
     [userid]          [UNIQUEIDENTIFIER] NOT NULL,
     [token]           [NVARCHAR](max) NOT NULL,
     [jwttoken]        [NVARCHAR](max) NOT NULL,
     [expiredate]      [DATETIME] NOT NULL,
     [replacedbytoken] [NVARCHAR](max) NULL,
     [createddate]     [DATETIME] NOT NULL,
     [revokeddate]     [DATETIME] NULL,
     [createdbyip]     [NVARCHAR](50) NOT NULL,
     [revokedbyip]     [NVARCHAR](50) NULL,
     CONSTRAINT [PK_UserTokens] PRIMARY KEY CLUSTERED ( [usertokenid] ASC )WITH
     (pad_index = OFF, statistics_norecompute = OFF, ignore_dup_key = OFF,
     allow_row_locks = on, allow_page_locks = on) ON [PRIMARY]
  )
ON [PRIMARY]
textimage_on [PRIMARY]

go

ALTER TABLE [dbo].[usertokens]
  WITH CHECK ADD CONSTRAINT [FK_UserTokens_Users] FOREIGN KEY([userid])
  REFERENCES [dbo].[users] ([userid])

go

ALTER TABLE [dbo].[usertokens]
  CHECK CONSTRAINT [FK_UserTokens_Users]

go

CREATE TABLE [dbo].[userbalances]
  (
     [userbalanceid] [UNIQUEIDENTIFIER] NOT NULL,
     [userid]        [UNIQUEIDENTIFIER] NOT NULL UNIQUE,
     [balance]       [DECIMAL](19, 4) NOT NULL DEFAULT 0
     CONSTRAINT [PK_UserBalance] PRIMARY KEY CLUSTERED ( [userbalanceid] ASC )
     WITH (pad_index = OFF, statistics_norecompute = OFF, ignore_dup_key = OFF,
     allow_row_locks = on, allow_page_locks = on) ON [PRIMARY]
  )
ON [PRIMARY]

go

ALTER TABLE [dbo].[userbalances]
  WITH CHECK ADD CONSTRAINT [FK_UserBalances_Users] FOREIGN KEY([userid])
  REFERENCES [dbo].[users] ([userid])

go

CREATE TABLE [dbo].[paymentstatuses]
  (
     [paymentstatusid]   [INT] IDENTITY(1, 1) NOT NULL,
     [statusdescription] [NVARCHAR](50) NULL,
     [statusvalue]       [NVARCHAR](50) NOT NULL,
     CONSTRAINT [PK_PaymentStatuses] PRIMARY KEY CLUSTERED ( [paymentstatusid]
     ASC )WITH (pad_index = OFF, statistics_norecompute = OFF, ignore_dup_key =
     OFF, allow_row_locks = on, allow_page_locks = on) ON [PRIMARY]
  )
ON [PRIMARY]

go

CREATE TABLE [dbo].[payments]
  (
     [paymentid]       [UNIQUEIDENTIFIER] NOT NULL,
     [userid]          [UNIQUEIDENTIFIER] NOT NULL,
     [amount]          [DECIMAL](19, 4) NOT NULL,
     [payto]           [UNIQUEIDENTIFIER] NOT NULL,
     [paymentstatusid] [INT] NOT NULL,
     [closedreason]    [NVARCHAR](50) NULL,
     [date]            [DATETIME] NOT NULL,
     CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ( [paymentid] ASC )WITH (
     pad_index = OFF, statistics_norecompute = OFF, ignore_dup_key = OFF,
     allow_row_locks = on, allow_page_locks = on) ON [PRIMARY]
  )
ON [PRIMARY]

go

ALTER TABLE [dbo].[payments]
  WITH CHECK ADD CONSTRAINT [FK_Payments_PaymentStatuses] FOREIGN KEY(
  [paymentstatusid]) REFERENCES [dbo].[paymentstatuses] ([paymentstatusid])

go

CREATE PROCEDURE [dbo].[Makepayment] (@UserId UNIQUEIDENTIFIER,
                                      @Amount DECIMAL(19, 4),
                                      @PayTo  UNIQUEIDENTIFIER)
AS
  BEGIN
	  DECLARE @PaymentId       UNIQUEIDENTIFIER,
              @PaymentStatusId [INT],
			  @Count [INT],
              @ClosedReason    NVARCHAR(50),
              @Date            DATETIME
			  

      SET @PaymentId = Newid()
      SET @Date = Getdate()
      SET @PaymentStatusId = 1

      SELECT @Count = COUNT(*)
      FROM   [dbo].[users]
      WHERE  userid = @UserId

      IF @Count < 1
        BEGIN
            SET @PaymentStatusId = 2
            SET @ClosedReason = 'From account does not exist'
        END

      SELECT @Count = COUNT(*)
      FROM   [dbo].[users]
      WHERE  userid = @PayTo

      IF @Count < 1
        BEGIN
            SET @PaymentStatusId = 2
            SET @ClosedReason = 'To account does not exist'
        END

      SELECT @Count = COUNT(*)
      FROM   [dbo].[userbalances] WITH(xlock, rowlock)
      WHERE  userid = @UserId
             AND balance >= @Amount

      IF @Count < 1
        BEGIN
            SET @PaymentStatusId = 2
            SET @ClosedReason = 'Insufficient funding'
        END

      SELECT @Count = COUNT(*)
      FROM   [dbo].[userbalances] WITH(xlock, rowlock)
      WHERE  userid = @PayTo

      IF @Count < 1
        BEGIN
            SET @PaymentStatusId = 2
            SET @ClosedReason = 'To account has invalid balance'
        END

      IF @PaymentStatusId = 1
        BEGIN
            UPDATE [dbo].[userbalances]
            SET    balance = balance - @Amount
            WHERE  userid = @UserId

            UPDATE [dbo].[userbalances]
            SET    balance = balance + @Amount
            WHERE  userid = @PayTo

            SET @ClosedReason = N'Confirmed by recipient'
        END

      INSERT [dbo].[payments]
             ([paymentid],
              [userid],
              [amount],
              [payto],
              [paymentstatusid],
              [closedreason],
              [date])
      VALUES (@PaymentId,
              @UserId,
              @Amount,
              @PayTo,
              @PaymentStatusId,
              @ClosedReason,
              @Date)
			        
      SELECT @PaymentId as PaymentId
  END

go 

SET IDENTITY_INSERT [dbo].[PaymentStatuses] ON 
INSERT [dbo].[PaymentStatuses] ([PaymentStatusId], [StatusDescription], [StatusValue]) VALUES (1, N'Paid', N'Paid')
INSERT [dbo].[PaymentStatuses] ([PaymentStatusId], [StatusDescription], [StatusValue]) VALUES (2, N'Declinced', N'Declinced')
SET IDENTITY_INSERT [dbo].[PaymentStatuses] OFF

INSERT [dbo].[Users] ([UserId], [UserName], [FirstName], [LastName], [PasswordHash], [PasswordSalt]) VALUES (N'4e55821a-db19-48ec-a69b-a5e970009844', N'admin', N'Jim', N'Green', N'jy4ckfx0M+fF2gDXTnB9Wlj61cwAzJj3mtidt0RL9OJ+YPc3yekqbIYwHlWBM4fdPow9D1hszy14alwFOqrDig==', N'gW3V9Py/jKCQQWgw0fhfiFLayjMvuA0sadsAUEq8RktZCqTrxzmAO4hjG04E6Powoec+Z9SZkkPSFNJRZnaI3w9ZnwYz6RMBFQc8QI5/IYnlu5H1etf6UjyS9fZxQ/0GG1CcNGv2b8LgZtDCxoS1CdpHid9HAi8oBxpZNGTkJB4=')
INSERT [dbo].[Users] ([UserId], [UserName], [FirstName], [LastName], [PasswordHash], [PasswordSalt]) VALUES (N'669293e1-1138-4f85-b32f-703271d8f216', N'user1', N'test1', N'user', N'jvRu3Rc32lBq7gvV+iL8QFxI9chWWBIjkzeX2J+qlZq1hxjvUuEcd3/yo48b4ByiLnPM8lQC5GEXkpUXyQPQBQ==', N'jun+ZVCZUWYpky3sEEbBnPpdQyS9BWUZZJPV8rKhvmtAojQCDreqCgOGBO4yXE3neV8LJKDfgt3410ceYGZ7FHUIvNwEnVl8uVCa4h9L9WtR/S2fMiUYKxwnmdnd8A2tPOwD5R3ew+fTxkUtHI0Z4o9xLVsqHmqo5+OjCDx+pok=')
INSERT [dbo].[Users] ([UserId], [UserName], [FirstName], [LastName], [PasswordHash], [PasswordSalt]) VALUES (N'64752bb6-cfd9-4966-9367-d7a79f09ed1d', N'user2', N'test2', N'user', N'D00GYJJoeisjYne8cpHLxMMoMHFS2+vR2HJzviyRQCUNkacfhT+GDF2XRmWe3elLH+ylQzBfAZ/c62ELkHYeZg==', N'AQ+a9lWj05+i6do/EQu3qqAKDps1RaNG+G1EfHYRwTouxmvlGozlaEeBX0IURuEWSlxBnTNOQZcbzZf7Cwsy/rnQjieJ2LIc0o15cX5ZV6qb3yPazEkRbKrJq80ZSD+scAfZeHpl7XCKaA2CvzpEZk0VrixjkzQyODO85LNGItc=')
INSERT [dbo].[Users] ([UserId], [UserName], [FirstName], [LastName], [PasswordHash], [PasswordSalt]) VALUES (N'936928f5-7b8c-458c-b485-bf4b7cea91a9', N'user3', N'test3', N'user', N'7qgn3rfEPfPYEpIGWEBzshSqlpHDXae5UpPEMf3r3FvOHqWgo4Zp2Hc6sIl4Kij1x4Q0YpSlMXoBvIcBcrV9NQ==', N'qfLSbwaVHVh41AAZtwjAaa56Y+aZYrDZk5BOpOgCZgqoRa++TnF7ABi0FqSW2piglhGxSy7tvHcxrqD/jt8DSsfu2AfJT3DAQUt2SCNUT2ugwkj7pwg4ikM1D6AwqYSBKf3hhNRbzS35t6Ax6LCG1LYLCf0z97QTeAbQ+GQsdIA=')
INSERT [dbo].[Users] ([UserId], [UserName], [FirstName], [LastName], [PasswordHash], [PasswordSalt]) VALUES (N'a1e20287-5db0-4b55-a137-488256271ed8', N'user4', N'test4', N'user', N'smBq/0oUEnyG+vl/o3ryrvP4pxp7NEZp/V3eYTCK/y5k4vI9xkU8Ly3RLoPDBQXwIKZLM7IBWdCsVwqZk+9KKg==', N'tvXutJMGU7tbUWXiR4iFIemVe65qRvzX2Jcb3nMogact+TX6uP8b5ppvqIrdUSSBFLXF1vqgG2oMR0lBHCN4/Sjp66O9QpGeI/vp2lIEA1dawvVMfOgDBRbntDDxEMWZHnuLXKQxcX/6A6qkGy/ZY/bM9tdRpaDzp3V6/+6Oipg=')
INSERT [dbo].[Users] ([UserId], [UserName], [FirstName], [LastName], [PasswordHash], [PasswordSalt]) VALUES (N'e6b32c49-5d4e-4212-bfdf-ab9adb64a994', N'user5', N'test5', N'user', N'6O5j4xNuJ+6CLvZdVkMXUz2cwFf+Q3FRQXJgkZfn8aRRu7VQmnW1maXBt9GDtaKdPvLeeSyhnH1IEyfgD0u/nQ==', N'joPeJoAfQrvkpEbuG29PzFFTjjXtoejUKOVDynGEDVQa3jo52yLiEHe9XdVXnRCxXBszzJiSJeIwKNJdDVYnl/8Id/VJFiK4Lz2zd9yuMWqostBKo20+Ad2zEva9oOhqqc8dh4Fy150Lrl7bLL33Ei3V/1TIk6AuoCoK9Q/RVYM=')

INSERT [dbo].[UserBalances] ([UserBalanceId], [UserId], [Balance]) VALUES (N'4e55821a-db19-48ec-a69b-a5e970009844', N'4e55821a-db19-48ec-a69b-a5e970009844', 10000)
INSERT [dbo].[UserBalances] ([UserBalanceId], [UserId], [Balance]) VALUES (N'669293e1-1138-4f85-b32f-703271d8f216', N'669293e1-1138-4f85-b32f-703271d8f216', 10000)
INSERT [dbo].[UserBalances] ([UserBalanceId], [UserId], [Balance]) VALUES (N'64752bb6-cfd9-4966-9367-d7a79f09ed1d', N'64752bb6-cfd9-4966-9367-d7a79f09ed1d', 10000)
INSERT [dbo].[UserBalances] ([UserBalanceId], [UserId], [Balance]) VALUES (N'936928f5-7b8c-458c-b485-bf4b7cea91a9', N'936928f5-7b8c-458c-b485-bf4b7cea91a9', 10000)
INSERT [dbo].[UserBalances] ([UserBalanceId], [UserId], [Balance]) VALUES (N'a1e20287-5db0-4b55-a137-488256271ed8', N'a1e20287-5db0-4b55-a137-488256271ed8', 10000)
INSERT [dbo].[UserBalances] ([UserBalanceId], [UserId], [Balance]) VALUES (N'e6b32c49-5d4e-4212-bfdf-ab9adb64a994', N'e6b32c49-5d4e-4212-bfdf-ab9adb64a994', 10000)

INSERT [dbo].[Payments] ([PaymentId], [UserId], [Amount], [PayTo], [PaymentStatusId], [ClosedReason], [Date]) VALUES (N'8ffa5011-f38b-a621-e0ee-3793d7eaf67c', N'4e55821a-db19-48ec-a69b-a5e970009844', 15, N'669293e1-1138-4f85-b32f-703271d8f216', 1, N'Confirmed by recipient', '2021-05-06T02:43:16.267')
INSERT [dbo].[Payments] ([PaymentId], [UserId], [Amount], [PayTo], [PaymentStatusId], [ClosedReason], [Date]) VALUES (N'8ffa5022-f38b-a621-e0ee-3793d7eaf67c', N'4e55821a-db19-48ec-a69b-a5e970009844', 9, N'669293e1-1138-4f85-b32f-703271d8f216', 1, N'Confirmed by recipient', '2021-04-06T02:43:16.267')
INSERT [dbo].[Payments] ([PaymentId], [UserId], [Amount], [PayTo], [PaymentStatusId], [ClosedReason], [Date]) VALUES (N'8ffa5033-f38b-a621-e0ee-3793d7eaf67c', N'4e55821a-db19-48ec-a69b-a5e970009844', 400000, N'64752bb6-cfd9-4966-9367-d7a79f09ed1d', 2, N'Insufficient fund', '2021-04-20T02:43:16.267')