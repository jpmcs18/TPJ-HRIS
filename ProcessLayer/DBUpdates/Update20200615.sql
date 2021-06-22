
USE [TPJ]
GO

ALTER PROC [kiosk].[GetApprovedOuterPortRequest](
	@PersonnelID BIGINT,
	@LocationID TINYINT,
	@StartDate DATE,
	@EndDate DATE
)
AS
BEGIN
	SELECT * 
	FROM kiosk.[Outer Port Request]
	WHERE [Personnel ID] = @PersonnelID
		AND (@LocationID IS NULL OR [Location ID] = @LocationID)
		AND (([Start Date] BETWEEN @StartDate AND @EndDate)
			OR ([End Date] BETWEEN @StartDate AND @EndDate))
		AND (Cancelled = 0 OR Cancelled IS NULL)
END
GO