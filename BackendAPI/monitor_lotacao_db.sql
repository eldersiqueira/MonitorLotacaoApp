CREATE DATABASE IF NOT EXISTS monitor_lotacao_db;
USE monitor_lotacao_db;

CREATE TABLE IF NOT EXISTS relatos (
    idRelato INT AUTO_INCREMENT PRIMARY KEY,
    codigoLinha VARCHAR(10) NOT NULL,
    nivelLotacao VARCHAR(50) NOT NULL,
    horario DATETIME NOT NULL,
    status VARCHAR(50) NOT NULL
);