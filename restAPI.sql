USE [CAR]
GO
/****** Object:  Table [dbo].[members]    Script Date: 12/7/2018 1:33:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[members](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](10) NULL,
	[name] [varchar](50) NULL,
	[first_nric] [char](1) NULL,
	[no_nric] [varchar](15) NULL,
	[last_nric] [char](1) NULL,
	[no_mobile] [varchar](50) NULL,
	[email] [varchar](100) NULL,
	[postal_code] [varchar](100) NULL,
	[promo_code] [varchar](100) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[uploads]    Script Date: 12/7/2018 1:33:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[uploads](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[member_id] [int] NOT NULL,
	[name] [varchar](100) NULL,
	[filepath] [varchar](100) NULL,
	[type] [varchar](10) NULL,
	[position] [varchar](10) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 12/7/2018 1:33:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
	[email] [varchar](100) NULL,
	[password] [varchar](100) NULL,
	[remember_token] [varchar](max) NULL,
	[is_active] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[members] ON 

INSERT [dbo].[members] ([id], [title], [name], [first_nric], [no_nric], [last_nric], [no_mobile], [email], [postal_code], [promo_code], [created_at], [updated_at]) VALUES (4, N'Mr', N'iqbal', N'T', N'1234567', N'A', N'658112245610', N'test@gmail.com', N'40192', N'CARFREE', CAST(N'2018-12-06T18:09:13.000' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[members] OFF
SET IDENTITY_INSERT [dbo].[users] ON 

INSERT [dbo].[users] ([id], [name], [email], [password], [remember_token], [is_active], [created_at], [updated_at]) VALUES (1, N'kiki', N'kiki@gmail.com', N'A075D17F3D453073853F813838C15B8023B8C487038436354FE599C3942E1F95', N'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJraWtpIjoia2lraUBnbWFpbC5jb20iLCIwIjoiMTIvNi8yMDE4IDY6MDk6MDcgUE0ifQ.iBydmGF6ZfklsmB3Ed5yVTGvTlGUh9RkUQp769v8ri4', 1, CAST(N'2018-12-06T11:41:34.000' AS DateTime), CAST(N'2018-12-06T18:09:07.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[users] OFF
