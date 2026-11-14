CREATE TABLE users (
    Id            SERIAL PRIMARY KEY,
    Username      VARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash TEXT         NOT NULL,
    CreatedAt    TIMESTAMP    NOT NULL DEFAULT NOW(),
    Token        TEXT         
);

CREATE TABLE media (
    id              SERIAL PRIMARY KEY,
    title           TEXT NOT NULL,
    description     TEXT,
    media_type      TEXT NOT NULL,
    release_year    INT NOT NULL,
    age_restriction INT NOT NULL DEFAULT 0,
    creator_id      INT NOT NULL REFERENCES users(id)
);

CREATE TABLE genres (
    id   SERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE media_genres (
    media_id INT NOT NULL REFERENCES media(id) ON DELETE CASCADE,
    genre_id INT NOT NULL REFERENCES genres(id),
    PRIMARY KEY (media_id, genre_id)
);