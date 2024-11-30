drop table dbo.users;
drop table dbo.tokens;

CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    login NVARCHAR(100) NOT NULL,
    password NVARCHAR(255) NOT NULL,
	isActive bit not null
);


CREATE TABLE tokens (
    id INT IDENTITY(1,1) PRIMARY KEY,
    userId INT NOT NULL,
	token NVARCHAR(1024) NOT NULL,
	createdAt DATETIME NOT NULL,
	expiresAt DATETIME NOT NULL,
	isRevoked bit not null
);

CREATE TABLE roles (
	id INT IDENTITY(1,1) PRIMARY KEY,
	name VARCHAR(255) NOT NULL
);

CREATE TABLE userRoles (
	userId INT,
	roleId INT
);

insert into userRoles values (1, 2)

select r.name from roles r
join userRoles ur on ur.roleId = r.id
where ur.userId = 1;

select * from userRoles;
select * from roles;


select * from dbo.Users;
select * from dbo.Tokens;

delete from dbo.Users;
delete from dbo.UserRoles;

update dbo.tokens 
set isRevoked = 1 
where Id = 1;