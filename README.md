DECLARE @ResponseMessage VARCHAR(MAX);

SELECT @ResponseMessage = RESPONSE_MESSAGE
FROM COR.INTEGRATION_MESSAGE_LOG
WHERE ID = 123; -- Kendi kayıt ID'n

IF ISJSON(@ResponseMessage) = 1
BEGIN
    SELECT COUNT(*) AS TotalRecordCount
    FROM OPENJSON(@ResponseMessage, '$.Data');
END
ELSE
BEGIN
    SELECT 'JSON geçersiz veya kesilmiş.' AS ErrorMessage;
END
