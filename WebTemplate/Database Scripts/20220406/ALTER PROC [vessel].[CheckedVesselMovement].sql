USE [TPJ]
GO
/****** Object:  StoredProcedure [vessel].[CheckedVesselMovement]    Script Date: 04/06/22 4:50:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [vessel].[CheckedVesselMovement](
	@ID BIGINT, 
	@LogBy BIGINT
)
AS
BEGIN
	UPDATE vessel.[Vessel Movement]
	SET [Movement Status ID] = 3, 
	[Checked By] = @LogBy, 
	[Checked Date] = GETDATE()
	WHERE ID = @ID
END
