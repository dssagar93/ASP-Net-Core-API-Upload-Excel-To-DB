/****** Object:  Database [BrandChannel]    Script Date: 06-07-2020 23:28:56 ******/

CREATE DATABASE [BrandChannel]
 
USE [BrandChannel]
GO
CREATE TYPE [dbo].[UDT_BrandChannels] AS TABLE(
	[Id] [bigint] NULL,
	[Brand] [nvarchar](max) NULL,
	[YY_Year] [bigint] NOT NULL,
	[MM_Month] [bigint] NOT NULL,
	[DocNo] [nvarchar](max) NULL,
	[Channel] [nvarchar](max) NULL,
	[TimeBandStart] [nvarchar](max) NULL,
	[TimeBandEnd] [nvarchar](max) NULL,
	[Amount] [float] NOT NULL,
	[ActivityDate] [datetime2](7) NOT NULL,
	[MWUID] [nvarchar](max) NULL,
	[IsMonitored] [bit] NOT NULL,
	[IsDisputed] [bit] NOT NULL,
	[ImpactBase] [nvarchar](max) NULL
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BrandChannels](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Brand] [nvarchar](max) NULL,
	[YY_Year] [bigint] NOT NULL,
	[MM_Month] [bigint] NOT NULL,
	[DocNo] [nvarchar](max) NULL,
	[Channel] [nvarchar](max) NULL,
	[TimeBandStart] [nvarchar](max) NULL,
	[TimeBandEnd] [nvarchar](max) NULL,
	[Amount] [float] NOT NULL,
	[ActivityDate] [datetime2](7) NOT NULL,
	[MWUID] [nvarchar](450) NOT NULL,
	[IsMonitored] [bit] NOT NULL,
	[IsDisputed] [bit] NOT NULL,
	[ImpactBase] [nvarchar](max) NULL,
 CONSTRAINT [PK_BrandChannels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_MWUID] UNIQUE NONCLUSTERED 
(
	[MWUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogDynSqlQry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SqlQry] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_brandchannel_mwuid]    Script Date: 06-07-2020 23:28:56 ******/


/****** Creating index for faster access ******/

CREATE NONCLUSTERED INDEX [idx_brandchannel_mwuid] ON [dbo].[BrandChannels]
(
	[MWUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  StoredProcedure [dbo].[SP_BulkInsertBrandChannel]    Script Date: 06-07-2020 23:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_BulkInsertBrandChannel]
@brandchannel UDT_BrandChannels READONLY
AS
BEGIN
    INSERT INTO BrandChannels (Brand, YY_Year, MM_Month, DocNo, Channel, TimeBandStart, TimeBandEnd, Amount, ActivityDate, MWUID, IsMonitored, IsDisputed, ImpactBase)
    SELECT Brand,
           YY_Year,
           MM_Month,
           DocNo,
           Channel,
           TimeBandStart,
           TimeBandEnd,
           Amount,
           ActivityDate,
           MWUID,
           IsMonitored,
           IsDisputed,
           ImpactBase
    FROM   @brandchannel;
END

GO


/****** Object:  StoredProcedure [dbo].[SP_FilterBrandChannel]    Script Date: 06-07-2020 23:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_FilterBrandChannel]
@FilterQuery NVARCHAR (MAX)="brand = 'Brand 3'"
AS
BEGIN
    DECLARE @Query AS NVARCHAR (2000);
    SET @Query = 'SELECT Brand, DocNo, Channel, COUNT(*) AS NoOfSpots, Round(SUM(Amount),0) As TotalAmount, 
	
	
	Round(SUM(CASE When TimeBandStart>=''19:00:00'' AND TimeBandEnd<=''23:00:00'' THEN Amount ELSE 0 END),0) AS PrimeTimeAmount,


	Round(SUM(CASE WHEN ImpactBase=''Impact'' THEN AMOUNT ELSE 0 END),0) AS ImpactSum,

	Round(SUM(CASE WHEN IsDisputed=1 THEN AMOUNT ELSE 0 END),0) AS DisputedSum
	
	FROM BrandChannels WHERE ' + @FilterQuery + ' GROUP BY DocNo, Channel, Brand';
    INSERT  INTO LogDynSQlQry
    VALUES (@Query, GETDATE());
    PRINT @Query;
    EXECUTE sp_executesql @Query;
END

GO

ALTER DATABASE [BrandChannel] SET  READ_WRITE 
GO
