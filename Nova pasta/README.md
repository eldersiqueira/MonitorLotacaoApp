# 🚌 TCC - Monitor de Lotação e Eficiência das Linhas de Ônibus
Trabalho de Conclusão de Curso Técnico em Desenvolvimento de Sistemas (EaD - Centro Paula Souza)

## 📖 Descrição Geral
O sistema consiste em um aplicativo móvel nativo desenvolvido para as plataformas Android e iOS, utilizando o framework **.NET MAUI**, onde os passageiros reportam em tempo real o nível de lotação do veículo por meio de uma interface simplificada de toque único[cite: 5]. O app mobile aproveita a integração profunda com o hardware do smartphone para realizar a validação automática via **API de Geolocalização nativa**, garantindo que o relato seja efetuado apenas por usuários que estejam efetivamente no trajeto da linha.

---

## 🎯 Objetivos e Metas
* **Objetivo Principal:** Desenvolver e implementar um sistema de monitoramento colaborativo para **5 linhas de ônibus específicas**.
* **Meta Quantitativa:** Atingir uma base de pelo menos 200 relatos semanais de usuários para subsidiar a tomada de decisão da gestão de frota.
* **Impacto Social (ODS):** 
  * **ODS 11:** Cidades e Comunidades Sustentáveis (transporte urbano mais eficiente).
  * **ODS 8:** Trabalho Decente e Crescimento Econômico (otimização do deslocamento e bem-estar do trabalhador).

---

## 🛠️ Stack Tecnológica & Ferramentas

### 💻 Linguagens e Frameworks
* **C# (.NET 8 / .NET MAUI):** Desenvolvimento multiplataforma da aplicação mobile.
* **XAML:** Estruturação visual e design das interfaces de usuário (UI).
* **PHP / Java:** Desenvolvimento do backend e rotas de API para comunicação com o servidor.
* **SQL:** Consultas e manipulação de dados relacionais.

### 🗄️ Bancos de Dados
* **SQLite:** Persistência de dados local no dispositivo móvel (suporte a modo *offline* e sincronização posterior).
* **MySQL:** Banco de dados relacional centralizado no servidor para consolidação dos relatos e gestão de frotas/perfis.

### 🛠️ IDEs e Editores
* **Visual Studio (Community / Professional):** Ambiente principal de desenvolvimento, compilação e testes em C#/.NET MAUI.
* **Visual Studio Code / NetBeans:** Utilizados como suporte para scripts de API e gerenciamento do backend.
* **phpMyAdmin / XAMPP:** Gestão local do servidor web e banco de dados MySQL.

### 📱 Simuladores e Dispositivos de Teste
* **Android Emulator (Google APIs / x86_64):** Utilizado para testes de layout, validação de inputs, rotas e consumo de APIs locais (`http://10.0.2.2/`).
* **Smartphones Físicos:** Testes de campo para validação da API de Geolocalização nativa e hardware de GPS.

### 📊 Ferramentas de Gestão e Modelagem
* **Trello:** Gestão de tarefas baseada na metodologia Ágil (Scrum / Sprints).
* **Astah UML:** Elaboração de diagramas de Casos de Uso e Diagramas de Classes.
* **Git & GitHub:** Versionamento de código-fonte e controle de branches da equipe.

---

## 📂 Estrutura do Repositório
O projeto está estruturado em frentes tecnológicas organizadas da seguinte forma:
* **`📱 /Mobile`**: Código-fonte do aplicativo desenvolvido em .NET MAUI (Views, ViewModels, Services, Models).
* **`⚙️ /API`**: Scripts de backend em PHP/Java responsáveis por processar requisições HTTP e comunicar com o MySQL.
* **`📊 /Docs`**: Documentações técnicas, diagramas UML e cronogramas de Sprints.

---

## 👥 Equipe Técnica
* **Carina Aparecida Geraldo de Melo**
* **Elder Henrique de Oliveira Siqueira** 
* **Emanuel Soares de Lima**
* **Elivelton Ortiz de Souza**
