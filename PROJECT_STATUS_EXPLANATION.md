# 📊 Project Status System Explanation

## 🔧 Installation Projects Status

### How Status is Determined

Installation projects have a **calculated status** based on:
1. **ProjectStatus enum** (stored in database)
2. **Stage statuses** (for elevators within the project)

### ProjectStatus Enum Values:
- `UnderInspectionAndQuotation` (0) → Shows as **"Pending"**
- `Approved` (1) → Shows as **"Active"** or **"InProgress"** (based on stages)
- `Rejected` (2) → Shows as **"Rejected"**
- `Active` (3) → Shows as **"Active"** or **"InProgress"** (based on stages)

### When Status Becomes "InProgress"

A project shows **"InProgress"** when:
- The project has elevators with stages
- **At least one stage** has status `InProgress` (StageStatus.InProgress)
- This happens when you **start a stage** using the "Start Stage" button

### Status Calculation Logic:

```
1. If ProjectStatus = UnderInspectionAndQuotation → "Pending"
2. If ProjectStatus = Rejected → "Rejected"
3. If ProjectStatus = Approved or Active:
   - Check all elevators' stages
   - If ANY stage is InProgress → "InProgress"
   - If ALL stages are Success → "Completed"
   - Otherwise → "Active"
```

### Stage Statuses (for each elevator stage):
- `Pending` - Stage not started yet
- `InProgress` - Stage is currently being worked on
- `Success` - Stage completed successfully

### Example Flow:
1. **Create Project** → Status: `UnderInspectionAndQuotation` → Shows: **"Pending"**
2. **Approve Project** → Status: `Approved` → Shows: **"Active"** (no stages started)
3. **Start Stage 1** → Stage status: `InProgress` → Project shows: **"InProgress"**
4. **Complete Stage 1** → Stage status: `Success` → Project shows: **"Active"** (if no other stage is InProgress)
5. **Start Stage 2** → Stage status: `InProgress` → Project shows: **"InProgress"** again
6. **Complete All Stages** → All stages: `Success` → Project shows: **"Completed"**

---

## 🔧 Maintenance Projects Status

### Maintenance Contract Status

Maintenance contracts have a **direct status** (not calculated):

#### MaintenanceContractStatus Enum:
- `Active` (0) → Contract is active and running
- `Pending` (1) → Contract is pending activation
- `Frozen` (2) → Contract is temporarily frozen
- `Cancelled` (3) → Contract is cancelled
- `Expired` (4) → Contract has expired

### Maintenance Elevator Status

Each elevator within a maintenance contract has its own status:

#### MaintenanceElevatorStatus Enum:
- `Active` (0) → Elevator is actively maintained
- `Frozen` (1) → Elevator maintenance is frozen
- `Stopped` (2) → Elevator maintenance is stopped

### How They Work Together:

1. **Contract Level**: The contract can be Active, Pending, Frozen, Cancelled, or Expired
2. **Elevator Level**: Each elevator can be Active, Frozen, or Stopped (independent of contract status)

### Status Management:

- **Freeze Contract**: Sets contract status to `Frozen` (affects all elevators)
- **Stop Contract**: Sets contract status to `Cancelled`
- **Activate Contract**: Sets contract status to `Active`

- **Freeze Elevator**: Sets individual elevator status to `Frozen`
- **Stop Elevator**: Sets individual elevator status to `Stopped`
- **Activate Elevator**: Sets individual elevator status to `Active`

### Important Notes:

- Elevator statuses are **independent** of contract status
- You can have an `Active` contract with `Frozen` or `Stopped` elevators
- You can have a `Cancelled` contract, but elevators still maintain their individual statuses
- The frontend shows elevator statuses, not contract statuses (as per your requirements)

---

## 📝 Summary

### Installation Projects:
- Status is **calculated** from ProjectStatus enum + Stage statuses
- Becomes **"InProgress"** when any stage is started
- Becomes **"Completed"** when all stages are completed
- Status changes automatically based on stage actions

### Maintenance Projects:
- Contract status is **direct** (stored value)
- Elevator status is **direct** (stored value)
- Statuses are **independent** - contract and elevators can have different statuses
- Status changes require explicit actions (Freeze/Stop/Activate buttons)

