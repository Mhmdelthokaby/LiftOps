export { logger } from "./logger";
export { parsePagination, getPrismaPagination, paginationSchema } from "./pagination";
export type { PaginationInput } from "./pagination";

import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatDate(dateString: string | Date | null | undefined): string {
  if (!dateString) return "N/A";
  try {
    const date = typeof dateString === "string" ? new Date(dateString) : dateString;
    if (isNaN(date.getTime())) return "N/A";
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = String(date.getFullYear());
    return `${day}/${month}/${year}`;
  } catch {
    return "N/A";
  }
}

export function formatDateInput(dateStr: string | Date): string {
  if (!dateStr) return "";
  try {
    let date: Date;
    if (dateStr instanceof Date) {
      date = dateStr;
    } else {
      const parts = dateStr.split("T")[0].split("-");
      if (parts.length === 3) {
        const year = parseInt(parts[0], 10);
        const month = parseInt(parts[1], 10) - 1;
        const day = parseInt(parts[2], 10);
        date = new Date(year, month, day);
      } else {
        date = new Date(dateStr);
      }
    }
    if (isNaN(date.getTime())) return "";
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = String(date.getFullYear());
    return `${day}/${month}/${year}`;
  } catch {
    return "";
  }
}

export function parseDateInput(dateStr: string): string {
  if (!dateStr) return "";
  try {
    const cleaned = dateStr.replace(/[^\d/]/g, "");
    const parts = cleaned.split("/");
    if (parts.length !== 3) return "";
    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10);
    let year = parseInt(parts[2], 10);
    if (year < 100) year = year < 50 ? 2000 + year : 1900 + year;
    if (isNaN(day) || isNaN(month) || isNaN(year)) return "";
    if (day < 1 || day > 31 || month < 1 || month > 12) return "";
    const date = new Date(year, month - 1, day);
    if (isNaN(date.getTime())) return "";
    if (date.getDate() !== day || date.getMonth() !== month - 1 || date.getFullYear() !== year) return "";
    const yyyy = String(year);
    const mm = String(month).padStart(2, "0");
    const dd = String(day).padStart(2, "0");
    return `${yyyy}-${mm}-${dd}`;
  } catch {
    return "";
  }
}

export function formatDateToLocalString(date: Date): string {
  if (!date || isNaN(date.getTime())) return "";
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

export function formatDateInputValue(value: string): string {
  const digits = value.replace(/\D/g, "");
  const limitedDigits = digits.slice(0, 8);
  if (limitedDigits.length <= 2) return limitedDigits;
  if (limitedDigits.length <= 4) return `${limitedDigits.slice(0, 2)}/${limitedDigits.slice(2)}`;
  return `${limitedDigits.slice(0, 2)}/${limitedDigits.slice(2, 4)}/${limitedDigits.slice(4, 8)}`;
}
