|CREATE DATABASE BW4_ECOMMERCE;

USE BW4_ECOMMERCE;

CREATE TABLE CATEGORIES(
Id INT IDENTITY PRIMARY KEY NOT NULL,
Name NVARCHAR(20) NOT NULL,
Img NVARCHAR(MAX) NOT NULL
);

CREATE TABLE PRODUCTS(
Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
Name NVARCHAR(50) NOT NULL,
Price MONEY NOT NULL,
Description NVARCHAR(MAX) NOT NULL,
DescriptionShort NVARCHAR(MAX) NOT NULL,
Img NVARCHAR(MAX) NOT NULL,
Img2 NVARCHAR(MAX),
Img3 NVARCHAR(MAX),
IdCategory INT NOT NULL,
CONSTRAINT FK_IdCategory FOREIGN KEY (IdCategory) REFERENCES CATEGORIES(Id)
);

CREATE TABLE CART(
Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
Quantity INT NOT NULL,
IdProduct UNIQUEIDENTIFIER NOT NULL,
CONSTRAINT FK_IdProduct FOREIGN KEY (IdProduct) REFERENCES PRODUCTS(Id)
);

INSERT INTO CATEGORIES(Name, Img) VALUES(
'Burgers', 'https://glovo.dhmedia.io/image/stores-glovo/stores/fb0c464a62de63c294ef415976d4e3f3d08b92c3b06e90287bbab4a9f0484890?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7Im1vZGUiOiJmaWxsIiwiYmciOiJ0cmFuc3BhcmVudCIsIndpZHRoIjo1ODgsImhlaWdodCI6MzIwfX1d'
),(
'Pizza', 'https://glovo.dhmedia.io/image/stores-glovo/stores/9bc6474e6e40b0837228e3d37cc2665ace58954726f49df7d19fa4f489afe9c6?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7Im1vZGUiOiJmaWxsIiwiYmciOiJ0cmFuc3BhcmVudCIsIndpZHRoIjo1ODgsImhlaWdodCI6MzIwfX1d'
),(
'Kebab', 'https://glovo.dhmedia.io/image/stores-glovo/stores/f2a178e2b041b860c39cb28edd2cd67d64be490f5c6ccc707784aa7d7b7d1029?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7Im1vZGUiOiJmaWxsIiwiYmciOiJ0cmFuc3BhcmVudCIsIndpZHRoIjo1ODgsImhlaWdodCI6MzIwfX1d'
),(
'Sushi', 'https://glovo.dhmedia.io/image/stores-glovo/stores/08a776f10793d29f4a49794d5c817ca0df11d93f2b2df4c34452a761ac66da06?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7Im1vZGUiOiJmaWxsIiwiYmciOiJ0cmFuc3BhcmVudCIsIndpZHRoIjo1ODgsImhlaWdodCI6MzIwfX1d'
),(
'Pasta', 'https://glovo.dhmedia.io/image/stores-glovo/stores/aabe41a3639e4349fe956c1d1a7fbc303f3d5f1347f2a282710d7a103493c359?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7Im1vZGUiOiJmaWxsIiwiYmciOiJ0cmFuc3BhcmVudCIsIndpZHRoIjo1ODgsImhlaWdodCI6MzIwfX1d'
);

INSERT INTO PRODUCTS(Id, Name, Price, Description, DescriptionShort, Img, IdCategory) VALUES
(
NEWID(),'Gran Crispy McBacon', 8.70, 'Chi ama il Crispy McBacon® ne prenderebbe volentieri un altro e un altro e un altro e un altro ancora. Per questo c’è il Gran Crispy McBacon®: panino con carne 100% bovina, croccante bacon 100% da pancetta italiana, formaggio e l''inconfondibile salsa Crispy. Come il classico, ma ancora più grande.',
'Crispy McBacon® ti aspetta in versione Gran.', 'https://d19yn8po4ik6n2.cloudfront.net/TPO-IT_CSO_6363_v5.png', 1
),
(
NEWID(), 'BigMac', 7.20, 'Se pensi di conoscerlo alla perfezione è perché non l’hai ancora provato.
Il Grande Classico di McDonald’s è pronto a stupirti con il suo gusto ancora più irresistibile. Lasciati avvolgere dal pane più caldo, trasportare dal sapore della sua carne più succosa e goditi un’ulteriore aggiunta della sua inconfondibile salsa: lo scoprirai ancora più buono.',
'Un gusto impossibile da spiegare.', 'https://d19yn8po4ik6n2.cloudfront.net/TPO-IT_CSO_2040_v5.png', 1
),
(
NEWID(), 'Salsiccia e Friarielli', 14, 'La pizza salsiccia e friarielli è una specialità napoletana con base di pizza bianca, condita con friarielli (tipica verdura amarognola simile alle cime di rapa), salsiccia sbriciolata, aglio, olio e peperoncino. Gustosa e saporita, è un classico della tradizione partenopea.',
'Provola affumicata d''Agerola, salsiccia e friarielli fatti in casa', 'https://glovo.dhmedia.io/image/menus-glovo/products/b2526164bab82d5d1768f9312447879b24b6ffa37560667c2fccd4505366969b?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 2
),
(
NEWID(), 'Bufalina', 14, 'La bufalina è una variante della margherita, preparata con mozzarella di bufala DOP,  pomodoro, basilico e olio extravergine d''oliva. Ha un sapore più intenso e una consistenza cremosa grazie alla qualità del formaggio.',
'Pomodori pelati, spolverata di Grana Padano e mozzarella di bufala campana', 'https://glovo.dhmedia.io/image/menus-glovo/products/a7df2222c173950e6c558a7dfe029d71e890182b1bccba31e151e677a9be0f59?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 2
),
(
NEWID(), 'Truffle Kebab', 8.99, 'Piadina artigianale, Kebab di pollo, Scaglie di Grana, Cipolla caramellata, Lattuga, Salsa Tartufata, Maionese al Tartufo by Heinz',
'Unisce sapori intensi e raffinati in un''esperienza unica', 'https://glovo.dhmedia.io/image/menus-glovo/products/0b20431e555571c16de7817bae66585a8425485582d7fa1697e54e0974e3bc85?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 3
),
(
NEWID(), 'Menu Kebab classico', 12, 'Kebab di Pollo, Pomodori, Cipolla, Lattuga, Salsa Yogurt, Salsa Piccante, Sides e Bevande a Scelta!',
'Un''ottima soluzione per chi ha tanta fame', 'https://glovo.dhmedia.io/image/menus-glovo/products/123854edadcd74219e1f40124638073ca4c46648b6b889270072363c6e85032e?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 3
),
(
NEWID(), 'Ebi Tempura Temaki', 8, 'Unisce la croccantezza della frittura alla freschezza degli ingredienti, tipico della cucina giapponese.',
'gambero fritto, riso, alga e sesamo', 'https://glovo.dhmedia.io/image/menus-glovo/products/de41612a7d874953b934e8870d990f0561bada91ca7993182438da85afb7ec7e?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 4
),
(
NEWID(), 'Rose Sake Nigiri', 14, 'Piatto raffinato dal sapore delicato e armonioso.',
'Riso alla barbabietola, salmone e spicy mayo.', 'https://glovo.dhmedia.io/image/menus-glovo/products/0dca930a2663132a5a437ca223db81f51103f03ecc253f8c4d8c64e6c25d1414?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 4
),
(
NEWID(), 'Amatriciana', 16, 'Un piatto iconico della cucina laziale, con guanciale croccante, pomodori freschi, peperoncino e pecorino romano. Un''esplosione di sapori autentici, perfetto per gli amanti della tradizione italiana',
'Pomodoro, guanciale e pecorino', 'https://glovo.dhmedia.io/image/menus-glovo/products/f385c6078f1fdcb10acc99bb164d1ee2f1dd5206cf92f47f2e2afc46bf0286f4?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=', 5
),
(
NEWID(), 'Carbonara', 18, 'Un classico della cucina romana: guanciale croccante, uova fresche, pecorino romano e pepe nero, per un piatto cremoso e ricco di sapore. Un connubio perfetto di semplicità e gusto.',
'Un connubio perfetto di semplicità e gusto.', 'https://images-ext-1.discordapp.net/external/C7ScDJlOxs-BX3qAOBhOhizBnXpGzbolh43vxdHlAiQ/%3Ft%3DW3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0%3D/https/glovo.dhmedia.io/image/menus-glovo/products/12be7475f4d232e9a62fe7fb3f85512e22604cf785737ac12a1b5e8e06288e24?format=webp&width=432&height=432', 5
)

INSERT INTO CATEGORIES(Name, Img) VALUES (
'Dolci', 'https://glovo.dhmedia.io/image/stores-glovo/stores/cd9ce163b9d7956d038a56c33b4cf0535ce77d0921f8576bb4af47c3b9466be0?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7Im1vZGUiOiJmaWxsIiwiYmciOiJ0cmFuc3BhcmVudCIsIndpZHRoIjo1ODgsImhlaWdodCI6MzIwfX1d'
)

INSERT INTO PRODUCTS(Id, Name, Price, Description, DescriptionShort, Img, IdCategory) VALUES
(NEWID(), 'Tiramisù', 11, 'Cremoso e avvolgente, è un classico amato in tutto il mondo.', 'Coppa di Tiramisù monoporzione in vetro.', 'https://glovo.dhmedia.io/image/menus-glovo/products/6d82e0340fda5bc6d953655ad73a07250b2703ad62a7a8da9661beb08c76333f?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=',6),
(NEWID(), 'Tortino al cioccolato', 9, 'Tortino al Cioccolato con Barattolino di Crema Pasticcera.', 'Servito caldo, sprigiona un''esplosione di cioccolato fuso ad ogni morso, perfetto per gli amanti del cacao.', 'https://glovo.dhmedia.io/image/menus-glovo/products/bf76c264592d249df107368c4f7e587f688e15cd0705bbde928f07aa3afc791a?t=W3siYXV0byI6eyJxIjoibG93In19LHsicmVzaXplIjp7IndpZHRoIjo2MDB9fV0=',6);

SELECT P.Id, P.Name, P.Price, P.Description, P.DescriptionShort, P.Img, C.Id FROM PRODUCTS P INNER JOIN CATEGORIES C ON P.IdCategory = C.Id WHERE P.IdCategory = 1;

UPDATE PRODUCTS SET Description = 'Servito caldo, sprigiona un''esplosione di cioccolato fuso ad ogni morso, perfetto per gli amanti del cacao.'  WHERE Name = 'Tortino al cioccolato'