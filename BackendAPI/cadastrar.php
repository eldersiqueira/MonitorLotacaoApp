<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $nome = $_POST['nome'] ?? '';
    $email = $_POST['email'] ?? '';
    $senha = $_POST['senha'] ?? '';
    $tipo_usuario = 0; // 0 para passageiro comum

    if (!empty($nome) && !empty($email) && !empty($senha)) {
        // Verifica se o e-mail já existe na base de dados
        $stmtCheck = $pdo->prepare("SELECT idPassageiro FROM passageiros WHERE email = ?");
        $stmtCheck->execute([$email]);
        
        if ($stmtCheck->rowCount() > 0) {
            echo json_encode(["sucesso" => false, "mensagem" => "Este e-mail já está registado no sistema."]);
            exit;
        }

        // Insere o novo registo
        $stmt = $pdo->prepare("INSERT INTO passageiros (nome, email, senha, tipo_usuario) VALUES (?, ?, ?, ?)");
        $stmt->execute([$nome, $email, $senha, $tipo_usuario]);

        echo json_encode(["sucesso" => true, "mensagem" => "Registo efetuado com sucesso!"]);
    } else {
        echo json_encode(["sucesso" => false, "mensagem" => "Preencha todos os campos obrigatórios."]);
    }
} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>