// Centralized API client with automatic token refresh and error handling

import { API_BASE_URL } from "./api-config";
import { getValidToken, logout, isAuthenticated, isTokenExpired } from "./auth-client";
import { skipLoginForAdminRoutes } from "./navigation";

function isPlatformApiDevBypass(endpoint: string): boolean {
  return (
    typeof window !== "undefined" &&
    skipLoginForAdminRoutes() &&
    endpoint.startsWith("/api/platform")
  );
}

// Custom error class for API errors
export class ApiError extends Error {
    constructor(
        public status: number,
        public message: string,
        public data?: any
    ) {
        super(message);
        this.name = 'ApiError';
    }
}

// Centralized fetch wrapper with auth and error handling
export const apiClient = async (
    endpoint: string,
    options: RequestInit = {}
): Promise<Response> => {
    const originalEndpoint = endpoint;
    endpoint = mapEndpoint(endpoint);
    const platformBypass = isPlatformApiDevBypass(originalEndpoint);

    const isLoginEndpoint = endpoint === '/api/auth/login' || originalEndpoint === '/api/Admin/login';

    if (!platformBypass) {
        if (!isAuthenticated() && !isLoginEndpoint) {
            logout();
            throw new ApiError(401, 'Unauthorized - Please login again');
        }
    }

    let token: string | null = null;
    if (isLoginEndpoint) {
        token = null;
    } else if (platformBypass) {
        const raw = typeof window !== "undefined" ? localStorage.getItem("accessToken") : null;
        token = raw && !isTokenExpired(raw) ? raw : null;
    } else {
        token = await getValidToken();
    }

    if (!token && !isLoginEndpoint && !platformBypass) {
        logout();
        throw new ApiError(401, 'Unauthorized - Please login again');
    }

    const headers: Record<string, string> = {
        "Content-Type": "application/json",
        ...(options.headers as Record<string, string> || {}),
    };

    if (token && !isLoginEndpoint) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    // Make the request
    let response: Response;
    try {
        response = await fetch(`${API_BASE_URL}${endpoint}`, {
            ...options,
            headers,
        });
    } catch (error: any) {
        // Handle network errors (backend not running, CORS, etc.)
        const errorMessage = error?.message || 'Unknown network error';
        console.error(`Network error fetching ${API_BASE_URL}${endpoint}:`, errorMessage);
        
        // Provide helpful error message
        if (errorMessage.includes('Failed to fetch') || errorMessage.includes('NetworkError')) {
            throw new ApiError(
                0,
                `Unable to connect to the API server at ${API_BASE_URL}. Please ensure the backend server is running.`,
                { originalError: errorMessage, endpoint, apiUrl: API_BASE_URL }
            );
        }
        
        throw new ApiError(0, `Network error: ${errorMessage}`, { originalError: errorMessage, endpoint });
    }

    // Handle 401 Unauthorized - token expired or invalid
    if (response.status === 401) {
        if (platformBypass && !token) {
            return response;
        }
        // Try to refresh token once
        const isRefreshEndpoint = endpoint === '/api/Admin/refresh-token' || originalEndpoint === '/api/Admin/refresh-token';
        if (!isLoginEndpoint && !isRefreshEndpoint) {
            const refreshed = await getValidToken();
            if (refreshed) {
                // Retry the request with new token
                const retryHeaders: Record<string, string> = {
                    ...headers,
                    "Authorization": `Bearer ${refreshed}`,
                };
                let retryResponse: Response;
                try {
                    retryResponse = await fetch(`${API_BASE_URL}${endpoint}`, {
                        ...options,
                        headers: retryHeaders,
                    });
                } catch (error: any) {
                    const errorMessage = error?.message || 'Unknown network error';
                    console.error(`Network error retrying ${API_BASE_URL}${endpoint}:`, errorMessage);
                    throw new ApiError(
                        0,
                        `Unable to connect to the API server at ${API_BASE_URL}. Please ensure the backend server is running.`,
                        { originalError: errorMessage, endpoint, apiUrl: API_BASE_URL }
                    );
                }
                
                if (retryResponse.status === 401) {
                    if (!platformBypass) {
                        logout();
                    }
                    throw new ApiError(401, 'Session expired - Please login again');
                }
                
                return retryResponse;
            } else {
                if (!platformBypass) {
                    logout();
                }
                throw new ApiError(401, 'Session expired - Please login again');
            }
        } else {
            if (!platformBypass) {
                logout();
            }
            throw new ApiError(401, 'Unauthorized - Please login again');
        }
    }

    return response;
};

// Map frontend API paths (from .NET era) to our Next.js backend paths
const pathMap: Record<string, string> = {
  "/api/dashboard/summary": "/api/dashboard",
  "/api/customers": "/api/installation/customers",
  "/api/category/list": "/api/categories",
  "/api/category/add": "/api/categories",
  "/api/inventory/all": "/api/inventory/items",
  "/api/inventory/active": "/api/inventory/items",
  "/api/inventory/add": "/api/inventory/items",
  "/api/Technician/all": "/api/installation/technicians",
  "/api/Technician/available": "/api/installation/technicians",
  "/api/Technician/add": "/api/installation/technicians",
  "/api/subscription-plans": "/api/subscription/plans",
  "/api/platform/dashboard": "/api/dashboard",
};

function mapEndpoint(endpoint: string): string {
  // Exact match first
  if (pathMap[endpoint]) return pathMap[endpoint];

  // Pattern-based matching for paths with IDs
  const patterns: [RegExp, string][] = [
    [/^\/api\/Admin\/login/, "/api/auth/login"],
    [/^\/api\/Admin\/(.+)$/, "/api/admin/$1"],
    [/^\/api\/customers\/(.+)$/, "/api/installation/customers/$1"],
    [/^\/api\/customers/, "/api/installation/customers"],
    [/^\/api\/inventory\/update\/(.+)$/, "/api/inventory/items/$1"],
    [/^\/api\/inventory\/disable\/(.+)$/, "/api/inventory/items/$1"],
    [/^\/api\/Technician\/update\/(.+)$/, "/api/installation/technicians/$1"],
    [/^\/api\/Technician\/disable\/(.+)$/, "/api/installation/technicians/$1"],
    [/^\/api\/Technician\/delete\/(.+)$/, "/api/installation/technicians/$1"],
  ];

  for (const [regex, replacement] of patterns) {
    if (regex.test(endpoint)) {
      return endpoint.replace(regex, replacement);
    }
  }

  return endpoint;
}

// Helper to parse JSON response with error handling
export const parseResponse = async <T>(response: Response): Promise<T> => {
    if (!response.ok) {
        let errorMessage = `Request failed with status ${response.status}`;
        let errorData = null;
        
        // Handle specific status codes with user-friendly messages
        if (response.status === 403) {
            errorMessage = "You don't have permission to access this resource. Please contact your administrator.";
        } else if (response.status === 401) {
            errorMessage = "Your session has expired. Please log in again.";
        } else if (response.status === 404) {
            errorMessage = "The requested resource was not found.";
        }
        
        try {
            const data = await response.json();
            // Backend uses Result wrapper: { succeeded, data, errors, message }
            if (data.errors && Array.isArray(data.errors) && data.errors.length > 0) {
                // Use first error, but keep status-specific message if it's more informative
                if (response.status !== 403 && response.status !== 401 && response.status !== 404) {
                    errorMessage = data.errors[0] || errorMessage;
                }
            } else if (data.message && response.status !== 403 && response.status !== 401 && response.status !== 404) {
                errorMessage = data.message || errorMessage;
            }
            errorData = data;
        } catch {
            // If response is not JSON, use status text (but keep our friendly message for common statuses)
            if (response.status !== 403 && response.status !== 401 && response.status !== 404) {
                errorMessage = response.statusText || errorMessage;
            }
        }
        
        throw new ApiError(response.status, errorMessage, errorData);
    }
    
    const data = await response.json();
    
    // Check for both .NET Result wrapper ({ succeeded, data, errors, message })
    // and our backend format ({ success, data, message, errors })
    const isSuccess = data && typeof data === 'object' && (
        ('succeeded' in data && data.succeeded === true) ||
        ('success' in data && data.success === true)
    );
    const isFailure = data && typeof data === 'object' && (
        ('succeeded' in data && data.succeeded === false) ||
        ('success' in data && data.success === false)
    );

    if (isFailure) {
        const errorMessage = data.errors
            ? (Array.isArray(data.errors) ? data.errors[0] : String(data.errors))
            : data.message || 'Request failed';
        throw new ApiError(response.status, errorMessage, data);
    }

    if (isSuccess && 'data' in data) {
        return data.data as T;
    }
    
    return data as T;
};

