# BinaryFormatter

SELECT
    'IF NOT EXISTS
(
    SELECT 1
    FROM ATM.ATM_GROUPS
    WHERE GROUP_CODE = ''73889''
      AND TERMINAL_ID = ''' + REPLACE(TERMINAL_ID, '''', '''''') + '''
)
BEGIN
    INSERT INTO ATM.ATM_GROUPS
    (
        LOG_SERIAL_NUMBER,
        GROUP_CODE,
        TERMINAL_ID,
        UPDATING_CHANNEL_CODE,
        UPDATING_TRAN_CODE,
        UPDATING_USER_CODE,
        UPDATE_DATE,
        RECORD_STATUS
    )
    VALUES
    (
        0,
        ''73889'',
        ''' + REPLACE(TERMINAL_ID, '''', '''''') + ''',
        ''DB.BULK_UPDATE'',
        ''MNDSUP'',
        ''ALICANYI'',
        GETDATE(),
        ''A''
    );
END;
'
AS INSERT_SCRIPT
FROM ATM.ATM_TBL WITH (NOLOCK)
ORDER BY TERMINAL_ID;
