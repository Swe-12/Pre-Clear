// Default to backend during development if VITE_API_BASE not set
const API_BASE = import.meta.env.VITE_API_BASE || (import.meta.env.DEV ? 'http://localhost:5232' : '');

function buildUrl(path) {
  // Ensure there's no double slash
  if (!API_BASE) return path;
  return API_BASE.replace(/\/+$/, '') + '/' + path.replace(/^\/+/, '');
}

async function get(path) {
  const res = await fetch(buildUrl(path), { credentials: 'include' });
  return res.json();
}

async function post(path, body) {
  const res = await fetch(buildUrl(path), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
    credentials: 'include'
  });
  return res.json();
}

async function uploadFile(path, file) {
  const fd = new FormData();
  fd.append('file', file);
  const res = await fetch(buildUrl(path), {
    method: 'POST',
    body: fd,
    credentials: 'include'
  });
  return res.json();
}

async function getAuditByShipment(shipmentId) {
  return get(`/api/audit/${shipmentId}`);
}

async function getAuditByUser(userId) {
  return get(`/api/audit/user/${userId}`);
}

async function getDashboardSummary() {
  return get(`/api/dashboard/summary`);
}

async function postSyncRun() {
  return post(`/api/sync/run`, {});
}

async function createPaymentIntent(userId, amount) {
  return post(`/api/payment/create-intent`, { userId, amount });
}

async function confirmPayment(paymentId) {
  return post(`/api/payment/confirm`, { paymentId });
}

export default {
  get,
  post,
  uploadFile,
  buildUrl,
  getAuditByShipment,
  getAuditByUser,
  getDashboardSummary,
  postSyncRun,
  createPaymentIntent,
  confirmPayment,
};
