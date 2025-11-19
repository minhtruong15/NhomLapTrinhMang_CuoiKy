# NetChatSolution -- Ứng dụng Chat Đơn Giản

Ứng dụng chat realtime viết bằng **C# WinForms** theo mô hình **Client-- Server**.

## 📁 Các Project

-   **ClientApp**: Đăng nhập, Đăng ký, Danh sách bạn bè, ChatForm, Admin.
-   **ServerApp**: Server, ClientHandler, Database.
-   **Shared**: Packet, Command, Models (dùng chung giữa client và
    server).

## ⭐ Chức năng chính

-   Đăng nhập / Đăng ký
-   Gửi & nhận tin nhắn realtime 1--1
-   Lưu lịch sử tin nhắn vào MySQL
-   Hỗ trợ role: admin / user
-   Tin nhắn hiển thị dạng bong bóng
-   Âm thanh khi có tin nhắn mới

## 🗄️ Cấu trúc Database (MySQL)

    CREATE DATABASE IF NOT EXISTS chatdb CHARACTER SET utf8mb4;
    USE chatdb;

    CREATE TABLE users (
        id INT AUTO_INCREMENT PRIMARY KEY,
        username VARCHAR(50) UNIQUE NOT NULL,
        password VARCHAR(100) NOT NULL,
        role VARCHAR(10) DEFAULT 'user'
    );

    CREATE TABLE messages (
        id INT AUTO_INCREMENT PRIMARY KEY,
        sender VARCHAR(50),
        receiver VARCHAR(50),
        text MEDIUMTEXT,
        time DATETIME DEFAULT CURRENT_TIMESTAMP
    );

## ▶️ Cách chạy

1.  Mở **ServerApp** → Nhấn Start để chạy server\
2.  Mở **ClientApp** → Đăng nhập\
3.  Chọn bạn bè → Bắt đầu chat

## 🔧 Ghi chú

-   Chuỗi kết nối MySQL nằm trong: **ServerApp/Database.cs**
-   Sử dụng **MEDIUMTEXT** để lưu tin nhắn dài hoặc ảnh Base64.
