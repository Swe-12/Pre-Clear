import { useState, useEffect } from 'react';
import { shipmentApi } from '../services/shipmentApi';

export function useBrokerShipments() {
  const [shipments, setShipments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Fetch shipments assigned to this broker
  const fetchBrokerShipments = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Get broker ID from localStorage
      const userStr = localStorage.getItem('user');
      const user = userStr ? JSON.parse(userStr) : null;
      const brokerId = user?.id || 1;
      
      console.log('📡 Fetching shipments for broker:', brokerId);
      
      // Fetch from backend API endpoint: /api/brokers/{brokerId}/shipments
      const response = await fetch(
        `http://localhost:5232/api/brokers/${brokerId}/shipments`,
        {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          },
          credentials: 'omit' // Don't include cookies, use header auth only
        }
      );

      if (!response.ok) {
        if (response.status === 401) {
          throw new Error('Unauthorized - please log in again');
        } else if (response.status === 404) {
          console.warn('⚠️ Broker endpoint not found or no shipments');
          setShipments([]);
          return;
        }
        throw new Error(`Failed to fetch: ${response.status} ${response.statusText}`);
      }

      const data = await response.json();
      console.log('✅ Broker shipments fetched:', data);
      
      if (Array.isArray(data)) {
        setShipments(data);
      } else if (data && Array.isArray(data.shipments)) {
        setShipments(data.shipments);
      } else {
        console.warn('Unexpected response format:', data);
        setShipments([]);
      }
    } catch (err) {
      console.error('❌ Failed to fetch broker shipments:', err);
      setError(err.message);
      setShipments([]);
    } finally {
      setLoading(false);
    }
  };

  // Fetch on mount
  useEffect(() => {
    fetchBrokerShipments();
  }, []);

  return {
    shipments,
    loading,
    error,
    refetch: fetchBrokerShipments
  };
}
