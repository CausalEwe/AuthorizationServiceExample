CREATE OR ALTER PROCEDURE AddUser
    @login NVARCHAR(50),
    @password NVARCHAR(50),
    @isActive BIT,
    @newUserId INT OUTPUT
AS
BEGIN
    INSERT INTO users (login, password, isActive)
    VALUES (@login, @password, @isActive);

    SET @newUserId = SCOPE_IDENTITY();
END;

go

CREATE OR ALTER PROCEDURE RevokeToken
    @userId INT
AS
BEGIN
    UPDATE dbo.tokens
    SET isRevoked = 1
    WHERE userId = @userId AND isRevoked = 0;
END;

go

CREATE OR ALTER PROCEDURE AddToken
    @userId INT,
    @token NVARCHAR(MAX),
    @createdAt DATETIME,
    @expiresAt DATETIME,
    @isRevoked BIT
AS
BEGIN
    INSERT INTO dbo.tokens (userId, token, createdAt, expiresAt, isRevoked)
    VALUES (@userId, @token, @createdAt, @expiresAt, @isRevoked);
END;

go

CREATE OR ALTER PROCEDURE GetShortUsers
AS
BEGIN
    SELECT id, login, isActive
    FROM users;
END;

go

CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    login NVARCHAR(100) NOT NULL,
    password NVARCHAR(255) NOT NULL,
	isActive bit not null
);

go

CREATE TABLE tokens (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT NOT NULL,
	token NVARCHAR(1024) NOT NULL,
	createdAt DATETIME NOT NULL,
	expiresAt DATETIME NOT NULL,
	isRevoked bit not null
);

go

CREATE TABLE roles (
	id INT IDENTITY(1,1) PRIMARY KEY,
	name VARCHAR(255) NOT NULL
);

go

CREATE TABLE userRoles (
	userId INT,
	roleId INT
);

go

insert into users values ('admin', '$2a$11$xZu9GDuzD5or02X6/.3eFerJWtC86FluDQsf3zTxyBlrihvsMVo.G', 1);
insert into users values ('andrey', '$2a$11$xZu9GDuzD5or02X6/.3eFerJWtC86FluDQsf3zTxyBlrihvsMVo.G', 0);
insert into users values ('ilya', '$2a$11$xZu9GDuzD5or02X6/.3eFerJWtC86FluDQsf3zTxyBlrihvsMVo.G', 1);
insert into users values ('pasha', '$2a$11$xZu9GDuzD5or02X6/.3eFerJWtC86FluDQsf3zTxyBlrihvsMVo.G', 0);
insert into users values ('sveta', '$2a$11$xZu9GDuzD5or02X6/.3eFerJWtC86FluDQsf3zTxyBlrihvsMVo.G', 1);
insert into users values ('dmitriy', '$2a$11$xZu9GDuzD5or02X6/.3eFerJWtC86FluDQsf3zTxyBlrihvsMVo.G', 1);

insert into roles values ('Admin');
insert into roles values ('User');

insert into userRoles values (1, 1);
insert into userRoles values (1, 2);
insert into userRoles values (2, 2);
insert into userRoles values (3, 2);
insert into userRoles values (4, 2);
insert into userRoles values (5, 2);
insert into userRoles values (6, 2);

