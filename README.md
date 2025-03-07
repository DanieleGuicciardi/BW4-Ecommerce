# BW4-Ecommerce - Build Week Project

## Descrizione
Questo progetto rappresenta il backend di un'applicazione **e-commerce** sviluppata utilizzando **C# con ASP.NET Core** e **SQL Server** per la gestione dei dati. Il sistema consente agli utenti di navigare tra le categorie di prodotti, visualizzare i dettagli degli articoli, aggiungerli al carrello e completare un ordine.

## Tecnologie Utilizzate
- **Linguaggio di Programmazione:** C#
- **Framework:** ASP.NET Core MVC
- **Database:** Microsoft SQL Server (gestito tramite SSMS)

## Struttura del Database
Il database **BW4_ECOMMERCE** è composto dalle seguenti tabelle principali:
- **CATEGORIES**: Gestisce le categorie dei prodotti.
- **PRODUCTS**: Contiene i dettagli dei prodotti, inclusi nome, prezzo, descrizione e immagini.
- **CART**: Memorizza i prodotti aggiunti al carrello dagli utenti.
- **LOGIN**: Gestisce gli account utente e le sessioni.

### Relazioni principali
- **`PRODUCTS`** è collegata a **`CATEGORIES`** tramite `IdCategory`.
- **`CART`** è collegata a **`PRODUCTS`** tramite `IdProduct` e a **`LOGIN`** tramite `Id`.

## Funzionalità Principali
**Gestione Utenti**
- Registrazione e login con controllo credenziali.
- Differenziazione tra utenti normali e amministratori.
- Logout sicuro.

**Gestione Prodotti**
- Aggiunta, modifica ed eliminazione di prodotti.
- Visualizzazione dei prodotti per categoria.
- Ricerca tra i prodotti.

**Gestione Categorie**
- Creazione e modifica di categorie con immagini.
- Eliminazione di categorie.

**Gestione Carrello**
- Aggiunta prodotti al carrello con gestione quantità.
- Modifica della quantità dei prodotti nel carrello.
- Eliminazione prodotti dal carrello.

## Installazione e Configurazione
### Clonare il Repository
```bash
git clone https://github.com/tuo-utente/tuo-repo.git
cd tuo-repo
```

### Configurare il Database
- Importare lo script SQL presente nella cartella `database`.
- Modificare `appsettings.json` con la stringa di connessione corretta.

### Avviare il Progetto
Aprire il progetto in **Visual Studio** e avviare l'applicazione.

## Developers
- [Daniele Guicciardi Ferrusi](https://github.com/DanieleGuicciardi)
- [Camilla Zicari](https://github.com/camillazicari)
- [Manuel Palmieri](https://github.com/Purple-Rain-Hub)
