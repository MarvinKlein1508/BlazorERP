CREATE OR ALTER TRIGGER GEN_CUSTOMER_NUMBER
BEFORE INSERT ON CUSTOMERS
AS
BEGIN
    IF (NEW.CUSTOMER_NUMBER IS NULL OR NEW.CUSTOMER_NUMBER = '') THEN
    BEGIN
        NEW.CUSTOMER_NUMBER = (SELECT NEXT_CUSTOMER_NUMBER FROM GET_NEXT_CUSTOMER_NUMBER) ;
    END
END

