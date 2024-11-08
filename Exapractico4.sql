create database Exapractico4;

use Exapractico4;

create table registro(
Id_usuario int(14) auto_increment Primary key,
Nom_usuario Varchar(45) not null,
dia int(2),
mes int(2),
ano int(2)
);

create table tem(
Id_tem int (10) auto_increment Primary key,
Tem_detetada float ,
Estado_servo Varchar(20)
);

select *from tem;