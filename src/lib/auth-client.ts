import { z } from "zod";

export const loginSchema = z.object({
    email: z.string().email("Invalid email address"),
    password: z.string().min(1, "Password is required"),
});

export type LoginFormData = z.infer<typeof loginSchema>;

export interface AuthResponse {
    token: string;
    refreshToken: string;
    refreshTokenExpiry: string;
    name: string;
    email: string;
    roles: string[];
}

import { API_BASE_URL } from "./api-config";

export const login = async (data: LoginFormData): Promise<AuthResponse> => {
    const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: data.email, password: data.password }),
    });

    if (!response.ok) {
        let errorMessage = "Login failed";
        try {
            const errorData = await response.json();
            errorMessage =
                (typeof errorData.message === "string" && errorData.message) ||
                (typeof errorData.code === "string" && errorData.code) ||
                errorMessage;
        } catch {
            // ignore JSON parse error
        }
        throw new Error(errorMessage);
    }

    return response.json();
};

export const loginAdmin = async (data: LoginFormData): Promise<AuthResponse> => {
    const response = await fetch(`/api/auth/admin/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: data.email, password: data.password }),
    });

    if (!response.ok) {
        let errorMessage = "Login failed";
        try {
            const errorData = await response.json();
            errorMessage =
                (typeof errorData.message === "string" && errorData.message) ||
                errorMessage;
        } catch {
            // ignore JSON parse error
        }
        throw new Error(errorMessage);
    }

    return response.json();
};

export const saveAuthDocs = (data: AuthResponse) => {
    if (typeof window !== 'undefined') {
        localStorage.setItem('accessToken', data.token);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('user', JSON.stringify({ name: data.name, email: data.email, roles: data.roles }));
    }
}

export type CurrentUser = { name: string; email: string; roles: string[] };

export const getCurrentUser = (): CurrentUser | null => {
    if (typeof window === "undefined") return null;
    const raw = localStorage.getItem("user");
    if (!raw) return null;
    try {
        const u = JSON.parse(raw) as { name?: string; email?: string; roles?: string[] };
        return {
            name: u.name ?? "",
            email: u.email ?? "",
            roles: Array.isArray(u.roles) ? u.roles : [],
        };
    } catch {
        return null;
    }
};

export const logout = (redirectTo: string = "/login") => {
    if (typeof window === "undefined") return;
    void fetch("/api/auth/logout", { method: "POST" }).finally(() => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");
        window.location.href = redirectTo;
    });
};

export const isTokenExpired = (token: string | null): boolean => {
    if (!token) return true;
    try {
        const parts = token.split('.');
        if (parts.length !== 3) return true;
        const payload = JSON.parse(atob(parts[1]));
        if (!payload.exp) return true;
        const expirationTime = payload.exp * 1000;
        const currentTime = Date.now();
        return currentTime >= (expirationTime - 5 * 60 * 1000);
    } catch (error) {
        console.error('Error checking token expiration:', error);
        return true;
    }
}

export const getTokenExpiration = (token: string | null): Date | null => {
    if (!token) return null;
    try {
        const parts = token.split('.');
        if (parts.length !== 3) return null;
        const payload = JSON.parse(atob(parts[1]));
        if (!payload.exp) return null;
        return new Date(payload.exp * 1000);
    } catch (error) {
        return null;
    }
}

export const isAuthenticated = (): boolean => {
    if (typeof window === 'undefined') return false;
    const token = localStorage.getItem('accessToken');
    if (!token) return false;
    return !isTokenExpired(token);
}

export const refreshToken = async (): Promise<AuthResponse | null> => {
    if (typeof window === 'undefined') return null;
    const token = localStorage.getItem('accessToken');
    const refreshTokenValue = localStorage.getItem('refreshToken');
    if (!token || !refreshTokenValue) {
        logout();
        return null;
    }
    try {
        const response = await fetch(`${API_BASE_URL}/api/Admin/refresh-token`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ token, refreshToken: refreshTokenValue }),
        });
        if (!response.ok) { logout(); return null; }
        const data: AuthResponse = await response.json();
        saveAuthDocs(data);
        return data;
    } catch (error) {
        console.error('Error refreshing token:', error);
        logout();
        return null;
    }
}

export const getValidToken = async (): Promise<string | null> => {
    if (typeof window === 'undefined') return null;
    const token = localStorage.getItem('accessToken');
    if (!token) { logout(); return null; }
    if (isTokenExpired(token)) {
        const refreshed = await refreshToken();
        if (refreshed) return refreshed.token;
        return null;
    }
    return token;
}
