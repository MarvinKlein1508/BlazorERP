CREATE TABLE users 
(
    user_id SERIAL,
    username VARCHAR(50) NOT NULL,
    first_name VARCHAR(50) NOT NULL DEFAULT '',
    last_name VARCHAR(50) NOT NULL DEFAULT '',
    active_directory_guid UUID,
    email VARCHAR(255),
    password VARCHAR(255),
    salt VARCHAR(255),
    is_active BOOLEAN NOT NULL,
    account_type INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT pk_users PRIMARY KEY (user_id),
    CONSTRAINT uq_users_account_type UNIQUE (username, account_type)
);

INSERT INTO users (user_id, username, first_name, password, salt, is_active, account_type) VALUES (1, 'admin', 'Administrator', 'AQAAAAIAAYagAAAAEIfctTCf+6YIelDIQ0niS7nJGnwd1wfSBNkJR30I90o7oO/wJc8Db3unBO1y9xg7Og==', 'jOuXx+GZvNaloIQBUXU7Dur+RdKOBLOHmEpNborcUZw=', true, 0);