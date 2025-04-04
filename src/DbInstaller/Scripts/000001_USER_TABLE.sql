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