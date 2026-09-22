<?php
header("Content-Type: application/json; charset=UTF-8");
$pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");

$acao = $_GET['acao'] ?? 'listar';

if ($acao == 'listar') {
    $stmt = $pdo->query("SELECT idPassageiro, nome, email, tipo_usuario FROM passageiros");
    echo json_encode(["sucesso" => true, "usuarios" => $stmt->fetchAll(PDO::FETCH_ASSOC)]);
} 
elseif ($acao == 'remover') {
    $id = $_POST['id'] ?? 0;
    $stmt = $pdo->prepare("DELETE FROM passageiros WHERE idPassageiro = ?");
    $stmt->execute([$id]);
    echo json_encode(["sucesso" => true, "mensagem" => "Utilizador removido com sucesso."]);
}
elseif ($acao == 'atualizar') {
    $id = $_POST['id'] ?? 0;
    $nome = $_POST['nome'] ?? '';
    $email = $_POST['email'] ?? '';
    $stmt = $pdo->prepare("UPDATE passageiros SET nome = ?, email = ? WHERE idPassageiro = ?");
    $stmt->execute([$nome, $email, $id]);
    echo json_encode(["sucesso" => true, "mensagem" => "Dados atualizados com sucesso."]);
}
?>