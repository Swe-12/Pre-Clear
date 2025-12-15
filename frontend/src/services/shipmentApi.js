// Backend API service for shipments
import { API_BASE_URL, getAuthToken } from '../config/api.js';

export const shipmentApi = {
  // Fetch single shipment by ID from backend
  async getShipmentById(id) {
    try {
      // Convert string ID to number if needed
      const numericId = typeof id === 'string' ? parseInt(id.split('-')[1] || id) : id;
      const url = `${API_BASE_URL}/shipments/details/${numericId}`;
      const token = getAuthToken();
      
      console.group('🔄 FETCH SHIPMENT REQUEST');
      console.log('📍 URL:', url);
      console.log('📋 Method:', 'GET');
      console.log('🔑 Auth Token Present:', !!token);
      console.groupEnd();
      
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          ...(token && { 'Authorization': `Bearer ${token}` })
        },
      });

      console.group('🔄 FETCH SHIPMENT RESPONSE');
      console.log('📊 Status:', response.status);
      console.log('📋 Status Text:', response.statusText);
      console.groupEnd();

      if (!response.ok) {
        if (response.status === 404) {
          console.warn(`⚠️ Shipment ${numericId} not found in backend`);
          return null;
        }
        const errorText = await response.text();
        throw new Error(`Failed to fetch shipment: ${response.statusText} - ${errorText}`);
      }

      const data = await response.json();
      console.log('✅ Shipment fetched from backend:', data);
      return data;
    } catch (error) {
      console.error(`❌ Error fetching shipment ${id}:`);
      console.error('   Message:', error.message);
      console.error('   Stack:', error.stack);
      return null;
    }
  },

  // Fetch all shipments for a user
  async getShipmentsByUser(userId) {
    try {
      const response = await fetch(`${API_BASE_URL}/shipments/${userId}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${getAuthToken()}`
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch shipments: ${response.statusText}`);
      }

      const data = await response.json();
      console.log('✅ Shipments fetched from backend:', data);
      return data;
    } catch (error) {
      console.error('Error fetching shipments:', error);
      return [];
    }
  },

  // Create a new shipment with proper DTO structure
  async createShipment(formData, userId = 1) {
    try {
      // Transform frontend form data to backend CreateShipmentRequest DTO
      const payload = {
        userId: userId,
        productName: formData.productName || formData.title || 'Shipment',
        quantity: parseFloat(formData.quantity || formData.packages?.[0]?.products?.[0]?.qty || 1),
        invoiceValue: parseFloat(formData.value || formData.customsValue || 0),
        hsCode: formData.hsCode || formData.packages?.[0]?.products?.[0]?.hsCode,
        currency: formData.currency || 'USD',
        totalValue: parseFloat(formData.value || formData.customsValue || 0),
        totalWeight: parseFloat(formData.weight || 0),
        shipperId: formData.shipperId || 1,
        shipperName: formData.shipper?.company || formData.shipperName || 'Shipper',
        // Include items if available
        items: formData.packages?.flatMap(pkg => 
          pkg.products?.map(prod => ({
            productName: prod.name || 'Product',
            description: prod.description,
            hsCode: prod.hsCode,
            quantity: prod.qty || 1,
            weight: prod.weight,
            unitPrice: prod.unitPrice
          })) || []
        ),
      };

      const url = `${API_BASE_URL}/shipments/create`;
      const token = getAuthToken();
      
      console.group('🔄 SHIPMENT API REQUEST');
      console.log('📍 URL:', url);
      console.log('📋 Method:', 'POST');
      console.log('🔑 Auth Token Present:', !!token);
      console.log('📤 Payload:', JSON.stringify(payload, null, 2));
      console.log('📊 Payload Type Check:', {
        userId: typeof payload.userId,
        productName: typeof payload.productName,
        quantity: typeof payload.quantity,
        invoiceValue: typeof payload.invoiceValue,
      });
      console.groupEnd();

      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token && { 'Authorization': `Bearer ${token}` })
        },
        body: JSON.stringify(payload),
      });

      console.group('🔄 SHIPMENT API RESPONSE');
      console.log('📊 Status:', response.status);
      console.log('📋 Status Text:', response.statusText);
      console.log('📦 Content-Type:', response.headers.get('content-type'));
      console.groupEnd();

      if (!response.ok) {
        const errorText = await response.text();
        console.error(`❌ Backend error (${response.status}):`, errorText);
        throw new Error(`Failed to create shipment: ${response.status} ${response.statusText} - ${errorText}`);
      }

      const result = await response.json();
      console.log('✅ Shipment created in backend with ID:', result.id);
      console.log('✅ Full response:', result);
      return result;
    } catch (error) {
      console.error('❌ Error creating shipment:');
      console.error('   Message:', error.message);
      console.error('   Stack:', error.stack);
      throw error;
    }
  },

  // Update an existing shipment
  async updateShipment(id, shipmentData) {
    try {
      const response = await fetch(`${API_BASE_URL}/shipments/update/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(shipmentData),
      });

      if (!response.ok) {
        throw new Error(`Failed to update shipment: ${response.statusText}`);
      }

      return true;
    } catch (error) {
      console.error('Error updating shipment:', error);
      throw error;
    }
  },

  // Submit a shipment for approval
  async submitShipment(id, notes = '') {
    try {
      const response = await fetch(`${API_BASE_URL}/shipments/${id}/submit`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ notes }),
      });

      if (!response.ok) {
        throw new Error(`Failed to submit shipment: ${response.statusText}`);
      }

      return true;
    } catch (error) {
      console.error('Error submitting shipment:', error);
      throw error;
    }
  },

  // Update shipment status
  async updateShipmentStatus(id, status) {
    try {
      const response = await fetch(`${API_BASE_URL}/shipments/update/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ status }),
      });

      if (!response.ok) {
        throw new Error(`Failed to update status: ${response.statusText}`);
      }

      return true;
    } catch (error) {
      console.error('Error updating status:', error);
      throw error;
    }
  },
};
