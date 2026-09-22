<?php
header("Content-Type: application/json; charset=UTF-8");

try {
    // Estabelece a ligação com a base de dados MySQL centralizada do projeto
    $pdo = new PDO("mysql:host=localhost;dbname=monitor_lotacao_db;charset=utf8", "root", "");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Prepara e executa a query para selecionar os campos necessários para a gestão de utilizadores
    $stmt = $pdo->prepare("SELECT idPassageiro, nome, email, tipo_usuario FROM passageiros");
    $stmt->execute();
    
    $usuarios = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Retorna os dados em formato JSON puro para serem desserializados pelo .NET MAUI
    echo json_encode($usuarios);

} catch (Exception $e) {
    echo json_encode(["sucesso" => false, "mensagem" => "Erro no servidor: " . $e->getMessage()]);
}
?>