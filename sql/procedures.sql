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

SET TERM ^ ;

CREATE OR ALTER PROCEDURE GET_NEXT_KUNDENNUMMER()
RETURNS 
(
    NEXT_KUNDENNUMMER INTEGER
)
AS
DECLARE VARIABLE MIN_KUNDE INTEGER;
DECLARE VARIABLE MAX_KUNDE INTEGER;
DECLARE VARIABLE CURRENT_KUNDE INTEGER;
DECLARE VARIABLE EXISTS_KUNDE INTEGER;
BEGIN
	 -- Initialize variables
    MIN_KUNDE = 0;
    MAX_KUNDE = 0;
    CURRENT_KUNDE = 0;
    EXISTS_KUNDE = 0;
   
   	-- Find the range for KUNDENNUMMER from NUMMERNKREISE table
    SELECT KUNDE_VON, KUNDE_BIS
    FROM NUMMERNKREISE
    INTO :MIN_KUNDE, :MAX_KUNDE;

    IF (MIN_KUNDE = 0 OR MAX_KUNDE = 0) THEN
    BEGIN
        -- Return error if range is not defined
        EXCEPTION NO_NUMBERS_AVAILABLE;
        SUSPEND;
    END
    
     -- Find the next available KUNDENNUMMER within the range
    FOR SELECT KUNDENNUMMER
    FROM KUNDEN
    WHERE KUNDENNUMMER BETWEEN :MIN_KUNDE AND :MAX_KUNDE
    INTO :CURRENT_KUNDE
    DO
    BEGIN
        EXISTS_KUNDE = 1;
    END
   
    IF (EXISTS_KUNDE = 0) THEN
    BEGIN
        -- If no KUNDENNUMMER exists within the range, return the minimum number
        NEXT_KUNDENNUMMER = MIN_KUNDE;
        SUSPEND;
    END
    ELSE
    BEGIN
    
        -- Otherwise, find the next available number
        NEXT_KUNDENNUMMER = (
                                SELECT COALESCE(MIN(NUMMER), -1) AS NUMMER FROM 
                                (
                                    WITH RECURSIVE NUMBERS(NUMMER) AS 
                                    (
                                        SELECT :MIN_KUNDE AS NUMMER FROM RDB$DATABASE -- Anfangswert
                                        UNION ALL
                                        SELECT NUMMER + 1 FROM NUMBERS WHERE NUMMER < :MAX_KUNDE -- Endwert
                                    )
                                    SELECT FIRST 1 N.NUMMER 
                                    FROM NUMBERS N
                                    LEFT JOIN KUNDEN K ON N.NUMMER = K.KUNDENNUMMER
                                    WHERE K.KUNDENNUMMER IS NULL
                                )
                            );

        IF (NEXT_KUNDENNUMMER = -1) THEN
        BEGIN
            EXCEPTION NO_NUMBERS_AVAILABLE;
        END

        SUSPEND;
    END
  

END^

SET TERM ; ^
