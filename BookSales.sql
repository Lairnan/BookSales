use [master];
if not exists (select * from [sys].[databases] where [name] = 'BookSales' collate sql_latin1_general_cp1_cs_as)
begin
create database [BookSales];
end
go
use [BookSales];
go
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Authors' collate sql_latin1_general_cp1_cs_as)
begin
create table [Authors] (
    id int primary key identity(1,1),
    surname nvarchar(30) not null,
    [name] nvarchar(30) not null,
    patronymic nvarchar(30) null
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Genres' collate sql_latin1_general_cp1_cs_as)
begin
create table [Genres] (
    id int primary key identity(1,1),
    [name] nvarchar(30) not null unique
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Publishers' collate sql_latin1_general_cp1_cs_as)
begin
create table [Publishers] (
    id int primary key identity(1,1),
    [name] nvarchar(30) not null unique
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Books' collate sql_latin1_general_cp1_cs_as)
begin
create table [Books] (
    id int primary key identity(1,1),
    [name] nvarchar(100) not null,
    authorId int not null,
    genreId int not null,
    publisherId int not null,
    [pages] int not null,
    [releaseDate] date not null,
    [retailPrice] decimal(18,2) not null,
	[image] image null,
    foreign key (authorId) references [Authors](id) on delete cascade on update cascade,
    foreign key (genreId) references [Genres](id) on delete cascade on update cascade,
    foreign key (publisherId) references [Publishers](id) on delete cascade on update cascade
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Storage' collate sql_latin1_general_cp1_cs_as)
begin
create table [Storage] (
    id int primary key identity(1,1),
	[name] nvarchar(100) not null unique,
	[address] nvarchar(75) not null
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'PlaceHolder' collate sql_latin1_general_cp1_cs_as)
begin
create table [PlaceHolder] (
    idBook int not null primary key,
	idStorage int not null,
	stock int not null check(stock >= 0),
	foreign key (idBook) references [Books](id) on delete cascade on update cascade,
	foreign key (idStorage) references [Storage](id) on delete cascade on update cascade
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Positions' collate sql_latin1_general_cp1_cs_as)
begin
create table Positions (
    id int primary key identity(1,1),
    [name] nvarchar(30) not null unique
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Users' collate sql_latin1_general_cp1_cs_as)
begin
create table [Users] (
    id int primary key identity(1,1),
    surname nvarchar(30) not null,
    [name] nvarchar(30) not null,
    patronymic nvarchar(30) null,
    [dateOfBirth] date not null,
    [login] nvarchar(50) not null unique,
    [password] nvarchar(50) not null,
    positionId int not null,
	[image] image null,
	dateOfStart datetime not null,
    foreign key (positionId) references Positions (id) on delete cascade on update cascade
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'Orders' collate sql_latin1_general_cp1_cs_as)
begin
create table [Orders] (
    id int primary key identity(1,1),
    idUser int not null,
    dateOrder datetime not null,
    price decimal(18,2) not null,
	paid bit,
	performed bit,
	dateSuccess datetime null,
    foreign key (idUser) references [Users](id) on delete cascade on update cascade
);
end
if not exists (select * from [BookSales].[sys].[all_objects] where [name] = 'OrderConsist' collate sql_latin1_general_cp1_cs_as)
begin
create table [OrderConsist] (
	id int primary key identity,
	idOrder int not null,
    idBook int not null,
    amount int not null,
    foreign key (idOrder) references [Orders](id) on delete cascade on update cascade,
    foreign key (idBook) references [Books](id) on delete cascade on update cascade
);
end
insert into [Positions] (name) VALUES (N'Клиент');
insert into [Positions] (name) VALUES (N'Менеджер');
insert into [Positions] (name) VALUES (N'Администратор');
go
use [master]