<?php
header("Content-Type: application/json; charset=UTF-8");
$pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");

$email = $_POST['email'] ?? '';
$senha = $_POST['senha'] ?? '';

$stmt = $pdo->prepare("SELECT idPassageiro, nome, email, tipo_usuario FROM passageiros WHERE email = ? AND senha = ?");
$stmt->execute([$email, $senha]);
$usuario = $stmt->fetch(PDO::FETCH_ASSOC);

if ($usuario) {
    echo json_encode([
        "sucesso" => true,
        "tipo_usuario" => (int)$usuario['tipo_usuario'], // 0 = Passageiro, 1 = Administrador
        "nome" => $usuario['nome']
    ]);
} else {
    echo json_encode(["sucesso" => false, "mensagem" => "Credenciais inválidas."]);
}
?>