<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $codigoLinha = $_GET['codigoLinha'] ?? '';

    if (!empty($codigoLinha)) {
        $stmt = $pdo->prepare("SELECT idPonto, nomePonto, latitude, longitude, ordem FROM pontos_itinerario WHERE codigoLinha = ? ORDER BY ordem ASC");
        $stmt->execute([$codigoLinha]);
        $pontos = $stmt->fetchAll(PDO::FETCH_ASSOC);

        echo json_encode([
            "sucesso" => true,
            "itinerario" => $pontos
        ]);
    } else {
        echo json_encode([
            "sucesso" => false, 
            "mensagem" => "Código da linha ausente."
        ]);
    }
} catch (Exception $e) {
    echo json_encode([
        "sucesso" => false, 
        "mensagem" => "Erro no servidor: " . $e->getMessage()
    ]);
}
?>