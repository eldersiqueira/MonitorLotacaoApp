<?php
header("Content-Type: application/json; charset=UTF-8");

// Configurações de conexão com o banco MySQL local
$servidor = "localhost";
$usuario = "root";
$senha = "";
$banco = "monitor_lotacao_db";

try {
    $pdo = new PDO("mysql:host=$servidor;dbname=$banco;charset=utf8", $usuario, $senha);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Captura o corpo da requisição JSON enviada pelo .NET MAUI
    $json = file_get_contents("php://input");
    $dados = json_decode($json, true);

    if (!empty($dados)) {
        $codigoLinha = $dados['CodigoLinha'] ?? '';
        $nivelLotacao = $dados['NivelLotacao'] ?? '';
        $horario = $dados['Horario'] ?? date('Y-m-d H:i:s');
        $status = $dados['Status'] ?? 'Sincronizado';

        // Prepara e executa a inserção no MySQL centralizado
        $sql = "INSERT INTO relatos (codigoLinha, nivelLotacao, horario, status) VALUES (:codigo, :nivel, :horario, :status)";
        $stmt = $pdo->prepare($sql);
        
        $stmt->execute([
            ':codigo' => $codigoLinha,
            ':nivel' => $nivelLotacao,
            ':horario' => $horario,
            ':status' => $status
        ]);

        http_response_code(200);
        echo json_encode(["sucesso" => true, "mensagem" => "Relato sincronizado com sucesso no servidor!"]);
    } else {
        http_response_code(400);
        echo json_encode(["sucesso" => false, "mensagem" => "Dados inválidos ou JSON vazio."]);
    }

} catch (PDOException $e) {
    http_response_code(500);
    echo json_encode(["sucesso" => false, "mensagem" => "Erro de conexão no MySQL: " . $e->getMessage()]);
}
?>