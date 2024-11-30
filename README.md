# School Management System - ASP.NET Core Web API & MVC  

## Overview  
This project is a **School Management System** built with **ASP.NET Core Web API** and **MySQL** to manage teacher records and their assigned courses. It provides basic functionalities for adding, retrieving, and deleting teacher records, and also for viewing which courses are assigned to teachers.

---

## Features  
- **Teacher Management**  
  - Get a list of all teachers.
  - Find a specific teacher by their ID, including their assigned courses.
  - Add a new teacher.
  - Delete an existing teacher.

- **Course Assignment**  
  - View which courses a teacher is assigned to.
  - Handle cases where a teacher is not assigned any courses.

- **Security**  
  - Prevent SQL injection through the use of parameterized queries.

---

## Getting Started  

### Prerequisites  
Before getting started, make sure you have the following installed:
- **.NET SDK**  
- **MySQL Server**

### Installation Steps  
1. **Clone the repository** to your local machine.
2. **Set up the MySQL database** and update the connection string in the project.
3. **Run the application** to start the API.

---

## API Endpoints  

- **Get All Teachers**: Retrieves a list of all teachers.
- **Find Teacher by ID**: Retrieves details of a specific teacher and their assigned courses.
- **Add a Teacher**: Adds a new teacher to the database.
- **Delete a Teacher**: Deletes a teacher by their ID.

---

## Technology Stack  
- **Backend**: ASP.NET Core Web API  
- **Database**: MySQL  
- **Language**: C#  

---

## Security & Best Practices  
- **Sanitized Inputs**: Protects the app from SQL injection using parameterized queries.
- **Separation of Concerns**: Keeps the project organized by separating controllers, models, and database logic.

---

## Future Enhancements  
- Implement the **Update functionality** for teacher records.
- Expand the system to manage **students** and **classes**.
- Add a **frontend interface** (e.g., using React or Angular).
- Improve **error handling** and add **logging**.

