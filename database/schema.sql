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
