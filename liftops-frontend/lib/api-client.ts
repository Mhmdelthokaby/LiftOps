// Centralized API client with automatic token refresh and error handling

import { API_BASE_URL } from "./api-config";
import { getValidToken, logout, isAuthenticated, isTokenExpired } from "./auth";
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
    const platformBypass = isPlatformApiDevBypass(endpoint);

    if (!platformBypass) {
        if (!isAuthenticated() && endpoint !== '/api/Admin/login') {
            logout();
            throw new ApiError(401, 'Unauthorized - Please login again');
        }
    }

    let token: string | null = null;
    if (endpoint === '/api/Admin/login') {
        token = null;
    } else if (platformBypass) {
        const raw = typeof window !== "undefined" ? localStorage.getItem("accessToken") : null;
        token = raw && !isTokenExpired(raw) ? raw : null;
    } else {
        token = await getValidToken();
    }

    if (!token && endpoint !== '/api/Admin/login' && !platformBypass) {
        logout();
        throw new ApiError(401, 'Unauthorized - Please login again');
    }

    const headers: Record<string, string> = {
        "Content-Type": "application/json",
        ...(options.headers as Record<string, string> || {}),
    };

    if (token && endpoint !== '/api/Admin/login') {
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
        if (endpoint !== '/api/Admin/login' && endpoint !== '/api/Admin/refresh-token') {
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
    
    // Backend uses Result wrapper: { succeeded, data, errors, message }
    // Check if it's a Result wrapper
    if (data && typeof data === 'object' && 'succeeded' in data) {
        // If succeeded is false, throw an error
        if (data.succeeded === false) {
            const errorMessage = data.errors && Array.isArray(data.errors) && data.errors.length > 0
                ? data.errors[0]
                : data.message || 'Request failed';
            throw new ApiError(response.status, errorMessage, data);
        }
        
        // If succeeded is true, return the data property
        if ('data' in data) {
            return data.data as T;
        }
    }
    
    // If not a Result wrapper, return the whole response
    return data as T;
};

