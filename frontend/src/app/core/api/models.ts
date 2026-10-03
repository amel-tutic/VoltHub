// TypeScript mirrors of the API's JSON contracts. Enums travel as strings (JsonStringEnumConverter).

export type Role = 'User' | 'Operator' | 'Admin';
export type ConnectorType = 'Type1' | 'Type2' | 'CCS' | 'CHAdeMO' | 'Tesla';
export type CurrentType = 'AC' | 'DC';
export type ChargerStatus = 'Available' | 'Occupied' | 'Reserved' | 'OutOfOrder' | 'UnderMaintenance';
export type ReservationStatus = 'Active' | 'Completed' | 'Cancelled' | 'Expired';
export type SessionStatus = 'InProgress' | 'Completed';
export type InvoiceStatus = 'Pending' | 'Paid' | 'Cancelled';
export type PaymentMethod = 'Card' | 'EWallet' | 'Subscription';
export type PaymentStatus = 'Pending' | 'Completed' | 'Failed';
export type MaintenanceType = 'Fault' | 'Repair' | 'ScheduledService' | 'Inspection';
export type ProblemType = 'OccupiedParkingSpot' | 'FaultyCable' | 'ChargingStartFailure' | 'Other';
export type ProblemStatus = 'Open' | 'InProgress' | 'Resolved';
export type NotificationType = 'MaintenanceDue' | 'ChargerOffline' | 'NewProblemReport';

export const CONNECTOR_TYPES: ConnectorType[] = ['Type1', 'Type2', 'CCS', 'CHAdeMO', 'Tesla'];
export const OPERATOR_STATUSES: ChargerStatus[] = ['Available', 'OutOfOrder', 'UnderMaintenance'];
export const PAYMENT_METHODS: PaymentMethod[] = ['Card', 'EWallet', 'Subscription'];
export const PROBLEM_TYPES: ProblemType[] = ['ChargingStartFailure', 'FaultyCable', 'OccupiedParkingSpot', 'Other'];
export const PROBLEM_TYPE_LABELS: Record<ProblemType, string> = {
  ChargingStartFailure: 'Charging does not start',
  FaultyCable: 'Faulty cable or plug',
  OccupiedParkingSpot: 'Parking spot occupied',
  Other: 'Other'
};

export interface AuthResponse { accessToken: string; accessTokenExpiresAt: string; refreshToken: string; role: Role; }
export interface UserProfile { id: string; firstName: string; lastName: string; email: string; role: Role; }

export interface StationSummary {
  id: string; name: string; address: string; city: string; latitude: number; longitude: number;
  chargerCount: number; availableChargerCount: number; averageRating: number | null;
}
export interface Charger {
  id: string; code: string; connectorType: ConnectorType; currentType: CurrentType;
  powerKw: number; pricePerKwh: number; status: ChargerStatus;
}
export interface StationDetails {
  id: string; name: string; address: string; city: string; latitude: number; longitude: number;
  description: string | null; averageRating: number | null; ratingCount: number; chargers: Charger[];
}
export interface NearestStation {
  id: string; name: string; address: string; city: string; latitude: number; longitude: number;
  availableChargers: number; distanceKm: number;
}
export interface BusySlot { startTime: string; endTime: string; }

export interface Vehicle { id: string; make: string; model: string; batteryCapacityKwh: number; connectorType: ConnectorType; }

export interface Reservation {
  id: string; startTime: string; endTime: string; status: ReservationStatus;
  chargerId: string; chargerCode: string; stationId: string; stationName: string; vehicleId: string; vehicleName: string;
}

export interface ActiveSession {
  sessionId: string; reservationId: string; stationName: string; chargerCode: string; vehicleName: string;
  startedAt: string; elapsedMinutes: number; estimatedEnergyKwh: number; pricePerKwh: number; estimatedCost: number;
  reservationEndsAt: string;
}
export interface SessionSummary {
  sessionId: string; startedAt: string; endedAt: string; energyKwh: number; pricePerKwh: number; totalPrice: number;
  invoiceId: string; invoiceNumber: string;
}
export interface SessionHistoryItem {
  sessionId: string; startedAt: string; endedAt: string | null; status: SessionStatus;
  stationName: string; chargerCode: string; vehicleName: string; energyKwh: number | null; totalPrice: number | null;
}

export interface Invoice {
  id: string; invoiceNumber: string; amount: number; issuedAt: string; status: InvoiceStatus;
  sessionId: string; stationName: string; paidAt: string | null;
}
export interface PaymentResult {
  paymentId: string; invoiceId: string; invoiceNumber: string; amount: number; method: PaymentMethod;
  status: PaymentStatus; paidAt: string | null;
}

// ---- Accounts (SSA 1.3, 1.5)
export interface AdminUser {
  id: string; firstName: string; lastName: string; email: string; role: Role; isActive: boolean; createdAt: string; vehicleCount: number;
}

// ---- Maintenance and notifications (SSA 7)
export interface MaintenanceRecord {
  id: string; type: MaintenanceType; description: string; reportedAt: string; scheduledDate: string | null; resolvedAt: string | null;
}
export interface ChargerMaintenance {
  chargerId: string; chargerCode: string; stationId: string; stationName: string; status: ChargerStatus; statusChangedAt: string;
  lastServiceAt: string | null; nextServiceAt: string | null; records: MaintenanceRecord[];
}
export interface OpenMaintenanceItem {
  id: string; type: MaintenanceType; description: string; reportedAt: string; scheduledDate: string | null;
  chargerId: string; chargerCode: string; stationId: string; stationName: string; chargerStatus: ChargerStatus;
}
// Named AppNotification because the browser already has a global type called Notification.
export interface AppNotification { id: string; type: NotificationType; title: string; message: string; isRead: boolean; createdAt: string; }
export interface NotificationList { unreadCount: number; items: AppNotification[]; }

// ---- Ratings, problem reports and statistics (SSA 8)
export interface StationRating { author: string; score: number; comment: string | null; createdAt: string; }
export interface StationRatings {
  average: number | null; count: number; mine: { score: number; comment: string | null } | null; items: StationRating[];
}
export interface ProblemReport {
  id: string; type: ProblemType; description: string; status: ProblemStatus; createdAt: string; resolvedAt: string | null;
  chargerId: string; chargerCode: string; stationId: string; stationName: string; reporterName: string;
}
export interface MyStatistics {
  sessionCount: number; totalEnergyKwh: number; totalCost: number; averageSessionMinutes: number;
  topStations: { stationId: string; stationName: string; sessions: number; energyKwh: number }[];
  monthly: { month: string; sessions: number; energyKwh: number; cost: number }[];
}

// ---- Administrator reports (SSA 9)
export interface OverviewReport {
  owners: number; activeOwners: number; operators: number; admins: number; vehicles: number;
  stations: number; chargers: number; chargersOutOfOrder: number; activeReservations: number; completedSessions: number;
}
export interface StationUsageReport {
  stationId: string; stationName: string; city: string; chargerCount: number; sessions: number; energyKwh: number; revenue: number;
  averageSessionMinutes: number; utilizationPercent: number;
}
export interface RevenueReport {
  from: string; to: string; sessions: number; energyKwh: number; billed: number; paid: number; outstanding: number;
  byDay: { day: string; sessions: number; energyKwh: number; revenue: number }[];
}
export interface FaultReport {
  chargerId: string; chargerCode: string; stationName: string; status: ChargerStatus; faults: number; openFaults: number; lastFaultAt: string | null;
}

export interface CreatedResponse { id: string; }

// RFC 7807 problem response returned by the API for every error.
export interface ProblemDetails { title?: string; detail?: string; status?: number; errors?: Record<string, string[]>; }
