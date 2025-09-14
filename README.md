# 📦 API de Locação de Motos

![.NET](https://img.shields.io/badge/.NET-6.0-blueviolet)
![MongoDB](https://img.shields.io/badge/MongoDB-%2347A248.svg?logo=mongodb&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?logo=rabbitmq&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-%230db7ed.svg?logo=docker&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)

Este projeto fornece uma **API RESTful** para gerenciamento de **locação de motos**, permitindo o controle de **motos, entregadores, planos de aluguel e locações**, além de integração com **RabbitMQ** para mensageria.

---

## 📑 Sumário

- [🧩 Estrutura da Solução](#-estrutura-da-solução)
- [🛠️ Tecnologias Utilizadas](#️-tecnologias-utilizadas)
- [🚀 Como Executar o Projeto](#-como-executar-o-projeto)
  - [▶️ Com Docker (Produção)](#️-com-docker-produção)
  - [💻 Modo Local (sem Docker)](#-modo-local-sem-docker)
- [📚 Endpoints Disponíveis](#-endpoints-disponíveis)
- [📂 Estrutura de Diretórios](#-estrutura-de-diretórios)
- [📝 Considerações Finais](#-considerações-finais)
- [👨‍💻 Autor](#-autor)

---

## 🧩 Estrutura da Solução

A solução é dividida em **três projetos principais**:

### 1. API_Desafio_Backend
Projeto principal que expõe os **endpoints da API**.

- **Controllers**
  - `DeliveryPerson` → CRUD de entregadores (📁 `entregadores`)
  - `Motorcycle` → CRUD de motos (📁 `motos`)
  - `Rent` → Gerenciamento de aluguéis (📁 `locacoes`)
  - `RentalPlan` → Consulta de planos de aluguel (📁 `planos`)

- **Response**
  - `MessageResponse.cs` → Estrutura padrão para mensagens da API

---

### 2. Library_Desafio_Backend
Contém as **regras de negócio**, persistência de dados e utilitários.

- **DataBase/MongoDB**
  - `MongoDbSettings.cs` → Configuração de conexão
  - `MotorcycleRentDbContext.cs` → Contexto do MongoDB (via EF Core)
  - `Class/` → Modelos da aplicação (Motorcycle, Rent, DeliveryPerson, etc.)

- **FileHandler/Image/CNH**
  - `SaveCNH.cs` → Manipulação de imagens da CNH (base64)

- **MessageBroker/Event**
  - Definição de eventos consumidos/publicados via RabbitMQ

- **Security**
  - `RandomToken.cs` → Geração de tokens aleatórios

- **Service/Rent**
  - `RentCost.cs` → Cálculo de custo de aluguel

- **Utility**
  - `Mask.cs` → Formatação monetária

---

### 3. MessageBroker_Desafio_Backend
Gerencia a **mensageria com RabbitMQ**.  
Ativado apenas em **produção (via Docker)**.

---

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core Web API**
- **MongoDB + Entity Framework Core**
- **RabbitMQ**
- **Docker / Docker Compose**
- **C#**
- **Clean Architecture**

---

## 🚀 Como Executar o Projeto

### ▶️ Com Docker (Produção)

**Pré-requisitos:**  
- Docker  
- Docker Compose  

**Passo a passo:**

```bash
docker compose up --build
```

Isso irá subir:
- API → [http://localhost:7020/swagger/index.html](http://localhost:7020/swagger/index.html)
- MongoDB (com seed inicial)
- RabbitMQ → [http://localhost:15672](http://localhost:15672) (user: `guest`, pass: `guest`)

💡 **Observação para Windows**:  
Abra o **CMD**, navegue até a pasta onde está o arquivo `docker-compose.yml` e rode:  

``` 
docker-compose up
```

---

### 💻 Modo Local (sem Docker)

1. Certifique-se de que o **MongoDB** esteja rodando localmente (`localhost:27017`).  
2. Configure a conexão no `appsettings.json`:

```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "MotorcycleRentDB"
}
```
3. Execute o projeto `API_Desafio_Backend` como startup.

---

## 📚 Endpoints Disponíveis

### 🚚 Entregadores (`/Entregadores`)
- **POST** → Cadastrar entregador  
- **GET** → Consultar entregadores (com filtros opcionais)  
- **PUT** → Adicionar/alterar CNH (base64)  
  - O arquivo é salvo em uma **pasta FILE** na raiz do projeto, dentro de uma pasta criada com o **ID do entregador**.  
  - O **nome do arquivo** é gerado automaticamente e armazenado no banco de dados.  

### 🛵 Motos (`/Motos`)
- **POST** → Cadastrar moto  
- **GET** → Consultar motos (por ID ou placa)  
- **PUT** → Alterar placa  
- **DELETE** → Deletar moto  

### 📦 Locações (`/Locacao`)
- **POST** → Criar locação  
- **GET** → Consultar locações  
- **PUT** → Registrar devolução  

### 📋 Planos de Aluguel (`/Planos`)
- **GET** → Listar planos (por ID ou período)

### 📩 Mensageria (RabbitMQ)

Este projeto utiliza o **RabbitMQ** para mensageria, permitindo a publicação e o consumo de eventos relacionados aos processos de **locação de motos**.

- Os eventos são manipulados pelo serviço **MessageBroker_Desafio_Backend**.  
- Localização dos eventos no código:  
  ```
  MessageBroker_Desafio_Backend/Event
  ```

#### 🔄 Fluxo de eventos
- Ao **registrar uma moto** via `POST`, uma mensagem será enviada para a fila:  
  ```
  register-motorcycle_queue
  ```
- Caso o **ano da moto seja 2024**, os dados cadastrados também serão exibidos no **console** do serviço `MessageBroker_Desafio_Backend`.

#### 🐳 Acessando o console via Docker
1. Liste os containers ativos:  
   ```bash
   docker ps -a
   ```
2. Localize o container do **MessageBroker** (nome: `message-broker-desafio-backend`).  
3. Acesse o console para acompanhar os logs de eventos em tempo real com o comando : docker logs -f <**CONTAINER ID **>

## Mensageria
---

## 📂 Estrutura de Diretórios

```
.
├── API_Desafio_Backend
│   └── Controllers
│       ├── DeliveryPerson
│       ├── Motorcycle
│       ├── Rent
│       └── RentalPlan
│   └── Response/MessageResponse.cs
│
├── Library_Desafio_Backend
│   ├── DataBase/MongoDB
│   │   ├── MongoDbSettings.cs
│   │   ├── MotorcycleRentDbContext.cs
│   │   └── Class/
│   ├── FileHandler/Image/CNH/SaveCNH.cs
│   ├── MessageBroker/Event/
│   ├── Security/RandomToken.cs
│   ├── Service/Rent/RentCost.cs
│   └── Utility/Mask.cs
│
├── MessageBroker_Desafio_Backend
│   ├── Consumer/RegisterMotorcyleConsumer.cs
│   └── Event/RegisterMotorcyleEvent.cs

```

---

## 📝 Considerações Finais

- O **MongoDB** é inicializado com dados de **planos de aluguel** via `mongo-init.js`.  
- Arquitetura dividida em **API, Library e Mensageria** para melhor organização e escalabilidade.

---

## 👨‍💻 Autor

**Eduardo Mariano Tonaco**  
📧 Contato: [LinkedIn](https://www.linkedin.com/in/eduardo-mariano-tonaco-564953220/) | [GitHub](https://github.com/EduardoMTonaco)

