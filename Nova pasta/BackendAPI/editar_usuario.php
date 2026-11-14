<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $id = $_POST['idPassageiro'] ?? '';
    $nome = $_POST['nome'] ?? '';
    $email = $_POST['email'] ?? '';
    $tipo_usuario = $_POST['tipo_usuario'] ?? '';

    if (!empty($id) && !empty($nome) && !empty($email)) {
        $stmt = $pdo->prepare("UPDATE passageiros SET nome = ?, email = ?, tipo_usuario = ? WHERE idPassageiro = ?");
        $stmt->execute([$nome, $email, $tipo_usuario, $id]);

        echo json_encode(["sucesso" => true, "mensagem" => "Utilizador atualizado com sucesso!"]);
    } else {
        echo json_encode(["sucesso" => false, "mensagem" => "Preencha todos os campos obrigatórios."]);
    }
} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>