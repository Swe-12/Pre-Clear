import React, { useEffect, useState } from 'react';
import apiClient from '../api/apiClient';

export default function DashboardSummaryWidget() {
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await apiClient.getDashboardSummary();
        setSummary(data);
      } catch (err) {
        console.error('Failed to load dashboard summary', err);
        setError(err.message || 'Failed to load');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  if (loading) return <div className="mb-6">Loading summary...</div>;
  if (error) return <div className="mb-6 text-red-600">{error}</div>;
  if (!summary) return null;

  return (
    <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div className="bg-white rounded-xl p-4 border border-slate-200">
        <p className="text-slate-500 text-sm">Total Shipments</p>
        <p className="text-slate-900 text-2xl font-semibold">{summary.totalShipments}</p>
      </div>

      <div className="bg-white rounded-xl p-4 border border-slate-200">
        <p className="text-slate-500 text-sm">Approved</p>
        <p className="text-slate-900 text-2xl font-semibold">{summary.approvedShipments}</p>
      </div>

      <div className="bg-white rounded-xl p-4 border border-slate-200">
        <p className="text-slate-500 text-sm">Pending</p>
        <p className="text-slate-900 text-2xl font-semibold">{summary.pendingShipments}</p>
      </div>

      <div className="bg-white rounded-xl p-4 border border-slate-200">
        <p className="text-slate-500 text-sm">Exceptions</p>
        <p className="text-slate-900 text-2xl font-semibold">{summary.exceptionsCount}</p>
      </div>
    </div>
  );
}
