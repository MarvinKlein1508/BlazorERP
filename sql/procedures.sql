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

CREATE OR ALTER PROCEDURE GET_NEXT_CUSTOMER_NUMBER()
RETURNS 
(
    NEXT_CUSTOMER_NUMBER INTEGER
)
AS
DECLARE VARIABLE MIN_CUSTOMER INTEGER;
DECLARE VARIABLE MAX_CUSTOMER INTEGER;
DECLARE VARIABLE CURRENT_CUSTOMER INTEGER;
DECLARE VARIABLE EXISTS_CUSTOMER INTEGER;
BEGIN
	 -- Initialize variables
    MIN_CUSTOMER = 0;
    MAX_CUSTOMER = 0;
    CURRENT_CUSTOMER = 0;
    EXISTS_CUSTOMER = 0;
   
   	-- Find the range for CUSTOMER_NUMBER from NUMMERNKREISE table
    SELECT CUSTOMER_FROM, CUSTOMER_TO
    FROM NUMBER_RANGES
    INTO :MIN_CUSTOMER, :MAX_CUSTOMER;

    IF (MIN_CUSTOMER = 0 OR MAX_CUSTOMER = 0) THEN
    BEGIN
        -- Return error if range is not defined
        EXCEPTION NO_NUMBERS_AVAILABLE;
        SUSPEND;
    END
    
     -- Find the next available CUSTOMER_NUMBER within the range
    FOR SELECT CUSTOMER_NUMBER
    FROM CUSTOMERS
    WHERE CUSTOMER_NUMBER BETWEEN :MIN_CUSTOMER AND :MAX_CUSTOMER
    INTO :CURRENT_CUSTOMER
    DO
    BEGIN
        EXISTS_CUSTOMER = 1;
    END
   
    IF (EXISTS_CUSTOMER = 0) THEN
    BEGIN
        -- If no CUSTOMER_NUMBER exists within the range, return the minimum number
        NEXT_CUSTOMER_NUMBER = MIN_CUSTOMER;
        SUSPEND;
    END
    ELSE
    BEGIN
    
        -- Otherwise, find the next available number
        NEXT_CUSTOMER_NUMBER = 
                            (
                                SELECT COALESCE(MIN(NUMBER), -1) AS NUMBER FROM 
                                (
                                    WITH RECURSIVE NUMBERS(NUMBER) AS 
                                    (
                                        SELECT :MIN_CUSTOMER AS NUMBER FROM RDB$DATABASE -- Initial value
                                        UNION ALL
                                        SELECT NUMBER + 1 FROM NUMBERS WHERE NUMBER < :MAX_CUSTOMER -- End value
                                    )
                                    SELECT FIRST 1 N.NUMBER 
                                    FROM NUMBERS N
                                    LEFT JOIN CUSTOMERS C ON N.NUMBER = C.CUSTOMER_NUMBER
                                    WHERE C.CUSTOMER_NUMBER IS NULL
                                )
                            );

        IF (NEXT_CUSTOMER_NUMBER = -1) THEN
        BEGIN
            EXCEPTION NO_NUMBERS_AVAILABLE;
        END

        SUSPEND;
    END
END^

SET TERM ; ^

