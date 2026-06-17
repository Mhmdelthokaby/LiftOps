export type {
  Company,
  AppUser,
  Subscription,
  SubscriptionPlan,
  Project,
  Customer,
  Stage,
  Elevator,
  Technician,
  MaintenanceContract,
  MaintenanceVisit,
  MaintenanceChecklist,
  Ticket,
  Item,
  ItemCategory,
  RefreshToken,
  UserRole,
  SubscriptionStatus,
  BillingCycle,
  ProjectStatus,
  ElevatorStatus,
  VisitStatus,
  TicketPriority,
  TicketStatus,
  EmergencyLevel,
} from "@prisma/client";

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  isActive: boolean;
  avatarUrl?: string | null;
  companyId: string;
}

export interface AuthResponse {
  user: UserDto;
  accessToken: string;
  refreshToken: string;
}

export interface TenantContext {
  companyId: string;
  userId: string;
  userRole: UserRole;
}
