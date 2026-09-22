<?php
header("Content-Type: application/json; charset=UTF-8");

$servidor = "localhost";
$usuario = "root";
$senha = "";
$banco = "monitor_lotacao_db";

try {
    $pdo = new PDO("mysql:host=$servidor;dbname=$banco;charset=utf8", $usuario, $senha);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $codigoLinha = $_GET['linha'] ?? '';

    if (!empty($codigoLinha)) {
        $sql = "SELECT codigoLinha, horario, nivelLotacao, status 
                FROM relatos 
                WHERE codigoLinha = :linha 
                ORDER BY horario DESC LIMIT 50";
                
        $stmt = $pdo->prepare($sql);
        $stmt->execute([':linha' => $codigoLinha]);
        $historico = $stmt->fetchAll(PDO::FETCH_ASSOC);

        http_response_code(200);
        echo json_encode(["sucesso" => true, "dados" => $historico]);
    } else {
        http_response_code(400);
        echo json_encode(["sucesso" => false, "mensagem" => "Código da linha não informado."]);
    }
} catch (PDOException $e) {
    http_response_code(500);
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>