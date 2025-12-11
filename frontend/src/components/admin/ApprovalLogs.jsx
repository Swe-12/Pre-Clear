import React, { useEffect, useState } from 'react';
import { FileSearch } from 'lucide-react';
import apiClient from '../../api/apiClient';

// Props: shipmentId or userId (optional). If provided, component will fetch audit logs
// for that shipment or user. If neither provided, shows a short info message.
export function ApprovalLogs({ shipmentId, userId }) {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchLogs = async () => {
      setError(null);
      setLoading(true);
      try {
        let data = [];
        if (shipmentId) {
          data = await apiClient.getAuditByShipment(shipmentId);
        } else if (userId) {
          data = await apiClient.getAuditByUser(userId);
        }
        // Ensure array
        setLogs(Array.isArray(data) ? data : []);
      } catch (err) {
        console.error('Failed to load audit logs', err);
        setError(err.message || 'Failed to load audit logs');
      } finally {
        setLoading(false);
      }
    };

    if (shipmentId || userId) fetchLogs();
  }, [shipmentId, userId]);

  return (
    <div style={{ background: '#FBF9F6', minHeight: '100vh', padding: 24 }}>
      <h1 className="mb-2" style={{ fontWeight: 600, display: 'flex', alignItems: 'center', gap: 10, fontSize: '1.5rem' }}>
        <FileSearch className="w-6 h-6" style={{ color: '#3A2B28' }} />
        <span>Approval Logs & Audit Trail</span>
      </h1>
      <p className="text-slate-600 mb-8">View approval activities and audit history. Pass <code>shipmentId</code> or <code>userId</code> to load real data.</p>

      {loading && <div className="mb-4">Loading...</div>}
      {error && <div className="mb-4 text-red-600">{error}</div>}

      {!shipmentId && !userId && (
        <div className="bg-white rounded-xl border border-slate-200 p-6">
          <p className="text-slate-600">No context provided. To view audit logs, navigate from a shipment detail or user page that passes <code>shipmentId</code> or <code>userId</code> to this component.</p>
        </div>
      )}

      {(shipmentId || userId) && (
        <div className="bg-white rounded-xl border border-slate-200 overflow-hidden">
          <table className="w-full">
            <thead>
              <tr className="bg-slate-50 border-b border-slate-200">
                <th className="text-left py-4 px-6 text-slate-700">Shipment ID</th>
                <th className="text-left py-4 px-6 text-slate-700">Action</th>
                <th className="text-left py-4 px-6 text-slate-700">User ID</th>
                <th className="text-left py-4 px-6 text-slate-700">Timestamp</th>
              </tr>
            </thead>
            <tbody>
              {logs.length === 0 && !loading ? (
                <tr>
                  <td colSpan={4} className="py-6 px-6 text-center text-slate-500">No audit entries found.</td>
                </tr>
              ) : (
                logs.map((log) => (
                  <tr key={log.id} className="border-b border-slate-100">
                    <td className="py-4 px-6 text-slate-900">{log.shipmentId ?? log.shipmentId}</td>
                    <td className="py-4 px-6 text-slate-700">{log.action}</td>
                    <td className="py-4 px-6 text-slate-700">{(log.userId ?? log.user) || 'N/A'}</td>
                    <td className="py-4 px-6 text-slate-600 text-sm">{new Date(log.createdAt || log.timestamp || log.CreatedAt).toLocaleString()}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

