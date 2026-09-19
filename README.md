# EventFlow: Attendee & Event Management System (EF Core)

EventFlow is a robust management system built using **ASP.NET Core** and **Entity Framework Core**. This project focuses on a **Master-Detail** architecture to handle event registrations, where an Attendee can purchase multiple tickets for various events in a single transaction.

## 🌟 Features
- **Master-Detail Registration:** Seamlessly register an Attendee (Master) and their chosen Events (Details) at once.
- **Bulk Ticket Purchase:** Logic to allow attendees to buy as many tickets as they want for multiple events.
- **EF Core Power:** Uses **LINQ** and **Eager Loading** (Include/ThenInclude) for efficient data retrieval.
- **Relational Integrity:** Implements a strong relational schema between `Attendee`, `Event`, and `Registration` tables.
- **Store Proceedure:** Implement store procedure for each action on `attendees` controller
- **Transaction Management:** Ensures data consistency so that a registration is only saved if all ticket entries are valid.
- **Aggregate Function:** I have use all the aggrigate function on this project to filter which event has the the maximum and minimum number of ticket perchese.

## 🛠️ Tech Stack
- **Framework:** ASP.NET Core
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Design Pattern:** Repository Pattern & ViewModel-based Data Transfer.

## 📊 Database Relationship
* **Attendee (Master):** Stores personal information of the participant.
* **Event:** Contains details of available events, pricing, and schedules.
* **Registration (Relational/Junction):** Tracks which attendee bought tickets for which event and the quantity purchased.

## 🚀 Setup & Installation

### 1. Clone the Project
### 2. Open it with Visual Studio 2022
### 3. Download packeges from nuget packege maneger.
### 4.Open `Packege maneger Console`
<p>Command<b>update-database</b></p>
### 4.open Store Procedure for EF_core_project.sql file on your sql server and execute it

<p>After all tese step run it from Visual studio and insert event data first then make e attndee registration with multiple event registration</p>
