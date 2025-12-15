import React, { useState } from 'react';
import apiClient from '../../api/apiClient';

export default function PaymentRunner() {
  const [userId, setUserId] = useState(1);
  const [amount, setAmount] = useState(10.0);
  const [result, setResult] = useState(null);
  const [confirmRes, setConfirmRes] = useState(null);

  async function createIntent() {
    setResult(null);
    try {
      const res = await apiClient.createPaymentIntent(Number(userId), Number(amount));
      setResult(res);
    } catch (e) {
      setResult({ error: e.message || String(e) });
    }
  }

  async function confirm() {
    if (!result?.id) return;
    setConfirmRes(null);
    try {
      const r = await apiClient.confirmPayment(result.id);
      setConfirmRes(r);
    } catch (e) {
      setConfirmRes({ error: e.message || String(e) });
    }
  }

  return (
    <div className="bg-white rounded-xl p-6 border border-slate-200 shadow-sm">
      <h3 className="text-slate-900 mb-2">Payment Simulation</h3>
      <div className="grid grid-cols-3 gap-2 mb-3">
        <input value={userId} onChange={(e) => setUserId(e.target.value)} className="p-2 border rounded" />
        <input value={amount} onChange={(e) => setAmount(e.target.value)} className="p-2 border rounded" />
        <button onClick={createIntent} className="px-4 py-2 bg-green-600 text-white rounded">Create Intent</button>
      </div>

      {result && (
        <div className="mb-3">
          <div className="text-sm text-slate-700">Created: <strong>{result.id}</strong> — <em>{result.status}</em> — ${result.amount}</div>
          <button onClick={confirm} className="mt-2 px-3 py-1 bg-blue-600 text-white rounded">Confirm</button>
        </div>
      )}

      {confirmRes && (
        <div className="mt-2 text-sm">Confirmed: {confirmRes.status || JSON.stringify(confirmRes)}</div>
      )}
    </div>
  );
}
