CREATE DATABASE IF NOT EXISTS SolarEdge;
CREATE SCHEMA IF NOT EXISTS SolarEdge;

CREATE TABLE IF NOT EXISTS `SolarEdge`.`EnergyDetails`(
	`id` INT NOT NULL AUTO_INCREMENT,
	`time` DATETIME,
	`SelfConsumption` DOUBLE NOT NULL,
	`Consumption` DOUBLE NOT NULL,
	`Purchased` DOUBLE NOT NULL,
	`Production` DOUBLE NOT NULL,
	`FeedIn` DOUBLE NOT NULL,
	PRIMARY KEY ( `id` ) );

CREATE TABLE IF NOT EXISTS `SolarEdge`.`Overview`(
	`id` INT NOT NULL AUTO_INCREMENT,
	`time` DATETIME,
	`metric` VARCHAR(100) NOT NULL,
	`LifeTimeData` DOUBLE NOT NULL,
	`LastYearData` DOUBLE NOT NULL,
	`LastMonthData` DOUBLE NOT NULL,
	`LastDayData` DOUBLE NOT NULL,
	`CurrentPower` DOUBLE NOT NULL,
	PRIMARY KEY ( `id` ) );
