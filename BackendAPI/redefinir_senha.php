<?php
header("Content-Type: application/json; charset=UTF-8");
$pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");

$data = json_decode(file_get_contents("php://input"), true);
$email = $data['email'] ?? '';
$novaSenha = $data['nova_senha'] ?? '';

if (!empty($email) && !empty($novaSenha)) {
    $stmt = $pdo->prepare("UPDATE passageiros SET senha = ? WHERE email = ?");
    $stmt->execute([$novaSenha, $email]);
    
    if ($stmt->rowCount() > 0) {
        echo json_encode(["sucesso" => true, "mensagem" => "Senha redefinida com sucesso."]);
    } else {
        echo json_encode(["sucesso" => false, "mensagem" => "E-mail não encontrado."]);
    }
} else {
    echo json_encode(["sucesso" => false, "mensagem" => "Preencha todos os campos."]);
}
?>