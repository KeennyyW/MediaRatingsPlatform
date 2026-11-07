CREATE TABLE users (
    Id            SERIAL PRIMARY KEY,
    Username      VARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash TEXT         NOT NULL,
    CreatedAt    TIMESTAMP    NOT NULL DEFAULT NOW()
);