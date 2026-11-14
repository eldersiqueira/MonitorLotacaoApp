<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $idPassageiro = $_POST['idPassageiro'] ?? '';

    if (!empty($idPassageiro)) {
        $stmt = $pdo->prepare("DELETE FROM passageiros WHERE idPassageiro = ?");
        $stmt->execute([$idPassageiro]);

        echo json_encode(["sucesso" => true, "mensagem" => "Utilizador removido com sucesso!"]);
    } else {
        echo json_encode(["sucesso" => false, "mensagem" => "ID do utilizador inválido."]);
    }
} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>