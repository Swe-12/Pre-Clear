# Shipment Creation & Sync Testing Guide

## Current Status ✅
- **Frontend**: Running on `http://localhost:3001`
- **Backend**: Ready to run on `http://localhost:5000`

## Step-by-Step Testing Instructions

### 1. Start the Backend (if not already running)
```powershell
cd C:\Users\devis\Pre-Clear\backend
dotnet run
```

Expected output:
```
info: PreClear.Api.Program[0]
      Using DB connection: Server=localhost;Database=preclear;User=root;Password=****
...
Application started. Press Ctrl+C to stop.
```

### 2. Access the Frontend
- Open browser: **http://localhost:3001**
- You should see the Pre-Clear app login/home page

### 3. Create a Test Shipment

**Flow:**
1. Navigate to shipper dashboard (or create shipment page)
2. Fill in shipment details:
   - **Product Name**: "Test Electronic Components"
   - **HS Code**: 8541.10.00
   - **Quantity**: 100
   - **Value**: 5000 USD
   - **Weight**: 50 kg
   - **Mode**: Air
   - **Shipment Type**: International
   - **Shipper Info**: Fill with test data
   - **Consignee Info**: Fill with test data
   - **Packages**: Add at least one package with products

3. Click **"Create Shipment"** button

### 4. Expected Behavior After Creation

**In Frontend Console** (Browser DevTools - F12 → Console):
```javascript
// You should see:
✅ "Shipment saved to backend: {id: 123, ...}"
// And the URL should change to:
http://localhost:3001/#shipment-details/SHP-1734267123456
```

**In Browser:**
- Loading spinner briefly appears
- Shipment Details page loads with all your data
- All fields show the values you entered

### 5. Test Persistence - Reload Page

1. **Reload the page** (Ctrl+R or Cmd+R)
2. **Expected**:
   - Loading spinner appears
   - Shipment Details loads with ALL the same data
   - No data loss
   - Everything looks exactly the same

### 6. Test Backend Sync

**Check Database Directly:**
```sql
-- SSH to your MySQL server
mysql -u root -ppass123 -h localhost preclear

-- Run:
SELECT * FROM Shipments ORDER BY Id DESC LIMIT 1;
SELECT COUNT(*) FROM Shipments;
```

You should see your created shipment in the database.

### 7. Auto-Refresh Test

The component automatically refreshes every 10 seconds from the backend:

1. After creating shipment, wait 10 seconds
2. Open **Browser Console** (F12)
3. You should see console logs showing data refresh
4. In another terminal, modify the shipment in the database
5. After 10 seconds, the frontend should reflect the change

## Testing Checklist

- [ ] Frontend runs on port 3001
- [ ] Backend runs without errors on port 5000
- [ ] Can create a shipment without errors
- [ ] URL includes shipment ID (e.g., `#shipment-details/SHP-123`)
- [ ] Console shows "Shipment saved to backend" message
- [ ] Page reload preserves all shipment data
- [ ] Shipment appears in database
- [ ] Auto-refresh works (check console logs)
- [ ] No console errors (F12 → Console tab)

## Troubleshooting

### Issue: Backend port 5000 already in use
```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill the process (replace PID)
taskkill /PID <PID> /F

# Or use different port:
set ASPNETCORE_URLS=http://localhost:5001
dotnet run
# Then update API_BASE_URL in shipmentApi.js
```

### Issue: "Shipment not found" after reload
**Possible causes:**
1. Backend is not running
2. Database connection failed
3. Shipment ID format mismatch

**Check:**
- Backend console for errors
- MySQL is running: `mysql -u root -ppass123 -e "SHOW DATABASES;"`
- Database exists: `preclear`

### Issue: Console shows CORS errors
```
Access to fetch blocked by CORS policy...
```

**Check:** Backend has CORS enabled in Program.cs
```csharp
app.UseCors(); // Should be before app.MapControllers()
```

### Issue: "API_BASE_URL connection refused"
- Verify backend is running
- Check backend logs for startup errors
- Verify port 5000 is correct in shipmentApi.js

## What to Report

If there are issues, provide:
1. **Frontend console logs** (F12 → Console)
2. **Backend console output** (dotnet run output)
3. **Network tab** (F12 → Network, then create shipment)
4. **Which step failed** from the checklist above
5. **Exact error messages**

## API Endpoints Being Used

- `POST /api/shipments/create` - Create new shipment
- `GET /api/shipments/details/{id}` - Fetch shipment by ID
- `PUT /api/shipments/update/{id}` - Update shipment
- `POST /api/shipments/{id}/submit` - Submit for approval

All endpoints are called from `shipmentApi.js` service.
