USE [TPJ PROD]

SELECT TOP 1 * FROM [System].Errors
ORDER BY ID DESC

--DELETE FROM kiosk.[Leave Request]

SELECT * FROM kiosk.[Leave Request]

UPDATE lookup.[Leave Type]
SET [CNB Note First] = 1
WHERE ID IN (3)

SELECT * FROM lookup.[Leave Type]

exec hr.GetLeaveType 3