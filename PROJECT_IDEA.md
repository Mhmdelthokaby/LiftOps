# 🎯 Full System Script Explanation

## Elevator Company Management System
### (.NET 10 Backend + Next.js Frontend)

---

## 🎙️ Introduction

This platform is a complete management system for an elevator company specializing in supplying, installing, inspecting, and maintaining electric elevators.

The system supports all business operations including:

- **Installation Projects**
- **Inventory & Spare Parts**
- **Maintenance Contracts**
- **Emergency Breakdowns**
- **Finance & Invoicing**
- **Role-Based Admin Management**
- **Technician Assignment & Reporting**

The goal is to deliver a fully automated workflow, reduce manual effort, and give managers full visibility on all operations.

---

## 🏗️ 1. System Architecture Overview

The platform is built using:

### Backend
- **.NET 10** (Clean Architecture + DDD + SOLID)
- **SQL Server**
- **JWT Authentication**
- **SignalR** for notifications
- **AutoMapper**
- **MediatR**

### Frontend
- **Next.js** (App Router)
- **Server Components + Client Components**
- **React Query / RTK Query** for API cache
- **Tailwind UI**
- **ShadCN**
- **NextAuth** (if required for JWT)
- **PDF rendering pages**
- **Dynamic dashboards**

### Infrastructure
- **Cloud storage** for contracts, PDFs, and invoices
- **Email service** for reminders
- **Background jobs** (Hangfire / Quartz)

---

## 👥 2. User & Role Management

### Main Roles

1. **Manager Admin** (Full Access)
2. **Installation Admin**
3. **Inventory Admin**
4. **Maintenance Admin**
5. **Breakdown Admin**
6. **Finance Admin**
7. **Technicians**

### Manager Admin Capabilities

- Create all admin accounts
- Assign & remove roles
- Disable/enable admins
- Reset passwords
- Change email/name/password for any admin
- View last login for all admins
- Full-read, full-write permission across all modules

---

## 🛠️ 3. Inventory Module (Inventory Admin)

### Purpose
Handles all spare parts operations within the company warehouse.

### Features

#### Add/Edit Spare Parts
Each part includes:
- ID
- Name
- Category
- Stock
- Price
- Supplier Name
- Uploaded By (Admin name)
- **No deletion** → parts can only be disabled

#### Inventory Admin Can:
- View full list
- View total stock value
- Filter by category, supplier, stock levels

#### System Triggers Alerts:
- Out-of-stock
- Low stock
- Requested parts (from Installation/Maintenance Admins)

---

## 🔧 4. Installation Module (Installation Admin)

This module manages client installation requests from contract creation to final project delivery.

### Client & Contract Entry

Installation Admin adds client information:
- Client name
- Project address
- Project number
- Phone
- Email
- Number of elevators
- Elevator type
- Building floors / stops
- Google Maps link
- Installation price per elevator
- Total price
- Contract date
- Expected start date
- Expected end date
- Client's site engineer (optional)
- Engineer phone (optional)
- Notes
- Installation Admin name

### Installation Stages (4 Stages)

Each elevator goes through 4 defined stages:

#### Stages 1–3: Supply & Install

Each stage includes:
- Stage start date
- Stage end date
- Stage status (Pending / In Progress / Success)
- Done by which admin
- Spare parts required for the stage
  - Ability to select parts even if stock = 0
  - System notifies Inventory Admin for missing parts
- After completing a stage → admin presses "Complete Stage" → system opens next stage.
- After finishing all 3 stages → stage 4 becomes available.

#### Stage 4: Final Delivery

Includes:
- Final handover
- Collecting all remaining payments
- Generating PDF delivery report
- Delivering the elevator to Maintenance Module
- Auto-add free maintenance months (based on contract)

### Multiple Elevators Support

If a client has multiple elevators:
- Each elevator gets its own 4-stage workflow
- Client shows as 1 profile → multiple elevator units under it

### Technician Assignment

Installation Admin (optional) can assign installation technicians.

**Manager Admin must first create technician profiles:**

Technician fields:
- Name
- Phone
- Total elevators completed
- Current workload
- Projects assigned
- Expected completion dates
- Rating

Installation Admin chooses technicians for each stage.

---

## 🧰 5. Maintenance Module (Maintenance Admin)

### Purpose
Manages monthly maintenance contracts & schedules.

### Maintenance Contract Entry

#### If a client wasn't part of installation:
Admin enters new maintenance contract:
- Client name
- Address
- Phone
- Number of elevators
- Project numbers
- Elevator descriptions
- Monthly maintenance price
- Maintenance status (Active, Paused, Frozen)
- Reason for freeze (if any)
- Freeze end date

#### If elevator was installed by the company:
System auto-applies:
- Free maintenance period (X months)
- Auto-switch to paid after free period ends

### Monthly Maintenance Scheduling

Maintenance Admin can:
- View monthly maintenance timetable
- Sort by date, area, priority
- Assign technicians
- Edit maintenance details
- Mark status:
  - Pending
  - In progress
  - Done
  - Frozen
- Add:
  - Notes
  - Spare parts used
  - Payment collected
  - Mark whether parts were delivered
  - Mark whether spare parts invoice was paid

### Parts Flow with Inventory

If admin selects parts but stock = 0:
- Still can select
- Inventory Admin gets notification
- Finance admin sees requested part cost

### Maintenance Reporting

- Elevator history report
- Monthly maintenance report
- Technician activity report
- Profit & cost details

---

## 🚨 6. Breakdown Module (Breakdown Admin)

### Purpose
Handles emergency issues & on-demand service calls.

### Breakdown Ticket Fields

- Ticket number
- Client name
- Phone
- Project address
- Google Maps link
- Description of issue
- Date & time
- Severity:
  - High
  - Medium
  - Low
- Status:
  - Pending
  - In progress
  - Done
- Assigned technician
- Spare parts used
- Payment status
- Notes

---

## 💰 7. Finance & Invoicing Module (Finance Admin)

### Purpose
Central module for all financial operations.

### Money In (Income)

Finance Admin sees:
- Installation payments
- Maintenance fees
- Breakdown service fees
- Spare parts sales
- Expected income for next month
- Unpaid invoices
- Monthly income analytics

### Money Out (Expenses)

Track:
- Spare parts purchased
- Technician salaries
- Supplier payments
- Logistics
- Installation expenses
- Maintenance expenses

### Invoicing

Finance Admin can generate:
- Installation invoices
- Maintenance monthly invoices
- Multi-month advance payments
- Breakdown invoices
- Spare parts invoices

#### Invoice includes:
- Invoice number
- Client information
- Service details
- Spare parts list
- Payment status
- Total amount
- Taxes (if applicable)

### Finance Dashboard

Shows:
- Total income
- Total expenses
- Net profit
- Monthly charts
- Spending distribution
- Spare parts profit
- Maintenance vs installation vs breakdown revenue

---

## 🚀 Conclusion

This system provides an end-to-end solution for managing all operations of an elevator company using:

- **.NET 10 backend**
- **Next.js frontend**
- **Modular roles**
- **Real-time notifications**
- **Full reporting & analytics**

It enhances efficiency, reduces manual work, and ensures smooth coordination between Installation, Maintenance, Inventory, Breakdown, and Finance departments.

---

## 📋 Testing Guidelines

### For Backend Team:
- Test all API endpoints for each module
- Verify role-based authorization
- Test JWT authentication flow
- Validate SignalR notifications
- Test database transactions and data integrity
- Verify AutoMapper mappings
- Test MediatR handlers

### For Frontend Team:
- Test all UI components and pages
- Verify role-based UI rendering
- Test API integration with React Query/RTK Query
- Validate form submissions and error handling
- Test PDF generation and rendering
- Verify real-time notifications via SignalR
- Test responsive design across devices
- Validate authentication flow and protected routes

---

**Document Version:** 1.0  
**Last Updated:** 2024

