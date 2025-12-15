# Broker Dashboard "Failed to Fetch" Error - FIXED

## Problem
Broker Dashboard was showing "Failed to fetch" error when trying to load shipments assigned to the broker.

## Root Causes Fixed

### 1️⃣ **CORS Misconfiguration (CRITICAL)**
**Issue**: Backend was using `AllowAnyOrigin()` which cannot support credentials.

```csharp
// ❌ BROKEN
.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
```

**Fix**: Updated to use specific origins with credentials support.

```csharp
// ✅ FIXED
options.AddPolicy("DevelopmentPolicy", policy =>
{
    policy
        .WithOrigins("http://localhost:3000", "http://localhost:5000", "http://localhost:5232")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
```

### 2️⃣ **Missing Broker-Specific API Call**
**Issue**: BrokerDashboard was using generic `useShipments()` hook instead of calling the broker-specific endpoint.

**Fix**: Created new `useBrokerShipments()` hook that calls the correct endpoint:
```
GET /api/brokers/{brokerId}/shipments
```

### 3️⃣ **Missing Authorization Header**
**Issue**: Requests to broker API didn't include the JWT token.

**Fix**: Added Authorization header to all broker API requests:
```js
headers: {
  'Authorization': `Bearer ${localStorage.getItem('token')}`
}
```

---

## Files Changed

### Backend
- **`backend/Program.cs`**
  - Fixed CORS configuration
  - Uses `DevelopmentPolicy` that allows credentials

### Frontend
- **`frontend/src/hooks/useBrokerShipments.js`** (NEW)
  - Fetches shipments assigned to broker
  - Gets broker ID from user in localStorage
  - Calls `/api/brokers/{brokerId}/shipments`
  - Includes JWT token in Authorization header
  - Has retry logic on errors

- **`frontend/src/components/broker/BrokerDashboard.jsx`**
  - Now uses `useBrokerShipments()` instead of `useShipments()`
  - Added error state display with retry button
  - Added loading state indicator
  - Auto-retry failed requests up to 2 times

---

## How It Works Now

### Flow Diagram
```
1. Broker logs in
   ↓
2. Token saved in localStorage
   ↓
3. BrokerDashboard mounts
   ↓
4. useBrokerShipments() hook initializes
   ↓
5. Hook extracts brokerId from localStorage.user
   ↓
6. Hook calls GET /api/brokers/{brokerId}/shipments
   ↓
7. Request includes Authorization: Bearer {token}
   ↓
8. Backend CORS allows it (credentials + specific origin)
   ↓
9. Backend returns shipments assigned to this broker
   ↓
10. React renders shipment table
```

---

## Testing Checklist

### ✅ Test 1: Login as Broker
1. Navigate to login page
2. Select "Broker" role
3. Enter credentials
4. Verify token is saved in localStorage

### ✅ Test 2: View Broker Dashboard
1. After login, navigate to broker dashboard
2. Should see loading indicator briefly
3. Shipments table should appear with broker's shipments
4. No "Failed to fetch" error

### ✅ Test 3: Error Handling
1. Disconnect network/stop backend
2. Dashboard should show error message
3. "Retry" button should appear
4. Auto-retry should attempt 2 times

### ✅ Test 4: Verify Backend CORS
Open DevTools → Network tab:
- Request to `/api/brokers/{id}/shipments`
- Check Headers: `Authorization: Bearer [token]`
- Response should have `Access-Control-Allow-Credentials: true`
- Status should be 200 (not `blocked:cors`)

### ✅ Test 5: Database Verification
```sql
-- Check if broker has assigned shipments
SELECT * FROM Shipments 
WHERE AssignedBrokerId = {brokerId}
ORDER BY CreatedAt DESC;
```

---

## Common Issues & Solutions

### Issue: Still getting "Failed to fetch"

**Solution 1: Check browser console**
```
Open DevTools → Console → Look for error message
Common messages:
- "Unauthorized - please log in again" → Token expired
- "Broker endpoint not found" → Backend not running
- "CORS policy blocked" → CORS config not reloaded
```

**Solution 2: Verify backend is running**
```bash
cd backend
dotnet run
# Should show: Now listening on: http://localhost:5232
```

**Solution 3: Clear browser cache**
```
DevTools → Application → Clear storage → Clear site data
F5 to refresh
```

**Solution 4: Restart both apps**
```bash
# Terminal 1 - Backend
cd backend
dotnet run

# Terminal 2 - Frontend  
cd frontend
npm run dev
```

### Issue: Loading state never goes away

**Check**: Browser console for silent errors
```js
// In browser console
localStorage.getItem('token')      // Should return a token
localStorage.getItem('user')       // Should return user JSON with id
```

### Issue: Seeing different broker's shipments

**This should NOT happen** - each broker's hook gets their specific ID from localStorage and only fetches their own shipments.

**Verify**: Check user ID in localStorage:
```js
JSON.parse(localStorage.getItem('user')).id
```

---

## CORS Explanation (For Viva/Interview)

**Question**: "Why was CORS failing?"

**Answer**:
> "CORS (Cross-Origin Resource Sharing) has a fundamental rule: if you're sending credentials (like cookies or Authorization headers), the backend cannot use the wildcard origin `*`. It must specify exact origins.
> 
> The original code used `AllowAnyOrigin()`, which sets `Access-Control-Allow-Origin: *`. This is incompatible with `AllowCredentials()` because the browser's security model prevents wildcard with credentials.
> 
> The fix specifies exact origins that are allowed: `http://localhost:3000` (frontend), `http://localhost:5232` (backend). Now the CORS policy can safely allow credentials."

---

## API Endpoint Reference

### Broker Shipments
```
GET /api/brokers/{brokerId}/shipments

Headers:
- Authorization: Bearer {token}
- Content-Type: application/json

Response:
[
  {
    id: 123,
    title: "...",
    aiScore: 94,
    brokerApproval: "pending",
    ...
  }
]
```

### Assign Broker to Shipment
```
POST /api/shipments/{shipmentId}/assign-broker

Body:
{
  "brokerId": 456
}
```

---

## Architecture Notes

### Why separate `useBrokerShipments()` hook?
- **Isolated responsibility**: Broker-specific logic is self-contained
- **Easy to maintain**: Changes to broker API don't affect shipper dashboard
- **Cleaner code**: BrokerDashboard doesn't have conditional filtering logic
- **Scalable**: Can add more broker-specific features easily

### Why no global state?
- **Per-request auth**: Each request verifies token, more secure
- **No stale data**: Always fetches fresh from backend
- **Simpler debugging**: Easy to trace data flow

---

## Next Steps (If Issues Persist)

1. **Check Swagger API documentation**:
   ```
   http://localhost:5232/swagger
   ```
   Find the broker endpoints and test them directly

2. **Monitor network requests**:
   - DevTools → Network tab
   - Filter for `/api/brokers/`
   - Check request/response details

3. **Enable detailed logging**:
   Add more console.log statements in `useBrokerShipments.js`

4. **Test API with Postman/cURL**:
   ```bash
   curl -H "Authorization: Bearer {token}" \
        http://localhost:5232/api/brokers/1/shipments
   ```

