import React, { useState } from 'react';
import apiClient from '../../api/apiClient';

export default function SyncRunner() {
  const [running, setRunning] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  async function runSync() {
    setRunning(true);
    setResult(null);
    setError(null);
    try {
      const res = await apiClient.postSyncRun();
      setResult(res);
    } catch (err) {
      setError(err?.message || String(err));
    } finally {
      setRunning(false);
    }
  }

  return (
    <div className="bg-white rounded-xl p-6 border border-slate-200 shadow-sm">
      <h3 className="text-slate-900 mb-2">ERP/WMS Sync</h3>
      <p className="text-slate-600 text-sm mb-4">Run a mock sync that imports/updates shipments.</p>

      <div className="flex items-center gap-3">
        <button
          onClick={runSync}
          disabled={running}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
        >
          {running ? 'Running...' : 'Run Sync'}
        </button>

        {result && (
          <div className="text-sm text-slate-700">
            Imported: <strong>{result.imported}</strong>, Updated: <strong>{result.updated}</strong>
          </div>
        )}

        {error && <div className="text-sm text-red-600">Error: {error}</div>}
      </div>

      {result?.details && (
        <pre className="mt-4 text-xs bg-slate-50 p-3 rounded">{JSON.stringify(result.details, null, 2)}</pre>
      )}
    </div>
  );
}
