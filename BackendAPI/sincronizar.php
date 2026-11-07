<?php
header("Content-Type: application/json; charset=UTF-8");

$host = "localhost";
$dbname = "monitor_lotacao_db";
$usuario_db = "root";
$senha_db = "";

try {
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8", $usuario_db, $senha_db);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Recebe os dados enviados via POST pelo aplicativo .NET MAUI
    $codigoLinha = trim($_POST['CodigoLinha'] ?? $_POST['codigo_linha'] ?? '');
    $nivelLotacao = trim($_POST['NivelLotacao'] ?? $_POST['nivel_lotacao'] ?? '');
    $idPassageiro = !empty($_POST['idPassageiro']) ? intval($_POST['idPassageiro']) : null;
    $horario = $_POST['Horario'] ?? date('Y-m-d H:i:s');
    $status = $_POST['Status'] ?? 'Ativo';

    if (empty($codigoLinha) || empty($nivelLotacao)) {
        echo json_encode([
            "sucesso" => false,
            "mensagem" => "Dados incompletos para o reporte."
        ]);
        exit;
    }

    // Insere o relato gravando o idPassageiro corretamente na tabela MySQL
    $stmt = $pdo->prepare("INSERT INTO relatos (CodigoLinha, NivelLotacao, idPassageiro, Horario, Status) VALUES (?, ?, ?, ?, ?)");
    $stmt->execute([$codigoLinha, $nivelLotacao, $idPassageiro, $horario, $status]);

    // Cálculo estatístico de consenso (TTL / Média da linha)
    $stmtMedia = $pdo->prepare("
        SELECT 
            COUNT(*) as total_relatos,
            SUM(CASE WHEN NivelLotacao = 'Vazio' THEN 1 ELSE 0 END) as total_vazio,
            SUM(CASE WHEN NivelLotacao = 'Moderado' THEN 1 ELSE 0 END) as total_moderado,
            SUM(CASE WHEN NivelLotacao = 'Lotado' THEN 1 ELSE 0 END) as total_lotado
        FROM relatos 
        WHERE CodigoLinha = ? AND Horario >= (NOW() - INTERVAL 2 HOUR)
    ");
    $stmtMedia->execute([$codigoLinha]);
    $estatisticas = $stmtMedia->fetch(PDO::FETCH_ASSOC);

    $total = (int)$estatisticas['total_relatos'];
    $statusAtual = "Sem dados recentes";
    $percentualMedio = 0;

    if ($total > 0) {
        $lotados = (int)$estatisticas['total_lotado'];
        $moderados = (int)$estatisticas['total_moderado'];
        
        if ($lotados >= $moderados && $lotados >= (int)$estatisticas['total_vazio']) {
            $statusAtual = "Lotado";
            $percentualMedio = 85;
        } elseif ($moderados >= (int)$estatisticas['total_vazio']) {
            $statusAtual = "Moderado";
            $percentualMedio = 65;
        } else {
            $statusAtual = "Vazio";
            $percentualMedio = 30;
        }
    }

    echo json_encode([
        "sucesso" => true,
        "mensagem" => "Relato sincronizado com sucesso!",
        "status_atual_linha" => $statusAtual,
        "percentual_medio" => $percentualMedio,
        "total_relatos_recentes" => $total
    ]);

} catch (Exception $e) {
    echo json_encode([
        "sucesso" => false,
        "mensagem" => "Erro no servidor: " . $e->getMessage()
    ]);
}
?>