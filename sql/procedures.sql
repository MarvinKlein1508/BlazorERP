SET TERM ^ ;

CREATE OR ALTER PROCEDURE RESEED_IDENTITY
(
    tableName VARCHAR(255),
    identityColumnName VARCHAR(255)
)
AS
DECLARE VARIABLE maxValue INTEGER;
DECLARE VARIABLE generatorName VARCHAR(255);
BEGIN
    -- Step 1: Find the current maximum value of the identity column
    EXECUTE STATEMENT 'SELECT MAX(' || :identityColumnName || ') FROM ' || :tableName INTO :maxValue;

    -- Step 2: Find the generator name associated with the identity column
    FOR SELECT RDB$GENERATOR_NAME
        FROM RDB$RELATION_FIELDS
        WHERE RDB$RELATION_NAME = :tableName
        AND RDB$FIELD_NAME = :identityColumnName
        INTO :generatorName
    DO
    BEGIN
        -- Step 3: Reseed the generator with the new seed value
        EXECUTE STATEMENT 'SET GENERATOR ' || :generatorName || ' TO ' || :maxValue;
    END
END^

SET TERM ; ^
