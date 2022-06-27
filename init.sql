/*--------DATABASE---------*/
CREATE DATABASE SyntellectDB
USE SyntellectDB
GO
/*--------TABLES---------*/
CREATE TABLE Employee
(
	Id int NOT NULL IDENTITY(1,1),
	FirstName varchar(50),
	LastName varchar(50),
	Patronymic varchar(50),
	BirthDate Date
)
ALTER TABLE Employee ADD CONSTRAINT PK_Employee PRIMARY KEY (Id)
GO
/*--------INDEXES---------*/
CREATE INDEX EmployeeFioIndex ON Employee (LastName,FirstName,Patronymic)
INCLUDE (BirthDate)
GO
/*--------TEST VALUES----------*/
INSERT INTO Employee (FirstName,LastName,Patronymic,BirthDate) 
VALUES
('TestName1','TestSurname1','TestPatronymic1','20010101'),
('TestName2','TestSurname2','TestPatronymic2','20020201'),
('TestName3','TestSurname3','TestPatronymic3','20030301'),
('TestName4','TestSurname4','TestPatronymic4','20040401'),
('Gleb','Ivanov','Alexandrovich','19990608')
GO