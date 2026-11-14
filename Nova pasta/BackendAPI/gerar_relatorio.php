<?php
header("Content-Type: application/json; charset=UTF-8");

$servidor = "localhost";
$usuario = "root";
$senha = "";
$banco = "monitor_lotacao_db";

try {
    $pdo = new PDO("mysql:host=$servidor;dbname=$banco;charset=utf8", $usuario, $senha);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $periodo = $_GET['periodo'] ?? date('Y-m-d');

    // Query analítica para identificar horários críticos e volume de relatos por linha
    $sql = "SELECT 
                codigoLinha,
                DATE_FORMAT(horario, '%H:00:00') as FaixaHoraria,
                nivelLotacao,
                COUNT(*) as TotalOcorrencias
            FROM relatos
            GROUP BY codigoLinha, FaixaHoraria, nivelLotacao
            ORDER BY TotalOcorrencias DESC";

    $stmt = $pdo->prepare($sql);
    $stmt->execute();
    $relatorioDados = $stmt->fetchAll(PDO::FETCH_ASSOC);

    http_response_code(200);
    echo json_encode([
        "sucesso" => true,
        "periodo" => $periodo,
        "dados_estatisticos" => $relatorioDados
    ]);

} catch (PDOException $e) {
    http_response_code(500);
    echo json_encode(["sucesso" => false, "mensagem" => "Erro ao gerar relatório: " . $e->getMessage()]);
}
?>