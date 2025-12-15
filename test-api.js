// Test script to verify backend connectivity and shipment creation
const API_BASE_URL = 'http://localhost:5000/api';

async function testCreateShipment() {
  console.log('Testing shipment creation...');
  
  const testShipment = {
    shipmentName: 'Test Shipment',
    status: 'draft',
    currency: 'USD',
    totalValue: 1000,
    totalWeight: 50,
    items: [
      {
        hsCode: '8541.10.00',
        quantity: 100,
        totalValue: 1000,
        description: 'Electronic Components'
      }
    ],
    parties: [
      {
        partyType: 'shipper',
        companyName: 'Test Shipper Inc',
        contactName: 'John Doe',
        email: 'john@test.com',
        phone: '+1-555-0001'
      },
      {
        partyType: 'consignee',
        companyName: 'Test Consignee Ltd',
        contactName: 'Jane Smith',
        email: 'jane@test.com',
        phone: '+1-555-0002'
      }
    ]
  };

  try {
    const response = await fetch(`${API_BASE_URL}/shipments/create`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(testShipment)
    });

    if (!response.ok) {
      console.error(`Error: ${response.status} ${response.statusText}`);
      const error = await response.text();
      console.error('Response:', error);
      return null;
    }

    const result = await response.json();
    console.log('✅ Shipment created successfully:', result);
    return result;
  } catch (error) {
    console.error('❌ Failed to create shipment:', error.message);
    return null;
  }
}

async function testGetShipment(id) {
  console.log(`Testing retrieve shipment ${id}...`);
  
  try {
    const response = await fetch(`${API_BASE_URL}/shipments/details/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      }
    });

    if (!response.ok) {
      console.error(`Error: ${response.status} ${response.statusText}`);
      return null;
    }

    const result = await response.json();
    console.log('✅ Shipment retrieved successfully:', result);
    return result;
  } catch (error) {
    console.error('❌ Failed to retrieve shipment:', error.message);
    return null;
  }
}

async function runTests() {
  console.log('=== Backend Connectivity Test ===\n');
  
  // Test 1: Create shipment
  const created = await testCreateShipment();
  
  if (created?.id) {
    console.log('\n--- Waiting 2 seconds before retrieval test ---\n');
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Test 2: Retrieve the created shipment
    await testGetShipment(created.id);
  }
  
  console.log('\n=== Test Complete ===');
}

// Run tests
runTests();
