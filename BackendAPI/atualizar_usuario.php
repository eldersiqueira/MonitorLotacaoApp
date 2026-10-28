<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $data = json_decode(file_get_contents("php://input"), true);

    $id = $data['idPassageiro'] ?? $_POST['idPassageiro'] ?? null;
    $nome = $data['nome'] ?? $_POST['nome'] ?? null;
    $email = $data['email'] ?? $_POST['email'] ?? null;
    $tipo_usuario = $data['tipo_usuario'] ?? $_POST['tipo_usuario'] ?? null;

    if (!empty($id) && !empty($nome) && !empty($email)) {
        $stmt = $pdo->prepare("UPDATE passageiros SET nome = ?, email = ?, tipo_usuario = ? WHERE idPassageiro = ?");
        $stmt->execute([$nome, $email, $tipo_usuario, $id]);

        echo json_encode(["sucesso" => true, "mensagem" => "Utilizador atualizado com sucesso!"]);
    } else {
        echo json_encode(["sucesso" => false, "mensagem" => "Campos obrigatórios em falta ou ID inválido."]);
    }
} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>