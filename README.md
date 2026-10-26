SELECT TOP 10
    T.VERSION_HASH_CODE,

    B.HashBytes,

    DATALENGTH(B.HashBytes) AS ByteLength,

    CAST('' AS XML).value(
        'xs:base64Binary(sql:column("B.HashBytes"))',
        'VARCHAR(100)'
    ) AS VersionHashCodeBase64

FROM YourTable T

CROSS APPLY
(
    SELECT TRY_CONVERT(
        VARBINARY(16),
        T.VERSION_HASH_CODE,
        2
    ) AS HashBytes
) B

WHERE T.VERSION_HASH_CODE IS NOT NULL
  AND DATALENGTH(T.VERSION_HASH_CODE) = 32
  AND B.HashBytes IS NOT NULL;
