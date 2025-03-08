# Discord Clone - Built with ASP.NET 8 & Next.js

This is a **Discord-like** real-time chat application built using **ASP.NET 8** and **Next.js**, following **Onion Architecture**. It supports real-time messaging, authentication, file uploads, and more.

## ✨ Features

### 🔑 Authentication
- JWT Bearer authentication with **refresh token** (sent securely).
- **EF Identity** for user management.
- **Email verification & password reset** using **MailKit**.

### 💬 Messaging & Direct Messages
- **Send & delete messages** (soft & hard delete).
- **Edit messages**.
- **Real-time messaging** with **SignalR**.
  
### 📢 Server & Channel Management
- **Create, update, and delete** servers.
- **Generate invite codes** for servers.
- **Manage channels and members**.

### 📁 File Uploading
- **Cloudinary** integration for image and file uploads.

### 🔄 Other Features
- **User Profile Management** (update profile, change password).
- **Conversation & DM system**.
- **Unit of Work & Generic Repository Pattern**.
- **AutoMapper** for efficient data mapping.

## 🛠️ Tech Stack

- **Backend**: ASP.NET 8, SignalR, EF Identity, JWT Bearer, SQL Server.
- **Frontend**: Next.js 15 (with Turbopack).
- **Storage**: Cloudinary (for file uploads).
- **Email Service**: MailKit (for email verification & password reset).
- **Patterns Used**: Onion Architecture, Unit of Work, Generic Repository.


### Prerequisites
- .NET 8 SDK
- SQL Server
- Cloudinary Account (for file uploads)
- SMTP Credentials (for email service)