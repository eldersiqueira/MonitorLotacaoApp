<?php
header("Content-Type: application/json; charset=UTF-8");

// Configuração da conexão com o banco de dados MySQL centralizado
$host = "localhost";
$dbname = "monitor_lotacao_db";
$usuario_db = "root";
$senha_db = "";

try {
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8", $usuario_db, $senha_db);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Recebe os dados enviados via POST pelo _apiService do .NET MAUI
    $nome = $_POST['nome'] ?? '';
    $email = $_POST['email'] ?? '';
    $senha = $_POST['senha'] ?? '';

    if (empty($nome) || empty($email) || empty($senha)) {
        echo json_encode([
            "sucesso" => false,
            "mensagem" => "Preencha todos os campos obrigatórios."
        ]);
        exit;
    }

    // Verifica se o e-mail já está registado no MySQL
    $stmtCheck = $pdo->prepare("SELECT idPassageiro FROM passageiros WHERE email = ?");
    $stmtCheck->execute([$email]);
    if ($stmtCheck->rowCount() > 0) {
        echo json_encode([
            "sucesso" => false,
            "mensagem" => "Este e-mail já está registado no sistema."
        ]);
        exit;
    }

    // Insere o novo passageiro na base de dados MySQL centralizada

    $stmtInsert = $pdo->prepare("INSERT INTO passageiros (nome, email, senha) VALUES (?, ?, ?)");
    $sucesso = $stmtInsert->execute([$nome, $email, $senha]);

    if ($sucesso) {
        echo json_encode([
            "sucesso" => true,
            "mensagem" => "Utilizador cadastrado com sucesso no MySQL."
        ]);
    } else {
        echo json_encode([
            "sucesso" => false,
            "mensagem" => "Erro ao inserir registo na base de dados."
        ]);
    }

} catch (Exception $e) {
    echo json_encode([
        "sucesso" => false,
        "mensagem" => "Erro no servidor: " . $e->getMessage()
    ]);
}
?>