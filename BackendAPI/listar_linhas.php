<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $stmt = $pdo->query("SELECT codigoLinha, nomeLinha, trajeto FROM linhas_onibus");
    $linhas = $stmt->fetchAll(PDO::FETCH_ASSOC);

    echo json_encode([
        "sucesso" => true,
        "linhas" => $linhas
    ]);
} catch (Exception $e) {
    echo json_encode([
        "sucesso" => false, 
        "mensagem" => "Erro ao carregar linhas: " . $e->getMessage()
    ]);
}
?>