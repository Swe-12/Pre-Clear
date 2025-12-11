import React, { useState } from 'react';
import api from '../api/apiClient';

export default function AiUploader() {
  const [file, setFile] = useState(null);
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  async function handleExtract() {
    if (!file) return setError('Please choose a file');
    setLoading(true);
    setError(null);
    try {
      const res = await api.uploadFile('/api/ai/extract-text', file);
      setResult(res);
    } catch (e) {
      setError('Request failed');
    } finally {
      setLoading(false);
    }
  }

  async function handleValidateInvoice() {
    if (!file) return setError('Please choose a file');
    setLoading(true);
    setError(null);
    try {
      const res = await api.uploadFile('/api/ai/validate-invoice', file);
      setResult(res);
    } catch (e) {
      setError('Request failed');
    } finally {
      setLoading(false);
    }
  }

  async function handleValidatePacking() {
    if (!file) return setError('Please choose a file');
    setLoading(true);
    setError(null);
    try {
      const res = await api.uploadFile('/api/ai/validate-packing-list', file);
      setResult(res);
    } catch (e) {
      setError('Request failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ padding: 12, border: '1px solid #ddd', borderRadius: 6, maxWidth: 720 }}>
      <h3>AI Uploader (mock)</h3>
      <input type="file" onChange={e => setFile(e.target.files && e.target.files[0])} />
      <div style={{ marginTop: 8 }}>
        <button onClick={handleExtract} disabled={loading}>Extract Text</button>
        <button onClick={handleValidateInvoice} disabled={loading} style={{ marginLeft: 8 }}>Validate Invoice</button>
        <button onClick={handleValidatePacking} disabled={loading} style={{ marginLeft: 8 }}>Validate Packing List</button>
      </div>

      {loading && <div style={{ marginTop: 8 }}>Loading…</div>}
      {error && <div style={{ marginTop: 8, color: 'crimson' }}>{error}</div>}

      {result && (
        <div style={{ marginTop: 12, whiteSpace: 'pre-wrap', background: '#f8f8f8', padding: 8 }}>
          <strong>Result:</strong>
          <pre>{JSON.stringify(result, null, 2)}</pre>
        </div>
      )}

      <div style={{ marginTop: 12, fontSize: 12, color: '#666' }}>
        Note: This component calls mock AI endpoints in the backend. Ensure the backend is running.
      </div>
    </div>
  );
}
