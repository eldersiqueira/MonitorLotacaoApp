-- Criação da Base de Dados oficial do TCC
CREATE DATABASE IF NOT EXISTS monitor_lotacao_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE monitor_lotacao_db;

-- --------------------------------------------------------
-- 1. Tabela de Passageiros e Administradores (Unificada com controle de perfil)
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS passageiros (
    idPassageiro INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    tipo_usuario INT DEFAULT 0 -- 0 = Passageiro Comum, 1 = Administrador (Gestão)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 2. Tabela de Linhas de Ônibus (As 5 linhas do escopo do projeto)
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS linhas_onibus (
    codigoLinha VARCHAR(10) PRIMARY KEY,
    nomeLinha VARCHAR(100) NOT NULL,
    trajeto TEXT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 3. Tabela de Relatos Colaborativos de Lotação
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS relatos (
    idRelato INT AUTO_INCREMENT PRIMARY KEY,
    codigoLinha VARCHAR(10) NOT NULL,
    horario DATETIME NOT NULL,
    nivelLotacao VARCHAR(50) NOT NULL, -- Ex: Vazio, Moderado, Lotado
    status VARCHAR(50) DEFAULT 'Ativo',
    CONSTRAINT fk_relato_linha FOREIGN KEY (codigoLinha) REFERENCES linhas_onibus(codigoLinha)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 4. Inserção de Dados Iniciais (Seeders)
-- --------------------------------------------------------

-- Inserção das 5 linhas de ônibus monitoradas (Escopo da Prova de Conceito)
INSERT INTO linhas_onibus (codigoLinha, nomeLinha, trajeto) VALUES 
('01', 'Linha 01 - Centro / Bairro', 'Centro - Av. Principal - Bairro'),
('02', 'Linha 02 - Circular Universitária', 'Terminal - Campus - Estação'),
('03', 'Linha 03 - Industrial', 'Garagem - Distrito Industrial - Centro'),
('04', 'Linha 04 - Norte / Sul', 'Terminal Norte - Shopping - Terminal Sul'),
('05', 'Linha 05 - Expressa', 'Expressa - Rota Direta - Centro'),
('06', 'Linha 06 - Periférica', 'Bairro Leste - Centro - Bairro Oeste');

-- Inserção de Utilizadores de Teste (Comum e Administrador)
INSERT INTO passageiros (nome, email, senha, tipo_usuario) VALUES 
('Administrador Sistema', 'admin@monitor.com', 'admin123', 1),
('Passageiro Teste', 'passageiro@monitor.com', '123456', 0);

-- Inserção da Massa de Dados Histórica (Relatos entre 08/09/2026 e 19/09/2026 para a US 5.1)
INSERT INTO relatos (codigoLinha, horario, nivelLotacao, status) VALUES 
-- Dia 08/09/2026
('01', '2026-09-08 07:30:00', 'Lotado', 'Ativo'),
('01', '2026-09-08 18:10:00', 'Lotado', 'Ativo'),
('02', '2026-09-08 08:00:00', 'Moderado', 'Ativo'),
('03', '2026-09-08 12:30:00', 'Vazio', 'Ativo'),

-- Dia 09/09/2026
('01', '2026-09-09 07:40:00', 'Lotado', 'Ativo'),
('04', '2026-09-09 17:50:00', 'Lotado', 'Ativo'),
('05', '2026-09-09 13:15:00', 'Moderado', 'Ativo'),

-- Dia 10/09/2026
('02', '2026-09-10 07:20:00', 'Lotado', 'Ativo'),
('03', '2026-09-10 18:30:00', 'Moderado', 'Ativo'),
('06', '2026-09-10 08:15:00', 'Lotado', 'Ativo'),

-- Dia 11/09/2026
('01', '2026-09-11 07:30:00', 'Lotado', 'Ativo'),
('04', '2026-09-11 12:00:00', 'Vazio', 'Ativo'),
('06', '2026-09-11 17:45:00', 'Lotado', 'Ativo'),

-- Dia 12/09/2026 (Fim de semana)
('03', '2026-09-12 10:00:00', 'Vazio', 'Ativo'),
('05', '2026-09-12 14:30:00', 'Moderado', 'Ativo

-- Dia 15/09/2026
('01', '2026-09-15 07:25:00', 'Lotado', 'Ativo'),
('02', '2026-09-15 08:05:00', 'Lotado', 'Ativo'),
('06', '2026-09-15 18:00:00', 'Lotado', 'Ativo'),

-- Dia 16/09/2026
('03', '2026-09-16 07:50:00', 'Moderado', 'Ativo'),
('04', '2026-09-16 17:30:00', 'Lotado', 'Ativo'),
('05', '2026-09-16 12:10:00', 'Vazio', 'Ativo'),

-- Dia 17/09/2026
('01', '2026-09-17 07:35:00', 'Lotado', 'Ativo'),
('02', '2026-09-17 18:20:00', 'Lotado', 'Ativo'),
('06', '2026-09-17 08:00:00', 'Moderado', 'Ativo'),

-- Dia 18/09/2026
('03', '2026-09-18 07:45:00', 'Lotado', 'Ativo'),
('04', '2026-09-18 13:00:00', 'Vazio', 'Ativo'),
('05', '2026-09-18 17:55:00', 'Lotado', 'Ativo'),

-- Dia 19/09/2026
('01', '2026-09-19 07:30:00', 'Lotado', 'Ativo'),
('02', '2026-09-19 12:30:00', 'Moderado', 'Ativo'),
('06', '2026-09-19 18:10:00', 'Lotado', 'Ativo');

UPDATE passageiros SET tipo_usuario = 1 WHERE email = 'admin@monitor.com';