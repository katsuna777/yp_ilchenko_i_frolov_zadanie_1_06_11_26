SET NAMES utf8mb4;
SET SESSION sql_mode = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION,NO_BACKSLASH_ESCAPES';
-- Полная база Чудо Обувь. Импортировать в пустую базу через MySQL Workbench.
CREATE DATABASE IF NOT EXISTS `isp-44_ilchenko_dm_chudo_obuv` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `isp-44_ilchenko_dm_chudo_obuv`;

CREATE TABLE users (
 id INT PRIMARY KEY AUTO_INCREMENT,
 login VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL UNIQUE,
 last_name VARCHAR(100) NOT NULL, first_name VARCHAR(100) NOT NULL,
 patronymic VARCHAR(100) NOT NULL,
 role ENUM('Admin','Manager','Customer') NOT NULL
) ENGINE=InnoDB;
CREATE TABLE sizes (id INT PRIMARY KEY AUTO_INCREMENT, label VARCHAR(20) NOT NULL UNIQUE) ENGINE=InnoDB;
CREATE TABLE products (
 id INT PRIMARY KEY AUTO_INCREMENT, category VARCHAR(100) NOT NULL,
 subcategory VARCHAR(100) NOT NULL, image VARCHAR(255) NOT NULL,
 name VARCHAR(255) NOT NULL, manufacturer VARCHAR(150) NOT NULL,
 description TEXT NOT NULL, composition TEXT NOT NULL,
 price DECIMAL(12,2) NOT NULL CHECK(price >= 0),
 UNIQUE KEY model(name,manufacturer)
) ENGINE=InnoDB;
CREATE TABLE stock_items (
 id INT PRIMARY KEY AUTO_INCREMENT, product_id INT NOT NULL, size_id INT NOT NULL,
 quantity INT NOT NULL CHECK(quantity >= 0), UNIQUE KEY model_size(product_id,size_id),
 FOREIGN KEY(product_id) REFERENCES products(id), FOREIGN KEY(size_id) REFERENCES sizes(id)
) ENGINE=InnoDB;
CREATE TABLE orders (
 id INT PRIMARY KEY AUTO_INCREMENT, ordered_at DATETIME NOT NULL,
 client_last_name VARCHAR(100) NOT NULL, client_first_name VARCHAR(100) NOT NULL,
 client_patronymic VARCHAR(100) NOT NULL, created_by INT NULL,
 version INT NOT NULL DEFAULT 1,
 FOREIGN KEY(created_by) REFERENCES users(id), INDEX order_date(ordered_at)
) ENGINE=InnoDB;
CREATE TABLE order_items (
 order_id INT NOT NULL, stock_item_id INT NOT NULL, quantity INT NOT NULL CHECK(quantity > 0),
 unit_price DECIMAL(12,2) NOT NULL CHECK(unit_price >= 0),
 PRIMARY KEY(order_id,stock_item_id),
 FOREIGN KEY(order_id) REFERENCES orders(id) ON DELETE CASCADE,
 FOREIGN KEY(stock_item_id) REFERENCES stock_items(id)
) ENGINE=InnoDB;

-- Generated from the supplied XLSX; stocks are already AVAILABLE quantities.
START TRANSACTION;
INSERT INTO users VALUES (1,'isivanov','Иванов','Иван','Сергеевич','Admin');
INSERT INTO users VALUES (2,'papetrov','Петров','Пётр','Алексеевич','Manager');
INSERT INTO users VALUES (3,'asidorova','Сидорова','Анна','Дмитриевна','Customer');
INSERT INTO users VALUES (4,'ekuznetsova','Кузнецова','Екатерина','Андреевна','Manager');
INSERT INTO users VALUES (5,'dsmirnov','Смирнов','Дмитрий','Владимирович','Customer');
INSERT INTO users VALUES (6,'apopov','Попов','Алексей','Николаевич','Admin');
INSERT INTO users VALUES (7,'ovasileva','Васильева','Ольга','Игоревна','Customer');
INSERT INTO users VALUES (8,'psokolov','Соколов','Павел','Викторович','Manager');
INSERT INTO users VALUES (9,'tmikhailova','Михайлова','Татьяна','Юрьевна','Customer');
INSERT INTO users VALUES (10,'nnovikova','Новикова','Наталья','Сергеевна','Manager');
INSERT INTO users VALUES (11,'afedorov','Фёдоров','Андрей','Максимович','Admin');
INSERT INTO users VALUES (12,'smorozov','Морозов','Сергей','Павлович','Customer');
INSERT INTO users VALUES (13,'jvolkova','Волкова','Юлия','Александровна','Manager');
INSERT INTO users VALUES (14,'aalekseev','Алексеев','Артём','Евгеньевич','Customer');
INSERT INTO users VALUES (15,'ilebedeva','Лебедева','Ирина','Владимировна','Customer');
INSERT INTO users VALUES (16,'negorov','Егоров','Никита','Денисович','Manager');
INSERT INTO users VALUES (17,'epavlova','Павлова','Елена','Петровна','Customer');
INSERT INTO users VALUES (18,'rkozlov','Козлов','Роман','Олегович','Admin');
INSERT INTO users VALUES (19,'vstepanova','Степанова','Виктория','Романовна','Manager');
INSERT INTO users VALUES (20,'vnikolaev','Николаев','Владимир','Иванович','Customer');
INSERT INTO sizes VALUES (1,'18');
INSERT INTO sizes VALUES (2,'19');
INSERT INTO sizes VALUES (3,'20');
INSERT INTO sizes VALUES (4,'21');
INSERT INTO sizes VALUES (5,'22');
INSERT INTO sizes VALUES (6,'23');
INSERT INTO sizes VALUES (7,'24');
INSERT INTO sizes VALUES (8,'25');
INSERT INTO sizes VALUES (9,'26');
INSERT INTO sizes VALUES (10,'27');
INSERT INTO sizes VALUES (11,'28');
INSERT INTO sizes VALUES (12,'29');
INSERT INTO sizes VALUES (13,'30');
INSERT INTO sizes VALUES (14,'31');
INSERT INTO sizes VALUES (15,'32');
INSERT INTO sizes VALUES (16,'33');
INSERT INTO sizes VALUES (17,'34');
INSERT INTO sizes VALUES (18,'35');
INSERT INTO sizes VALUES (19,'36');
INSERT INTO sizes VALUES (20,'36.5');
INSERT INTO sizes VALUES (21,'37');
INSERT INTO sizes VALUES (22,'37.5');
INSERT INTO sizes VALUES (23,'38');
INSERT INTO sizes VALUES (24,'38.5');
INSERT INTO sizes VALUES (25,'39');
INSERT INTO sizes VALUES (26,'39.5');
INSERT INTO sizes VALUES (27,'40');
INSERT INTO sizes VALUES (28,'40.5');
INSERT INTO sizes VALUES (29,'41');
INSERT INTO sizes VALUES (30,'41.5');
INSERT INTO sizes VALUES (31,'42');
INSERT INTO sizes VALUES (32,'42.5');
INSERT INTO sizes VALUES (33,'43');
INSERT INTO sizes VALUES (34,'44');
INSERT INTO sizes VALUES (35,'45');
INSERT INTO products VALUES (1,'Детская обувь','Кроссовки','IMG_KS_195217.png','Кроссовки детские «Звёздочка» экокожа','Малыш-Спорт','Кроссовки детские «Звёздочка» экокожа. Производитель: ООО «Малыш-Спорт», г. Смоленск','Верх — экокожа (полиуретан на хлопковой основе), подкладка — хлопок 100%, подошва — ПВХ',2190.00);
INSERT INTO products VALUES (2,'Детская обувь','Кроссовки','IMG_KS_195213.png','Кроссовки детские «Гонщик» экокожа','Топ-Топ','Кроссовки детские «Гонщик» экокожа. Производитель: Фабрика «Топ-Топ», г. Киров','Верх — экокожа (микрофибра), подкладка — текстиль сетчатый (полиэстер), подошва — термопластичная резина (ТЭП)',2490.00);
INSERT INTO products VALUES (3,'Детская обувь','Кроссовки','IMG_KS_195222.png','Кроссовки детские «Радуга» экокожа перфорированная','Лапушка','Кроссовки детские «Радуга» экокожа перфорированная. Производитель: ИП Смирнова Л.В. (бренд «Лапушка»), г. Вологда','Верх — экокожа перфорированная, вставки — нейлон, стелька — ортопедическая с латексной подушкой, подошва — ЭВА',1990.00);
INSERT INTO products VALUES (4,'Детская обувь','Сапоги','IMG_KB_185501.png','Сапожки детские «Снежинка» зимние на меху','Тёплые ножки','Сапожки детские «Снежинка» зимние на меху. Производитель: ООО «Тёплые ножки», г. Новосибирск','Верх — экокожа морозостойкая, утеплитель — искусственный мех (акрил 70%, полиэстер 30%), подкладка — флис, подошва — термоэластопласт (ТЭП)',3490.00);
INSERT INTO products VALUES (5,'Детская обувь','Сапоги','IMG_KB_164816.png','Сапожки детские «Медвежонок» демисезонные непромокаемые','Северята','Сапожки детские «Медвежонок» демисезонные непромокаемые. Производитель: Компания «Северята», г. Архангельск','Верх — плащёвая ткань с водоотталкивающей пропиткой (полиэстер 100%), отделка — экокожа, подкладка — микрофибра, мембрана — AirTex, подошва — резина',3190.00);
INSERT INTO products VALUES (6,'Детская обувь','Сапоги','IMG_KB_164811.png','Сапожки детские «Принцесса» кожаные','Весна','Сапожки детские «Принцесса» кожаные. Производитель: Мастерская «Весна», г. Тула','Верх — натуральная кожа (КРС), подкладка — шерсть натуральная (70%), байка (30%), стелька — войлочная, подошва — полиуретан',4290.00);
INSERT INTO products VALUES (7,'Детская обувь','Босоножки','IMG_KSd_195207.png','Сандалии детские «Пчёлка» ортопедические летние','Здоровый шаг','Сандалии детские «Пчёлка» ортопедические летние. Производитель: ООО «Здоровый шаг», г. Воронеж','Верх — нубук натуральный, подкладка — кожа растительного дубления, стелька — анатомическая с супинатором (латекс + пробка), подошва — каучук',2790.00);
INSERT INTO products VALUES (8,'Детская обувь','Босоножки','IMG_KSd_185508.png','Сандалии детские «Морячок» текстильные с фиксатором','Карапуз','Сандалии детские «Морячок» текстильные с фиксатором. Производитель: ИП Кузнецов А.Д. (бренд «Карапуз»), г. Краснодар','Верх — джинсовая ткань (хлопок 100%), окантовка — искусственная кожа, подкладка — хлопок, подошва — ЭВА лёгкая',1690.00);
INSERT INTO products VALUES (9,'Детская обувь','Туфли','IMG_KSho_185512.png','Туфли детские «Бантик» лаковые праздничные','Маленький франт','Туфли детские «Бантик» лаковые праздничные. Производитель: Дом обуви «Маленький франт», г. Москва','Верх — искусственная лаковая кожа (ПВХ), носок усиленный — термопластичный полиуретан, подкладка — кожа искусственная дышащая, стелька — анатомическая с амортизацией, подошва — полиуретан',2390.00);
INSERT INTO products VALUES (10,'Женская обувь','Сапоги','IMG_WB_170905.png','Сапоги зимние натуральная кожа','Барбари','Сапоги женские зимние, нескользящая, устойчивая подошва, натуральный мех. Производитель: Дом обуви «Барбари», г. Новороссийск ','Верх — натуральная кожа, подкладка — евромех, натуральный мех, текстильный утеплитель, подошва — резина',25000.00);
INSERT INTO products VALUES (11,'Женская обувь','Сапоги','IMG_WB_170859.png','Сапоги осенние натуральная кожа','Барбари','Сапоги женские демисезонные из высококачественной натуральной кожи. Производитель: Дом обуви «Барбари», г. Новороссийск ','Верх — натуральная кожа, подкладка — текстильный утеплитель, стелька — текстиль, подошва — ТПР',15000.00);
INSERT INTO products VALUES (12,'Женская обувь','Сапоги','IMG_WB_170906.png','Сапоги трубы демисезонные замшевые','Твой комфорт','Сапоги женские демисезонные замшевые. Производитель: ОАО «Твой комфорт», г. Ярославль','Верх — натуральная замша, утепленные, тракторная подошва, стелька — байка, подошва — резина',17000.00);
INSERT INTO products VALUES (13,'Женская обувь','Босоножки','IMG_WSd_174332.png','Босоножки «Песчаный берег» коричневые','Стиль и комфорт','Коричневая кожаная модель с открытым носком, боковой пряжкой и закрытым каблуком. Производитель: ИП «Стиль и комфорт», г. Санкт-Петербург','Верх — 100% козья кожа, подкладка — 100% кожа; подошва: 100% кожа',7500.00);
INSERT INTO products VALUES (14,'Женская обувь','Босоножки','IMG_WSd_185156.png','Женские босоножки «Черный кофе» со скульптурным каблуком','Кожевенные традиции','Открытый носок и ремешок со сборной пряжкой, низкий каблук и резиновая подошва гарантируют комфорт. Производитель: ЗАО «Кожевенные традиции», г. Вятка','Верх — кожа собственного производств, подкладка — кожа; подошва: резина',5600.00);
INSERT INTO products VALUES (15,'Женская обувь','Босоножки','IMG_WSd_185551.png','Босоножки женские «Летний бриз»','Модный силуэт','Босоножки женские кожаные белые, регулируемый ремешок, закрытый каблук и кожаная подошва. Производитель: ООО «Модный силуэт», г. Кострома','Верх — натуральная кожа, подкладка — натуральная кожа, подошва — кожа',7130.00);
INSERT INTO products VALUES (16,'Женская обувь','Туфли','IMG_WSho_174409.png','Демисезонные туфли натуральная кожа','Модный силуэт','Комфортные лаконичные лодочки, классический дизайна с современными акцентами. Производитель: ООО «Модный силуэт», г. Кострома','Верх — натуральная овечья кожа, подкладка и стелька — натуральная овечья кожа, подошва — резина',12340.00);
INSERT INTO products VALUES (17,'Женская обувь','Туфли','IMG_WSho_185202.png','Туфли женские натуральная кожа с металлическими вставками','Кожевенные традиции','Стильные туфли для весеннего и летнего сезона. Производитель: ЗАО «Кожевенные традиции», г. Вятка','Верх — натуральная телячьей кожа, металлическая пряжка, подкладка и стелька— натуральная овечья кожа, подошва — резина',7757.00);
INSERT INTO products VALUES (18,'Женская обувь','Туфли','IMG_WSho_185545.png','Туфли классические лодочки натуральная кожа','Стиль и комфорт','Модель из натуральной кожи, классический острый носок и изящная шпилька. Производитель: ИП «Стиль и комфорт», г. Санкт-Петербург','Верх — натуральная кожа, подкладка и стелька — натуральная кожа, подошва — тунит',7550.00);
INSERT INTO products VALUES (19,'Женская обувь','Кроссовки','IMG_WS_174110.png','Кроссовки белые с оранжевым и красным','Топ-Топ','Многослойный верх из кожи и текстиля. Производитель: Фабрика «Топ-Топ», г. Киров','Верх — 70% кожа, 30% текстиль, подкладка — текстиль, подошва — резина',9450.00);
INSERT INTO products VALUES (20,'Женская обувь','Кроссовки','IMG_WS_185528.png','Кроссовки летние кожаные','Топ-Топ','Уверенность на каждом шагу. Материал обеспечивает долговечность и комфорт при носке. Производитель: Фабрика «Топ-Топ», г. Киров','Верх — 87% кожа, 11% текстиль, 2% синтетика; подкладка: 100% текстиль; подошва: резина, пластик',8565.00);
INSERT INTO products VALUES (21,'Женская обувь','Кроссовки','IMG_WS_185535.png','Кроссовки кожаные','Стиль и комфорт','Кроссовки женские из натуральной кожи. Производитель: ИП «Стиль и комфорт», г. Санкт-Петербург','Верх — натуральная кожа, подкладка — кожа, подошва — полиуретан',8765.00);
INSERT INTO products VALUES (22,'Женская обувь','Кроссовки','IMG_WS_185538.png','Кроссовки «Стиль и комфорт»','Стиль и комфорт','Кроссовки женские из натуральной кожи и синтетических материалов, сетчатые вставки. Производитель: ИП «Стиль и комфорт», г. Санкт-Петербург','Верх — 70% кожа, 20% полиамид, 10% полиуретан; подкладка — текстиль, подошва — ТПУ',7795.00);
INSERT INTO products VALUES (23,'Мужская обувь','Кроссовки','IMG_MS_185531.png','Кроссовки для бега «Драйв»','Азимут','Сочетание стиля, комфорта и современных технологий. Производитель: Фабрика «Азимут», г. Санкт-Петербург','Верх —  сетка, подкладка — текстиль, подошва —  резина',6550.00);
INSERT INTO products VALUES (24,'Мужская обувь','Кроссовки','IMG_MS_185532.png','Кроссовки «Азимут Бег» ','Азимут','Универсальный выбор для повседневного использования. Производитель: Фабрика «Азимут», г. Санкт-Петербург','Верх — 6% кожа, 29% текстиль, 15% полимерные материалы; подкладка — 100% текстиль; подошва — полимерные материалы, резина',7890.00);
INSERT INTO products VALUES (25,'Мужская обувь','Кроссовки','IMG_MS_185533.png','Кроссовки','Топ-Топ','Кроссовки для мужчин, идеальное сочетание стиля и комфорта. Производитель: Фабрика «Топ-Топ», г. Киров','Верх — 56% кожа, 29% текстиль, 15% полимерные материалы; подкладка — 100% текстиль; подошва — полимерные материалы, резина',9567.00);
INSERT INTO products VALUES (26,'Мужская обувь','Туфли','IMG_MSho_190514.png','Черные туфли в классическом стиле — база для деловых образов','Барбари','Модель выполнена из натуральной кожи, на резиновой подошве. Производитель: Дом обуви «Барбари», г. Новороссийск ','Верх — натуральная кожа, подкладка — текстиль, стелька — натуральная кожа, подошва — резина',24569.00);
INSERT INTO products VALUES (27,'Мужская обувь','Туфли','IMG_MSho_190518.png','Туфли мужские светлые классика демисезонные','Берестов','Туфли на низком каблуке кожаные','Верх — натуральная кожа, подкладка — текстиль, стелька — натуральная кожа, подошва — ТЭП',10155.00);
INSERT INTO products VALUES (28,'Мужская обувь','Туфли','IMG_MSho_191612.png','Туфли из натуральной кожи','Берестов','Модель выполнена из натуральной кожи, на резиновой подошве, поддеживает классический и деловой стиль образов. Производитель: Дом обуви «Берестов», г. Москва','Верх — натуральная кожа, подкладка — натуральная кожа, стелька — натуральная кожа, подошва — резина',25625.00);
INSERT INTO products VALUES (29,'Мужская обувь','Ботинки','IMG_MB_174349.png','Ботинки мужские демисезонные','Барбари','Ботинки из натуральной кожи темно-коричневого и черного цвета на шнурках. Производитель: Дом обуви «Берестов», г. Москва','Верх — натуральная кожа, подкладка — текстиль, подошва — искусственный материал',21500.00);
INSERT INTO products VALUES (30,'Мужская обувь','Ботинки','IMG_MB_191200.png','Ботинки черные из натуральной кожи','Кожевенные традиции','Ботинки представляют баланс традиций и современности. Выполнены из натуральной кожи насыщенного черного цвета. Производитель: ЗАО «Кожевенные традиции», г. Вятка','Верх — натуральная кожа, подкладка — ворсин, стелька — ворсин, подошва — тунит',14567.00);
INSERT INTO products VALUES (31,'Мужская обувь','Ботинки','IMG_MB_191205.png','Ботинки зимние классические','Барбари','Выбор на каждый день — натуральная кожа, натуральный мех. Производитель: Дом обуви «Барбари», г. Новороссийск ','Верх — натуральная кожа, подкладка — натуральный мех, стелька — натуральный мех, подошва — ТЭП',15670.00);
INSERT INTO stock_items VALUES (1,1,3,20);
INSERT INTO stock_items VALUES (2,1,4,20);
INSERT INTO stock_items VALUES (3,1,5,20);
INSERT INTO stock_items VALUES (4,1,6,20);
INSERT INTO stock_items VALUES (5,1,7,20);
INSERT INTO stock_items VALUES (6,1,8,20);
INSERT INTO stock_items VALUES (7,2,16,2);
INSERT INTO stock_items VALUES (8,2,17,2);
INSERT INTO stock_items VALUES (9,3,2,30);
INSERT INTO stock_items VALUES (10,3,3,30);
INSERT INTO stock_items VALUES (11,4,2,15);
INSERT INTO stock_items VALUES (12,4,3,15);
INSERT INTO stock_items VALUES (13,5,4,5);
INSERT INTO stock_items VALUES (14,5,5,5);
INSERT INTO stock_items VALUES (15,5,6,5);
INSERT INTO stock_items VALUES (16,6,1,4);
INSERT INTO stock_items VALUES (17,6,2,4);
INSERT INTO stock_items VALUES (18,6,3,4);
INSERT INTO stock_items VALUES (19,7,4,4);
INSERT INTO stock_items VALUES (20,7,5,4);
INSERT INTO stock_items VALUES (21,7,6,4);
INSERT INTO stock_items VALUES (22,7,7,4);
INSERT INTO stock_items VALUES (23,8,10,7);
INSERT INTO stock_items VALUES (24,8,11,7);
INSERT INTO stock_items VALUES (25,8,12,7);
INSERT INTO stock_items VALUES (26,8,13,7);
INSERT INTO stock_items VALUES (27,9,1,7);
INSERT INTO stock_items VALUES (28,9,2,7);
INSERT INTO stock_items VALUES (29,9,3,7);
INSERT INTO stock_items VALUES (30,10,19,12);
INSERT INTO stock_items VALUES (31,10,21,12);
INSERT INTO stock_items VALUES (32,11,25,2);
INSERT INTO stock_items VALUES (33,11,27,2);
INSERT INTO stock_items VALUES (34,12,21,5);
INSERT INTO stock_items VALUES (35,12,25,5);
INSERT INTO stock_items VALUES (36,12,27,5);
INSERT INTO stock_items VALUES (37,13,19,10);
INSERT INTO stock_items VALUES (38,13,21,10);
INSERT INTO stock_items VALUES (39,13,23,10);
INSERT INTO stock_items VALUES (40,13,25,10);
INSERT INTO stock_items VALUES (41,13,27,10);
INSERT INTO stock_items VALUES (42,14,18,3);
INSERT INTO stock_items VALUES (43,15,21,23);
INSERT INTO stock_items VALUES (44,15,25,23);
INSERT INTO stock_items VALUES (45,15,27,23);
INSERT INTO stock_items VALUES (46,16,19,25);
INSERT INTO stock_items VALUES (47,16,21,25);
INSERT INTO stock_items VALUES (48,16,23,25);
INSERT INTO stock_items VALUES (49,17,19,4);
INSERT INTO stock_items VALUES (50,17,23,4);
INSERT INTO stock_items VALUES (51,17,25,4);
INSERT INTO stock_items VALUES (52,17,27,4);
INSERT INTO stock_items VALUES (53,18,18,9);
INSERT INTO stock_items VALUES (54,18,23,9);
INSERT INTO stock_items VALUES (55,19,18,2);
INSERT INTO stock_items VALUES (56,19,19,2);
INSERT INTO stock_items VALUES (57,20,21,5);
INSERT INTO stock_items VALUES (58,20,22,5);
INSERT INTO stock_items VALUES (59,20,23,5);
INSERT INTO stock_items VALUES (60,20,24,5);
INSERT INTO stock_items VALUES (61,21,18,16);
INSERT INTO stock_items VALUES (62,21,19,16);
INSERT INTO stock_items VALUES (63,21,23,16);
INSERT INTO stock_items VALUES (64,21,25,16);
INSERT INTO stock_items VALUES (65,22,18,3);
INSERT INTO stock_items VALUES (66,22,23,3);
INSERT INTO stock_items VALUES (67,23,30,18);
INSERT INTO stock_items VALUES (68,23,31,18);
INSERT INTO stock_items VALUES (69,23,32,18);
INSERT INTO stock_items VALUES (70,23,33,18);
INSERT INTO stock_items VALUES (71,24,23,50);
INSERT INTO stock_items VALUES (72,24,24,50);
INSERT INTO stock_items VALUES (73,24,25,50);
INSERT INTO stock_items VALUES (74,24,26,50);
INSERT INTO stock_items VALUES (75,24,27,50);
INSERT INTO stock_items VALUES (76,24,29,50);
INSERT INTO stock_items VALUES (77,25,29,3);
INSERT INTO stock_items VALUES (78,25,31,3);
INSERT INTO stock_items VALUES (79,25,34,3);
INSERT INTO stock_items VALUES (80,26,27,23);
INSERT INTO stock_items VALUES (81,26,29,23);
INSERT INTO stock_items VALUES (82,26,33,23);
INSERT INTO stock_items VALUES (83,27,31,15);
INSERT INTO stock_items VALUES (84,27,33,15);
INSERT INTO stock_items VALUES (85,27,34,15);
INSERT INTO stock_items VALUES (86,28,34,3);
INSERT INTO stock_items VALUES (87,29,29,1);
INSERT INTO stock_items VALUES (88,29,31,1);
INSERT INTO stock_items VALUES (89,30,31,20);
INSERT INTO stock_items VALUES (90,30,33,20);
INSERT INTO stock_items VALUES (91,31,29,30);
INSERT INTO stock_items VALUES (92,31,31,30);
INSERT INTO orders VALUES (1,'2026-04-02 00:00:00','Сидорова','Анна','Дмитриевна',NULL,1);
INSERT INTO order_items VALUES (1,1,1,2190.00);
INSERT INTO order_items VALUES (1,20,1,2790.00);
INSERT INTO orders VALUES (2,'2026-04-05 00:00:00','Павлова','Елена','Петровна',NULL,1);
INSERT INTO order_items VALUES (2,30,1,25000.00);
INSERT INTO order_items VALUES (2,39,1,7500.00);
INSERT INTO orders VALUES (3,'2026-04-08 00:00:00','Васильева','Ольга','Игоревна',NULL,1);
INSERT INTO order_items VALUES (3,34,1,17000.00);
INSERT INTO order_items VALUES (3,57,1,8565.00);
INSERT INTO order_items VALUES (3,65,1,7795.00);
INSERT INTO orders VALUES (4,'2026-04-12 00:00:00','Лебедева','Ирина','Владимировна',NULL,1);
INSERT INTO order_items VALUES (4,50,1,7757.00);
INSERT INTO order_items VALUES (4,48,1,12340.00);
INSERT INTO order_items VALUES (4,44,1,7130.00);
INSERT INTO order_items VALUES (4,62,1,8765.00);
INSERT INTO order_items VALUES (4,32,1,15000.00);
INSERT INTO orders VALUES (5,'2026-04-18 00:00:00','Морозов','Сергей','Павлович',NULL,1);
INSERT INTO order_items VALUES (5,77,1,9567.00);
INSERT INTO order_items VALUES (5,87,1,21500.00);
INSERT INTO order_items VALUES (5,73,1,7890.00);
INSERT INTO orders VALUES (6,'2026-04-22 00:00:00','Михайлова','Татьяна','Юрьевна',NULL,1);
INSERT INTO order_items VALUES (6,80,1,24569.00);
INSERT INTO order_items VALUES (6,89,1,14567.00);
INSERT INTO order_items VALUES (6,91,1,15670.00);
INSERT INTO orders VALUES (7,'2026-04-25 00:00:00','Алексеев','Артём','Евгеньевич',NULL,1);
INSERT INTO order_items VALUES (7,16,1,4290.00);
INSERT INTO order_items VALUES (7,2,2,2190.00);
INSERT INTO order_items VALUES (7,24,1,1690.00);
INSERT INTO orders VALUES (8,'2026-04-28 00:00:00','Николаев','Владимир','Иванович',NULL,1);
INSERT INTO order_items VALUES (8,86,1,25625.00);
INSERT INTO order_items VALUES (8,75,3,7890.00);
INSERT INTO order_items VALUES (8,70,2,6550.00);
INSERT INTO orders VALUES (9,'2026-05-05 00:00:00','Смирнов','Дмитрий','Владимирович',NULL,1);
INSERT INTO order_items VALUES (9,7,1,2490.00);
INSERT INTO order_items VALUES (9,14,2,3190.00);
INSERT INTO orders VALUES (10,'2026-05-08 00:00:00','Павлова','Елена','Петровна',NULL,1);
INSERT INTO order_items VALUES (10,53,1,7550.00);
INSERT INTO order_items VALUES (10,43,1,7130.00);
INSERT INTO order_items VALUES (10,46,1,12340.00);
INSERT INTO order_items VALUES (10,61,2,8765.00);
COMMIT;
