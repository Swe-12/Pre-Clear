const API_BASE = import.meta.env.VITE_API_BASE || '';

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

export default {
  get,
  post,
  uploadFile,
  buildUrl,
};
