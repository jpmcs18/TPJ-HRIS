USE [TPJ]
GO
/****** Object:  StoredProcedure [kiosk].[CreateOrUpdateOuterPortRequest]    Script Date: 6/13/2020 11:42:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [kiosk].[CreateOrUpdateOuterPortRequest] (
	@ID BIGINT OUT,
	@PersonnelID BIGINT,
	@StartDate DATE,
	@EndDate DATE,
	@Purpose NVARCHAR(200),
	@LocationID TINYINT,
	@LogBy INT
)
AS
BEGIN
	IF @ID IS NULL OR @ID = 0
	BEGIN
		INSERT INTO kiosk.[Outer Port Request](
			[Personnel ID]
			, [Start Date]
			, [End Date]
			, Purpose
			, [Location ID]
			, [Created By])
		VALUES(
			@PersonnelID
			, @StartDate
			, @EndDate
			, @Purpose
			, @LocationID
			, @LogBy
		)

		SET @ID = SCOPE_IDENTITY();
	END	
	ELSE
	BEGIN
		UPDATE kiosk.[Outer Port Request]
		SET [Start Date] = @StartDate
			, [End Date] = @EndDate
			, [Purpose] = @Purpose
			, [Location ID] = @LocationID
			, [Modified By] = @LogBy
			, [Modified On] = GETDATE()
		WHERE ID = @ID 
	END
END
