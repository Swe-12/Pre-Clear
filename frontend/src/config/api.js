// Centralized API configuration
// Use environment variable if available, otherwise use localhost:5232 in development

export const API_BASE_URL = 
  import.meta.env.VITE_API_BASE_URL || 
  (import.meta.env.DEV ? 'http://localhost:5232/api' : '/api');

console.log('🔧 API Base URL configured to:', API_BASE_URL);

// Helper to get auth token
export const getAuthToken = () => {
  return localStorage.getItem('token') || localStorage.getItem('authToken');
};

// Helper to build full API URL
export const buildApiUrl = (path) => {
  if (path.startsWith('http')) return path;
  const cleanPath = path.startsWith('/') ? path : `/${path}`;
  return `${API_BASE_URL}${cleanPath}`;
};
