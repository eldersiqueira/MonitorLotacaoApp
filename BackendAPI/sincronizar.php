<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Recebe os dados padronizados via FormUrlEncoded (POST)
    $codigoLinha = $_POST['CodigoLinha'] ?? null;
    $nivelLotacao = $_POST['NivelLotacao'] ?? null;
    $horario = $_POST['Horario'] ?? null;
    $status = $_POST['Status'] ?? null;

    if (!empty($codigoLinha) && !empty($nivelLotacao)) {
        // Ajuste o nome da tabela 'relatos' conforme a estrutura real do seu banco
        $stmt = $pdo->prepare("INSERT INTO relatos (codigoLinha, nivelLotacao, horario, status) VALUES (?, ?, ?, ?)");
        $stmt->execute([$codigoLinha, $nivelLotacao, $horario, $status]);

        echo json_encode(["sucesso" => true, "mensagem" => "Relato sincronizado com sucesso!"]);
    } else {
        echo json_encode(["sucesso" => false, "mensagem" => "Dados de lotação incompletos."]);
    }
} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>