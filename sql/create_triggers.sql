CREATE TRIGGER trg_UpdateTimestamp
ON [User]
AFTER UPDATE AS
BEGIN
    UPDATE [User]
    SET UpdatedAt = GETDATE()
    FROM [User] INNER JOIN inserted ON [User].UserId = inserted.UserId;
END;
