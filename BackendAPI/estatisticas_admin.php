<?php
header("Content-Type: application/json; charset=UTF-8");
try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // 1. Total de relatos (Relatório de eficiência)
    $stmtTotal = $pdo->query("SELECT COUNT(*) as total FROM relatos");
    $totalRelatos = $stmtTotal->fetch(PDO::FETCH_ASSOC)['total'];

    // 2. Horários críticos (Agrupando pela hora onde a lotação é alta)
    $stmtCriticos = $pdo->query("
        SELECT HOUR(horario) as hora, COUNT(*) as quantidade
        FROM relatos
        WHERE nivelLotacao = 'Lotado' OR nivelLotacao = 'Moderado'
        GROUP BY HOUR(horario)
        ORDER BY quantidade DESC
        LIMIT 3
    ");
    $horarios = $stmtCriticos->fetchAll(PDO::FETCH_ASSOC);

    echo json_encode([
        "sucesso" => true,
        "totalRelatos" => $totalRelatos,
        "horariosCriticos" => $horarios
    ]);
} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => $e->getMessage()]);
}
?>