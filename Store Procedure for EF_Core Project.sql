use EventMgmtDB
go
-- 1. Create the Table Type for the Detail records
-- This allows us to pass multiple registrations as a single parameter
CREATE TYPE dbo.RegistrationTableType AS TABLE (
    RegistrationId INT, -- Needed for Update/Delete logic
    EventId INT,
    TicketCount INT,
    RegistrationDate DATE,
    TotalPaid MONEY
);
GO

-- 2. Create the All-in-One Master-Detail Procedure
CREATE PROCEDURE sp_ManageAttendeeRegistrations
    @Action NVARCHAR(10),
    @AttendeeId INT = 0,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(50) = NULL,
    @PhoneNumber NVARCHAR(15) = NULL,
    @Image NVARCHAR(250) = NULL,
    @IsAdult BIT = NULL,
    @Registrations dbo.RegistrationTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF (@Action = 'INSERT')
        BEGIN
            INSERT INTO Attendees (FullName, Email, PhoneNumber, Image, IsAdult)
            VALUES (@FullName, @Email, @PhoneNumber, @Image, @IsAdult);

            DECLARE @NewAttendeeId INT = SCOPE_IDENTITY();

            INSERT INTO Registrations (AttendeeId, EventId, TicketCount, RegistrationDate, TotalPaid)
            SELECT @NewAttendeeId, EventId, TicketCount, RegistrationDate, TotalPaid
            FROM @Registrations;
        END
        ELSE IF (@Action = 'UPDATE')
        BEGIN
            UPDATE Attendees 
            SET FullName = @FullName,
                Email = @Email,
                PhoneNumber = @PhoneNumber,
                Image = @Image,
                IsAdult = @IsAdult
            WHERE AttendeeId = @AttendeeId;

            DELETE FROM Registrations WHERE AttendeeId = @AttendeeId;

            INSERT INTO Registrations (AttendeeId, EventId, TicketCount, RegistrationDate, TotalPaid)
            SELECT @AttendeeId, EventId, TicketCount, RegistrationDate, TotalPaid
            FROM @Registrations;
        END
        ELSE IF (@Action = 'DELETE')
        BEGIN
            DELETE FROM Registrations WHERE AttendeeId = @AttendeeId;
            DELETE FROM Attendees WHERE AttendeeId = @AttendeeId;
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;

        -- Error bubble up (debug friendly)
        THROW;
    END CATCH
END
GO