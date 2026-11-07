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
    $email = trim($_POST['email'] ?? '');
    $senha = trim($_POST['senha'] ?? '');

    if (empty($email) || empty($senha)) {
        echo json_encode([
            "sucesso" => false,
            "mensagem" => "Preencha o e-mail e a senha."
        ]);
        exit;
    }

    // Consulta o utilizador na base de dados centralizada MySQL
    $stmt = $pdo->prepare("SELECT idPassageiro, nome, email, senha, tipo_usuario FROM passageiros WHERE email = ?");
    $stmt->execute([$email]);
    $usuario = $stmt->fetch(PDO::FETCH_ASSOC);

    // Validação da senha em texto plano (conforme os registos atuais no banco)
    if ($usuario && $senha === $usuario['senha']) {
        echo json_encode([
            "sucesso" => true,
            "tipo_usuario" => isset($usuario['tipo_usuario']) ? (int)$usuario['tipo_usuario'] : 0,
            "nome" => $usuario['nome'],
            "mensagem" => "Login efetuado com sucesso."
        ]);
    } else {
        echo json_encode([
            "sucesso" => false,
            "mensagem" => "Credenciais inválidas ou utilizador não cadastrado."
        ]);
    }

} catch (Exception $e) {
    echo json_encode([
        "sucesso" => false,
        "mensagem" => "Erro no servidor: " . $e->getMessage()
    ]);
}
?>