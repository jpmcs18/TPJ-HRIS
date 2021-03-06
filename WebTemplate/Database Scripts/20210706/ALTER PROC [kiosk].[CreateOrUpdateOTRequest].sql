USE [TPJ PROD LATEST]
GO
/****** Object:  StoredProcedure [kiosk].[CreateOrUpdateOTRequest]    Script Date: 07/05/21 11:43:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [kiosk].[CreateOrUpdateOTRequest](
	@ID BIGINT OUT
	, @PersonnelID BIGINT
	, @RequestDate DATE
	, @Reasons nvarchar(2000)
	, @IsOffice BIT
	, @IsEarlyOT BIT
	, @OTType INT
	, @StartDateTime DATETIME
	, @EndDateTime DATETIME
	, @LogBy INT)
AS
BEGIN
	DECLARE @msg NVARCHAR(MAX);

	IF @PersonnelID IS NULL OR @PersonnelID = 0
	BEGIN
		IF @msg IS NOT NULL
			SET @msg = CONCAT(@msg, '<br/>');
		SET @msg = CONCAT(@msg, '- Requestor cannot be null.');
	END
	

	IF @RequestDate IS NULL
	BEGIN
		IF @msg IS NOT NULL
			SET @msg = CONCAT(@msg, '<br/>');
		SET @msg = CONCAT(@msg, '- Request Date cannot be null.');
	END

	IF @msg IS NOT NULL
		RAISERROR(@msg, 16, 2)
	ELSE
	BEGIN
		IF @ID IS NULL OR @ID = 0
		BEGIN
			INSERT INTO kiosk.[OT Request](
				[Personnel ID], 
				[Request Date], 
				[Reasons], 
				[Is Office],
				[Is Early OT],
				[OT Type],
				[Start Date Time],
				[End Date Time],
				[Created By], 
				[Created On]
			)
			VALUES(
				@PersonnelID, 
				@RequestDate, 
				@Reasons, 
				@IsOffice,
				@IsEarlyOT,
				@OTType,
				@StartDateTime,
				@EndDateTime,
				@LogBy, 
				GETDATE()
			);

			SET @ID = SCOPE_IDENTITY();
		END
		ELSE
		BEGIN
			UPDATE kiosk.[OT Request]
			SET [Request Date] = @RequestDate
				, [Reasons] = @Reasons
				, [Is Office] = @IsOffice
				, [Is Early OT] = @IsEarlyOT
				, [OT Type] = @OTType
				, [Start Date Time] = @StartDateTime
				, [End Date Time] = @EndDateTime
				, [Modified By] = @LogBy
				, [Modified On] = GETDATE()
			WHERE ID = @ID;
		END
	END
END
