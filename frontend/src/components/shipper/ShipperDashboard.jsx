import { PackagePlus, Package, Clock, CheckCircle, XCircle, AlertTriangle, TrendingUp, Upload, DollarSign, Eye, Edit, Filter, MessageSquare } from 'lucide-react';
import { NotificationPanel } from '../NotificationPanel';
import DashboardSummaryWidget from '../DashboardSummaryWidget';
import { shipmentsStore, createDefaultShipment } from '../../store/shipmentsStore';
import { useState } from 'react';
import { useShipments } from '../../hooks/useShipments';
import { getCurrencyByCountry } from '../../utils/validation';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';


export function ShipperDashboard({ onNavigate }) {
  const { shipments: allShipments } = useShipments();
  const [filterStatus, setFilterStatus] = useState('all');
  
  // Categorize shipments
  const pendingReview = allShipments.filter(s => 
    s.status === 'draft' || 
    s.status === 'documents-uploaded' || 
    s.status === 'document-requested'
  );
  
  const inReview = allShipments.filter(s => 
    s.status === 'awaiting-ai' || 
    s.status === 'awaiting-broker' ||
    (s.aiApproval === 'pending' || s.brokerApproval === 'pending')
  );
  
  const cleared = allShipments.filter(s => 
    s.status === 'token-generated' || 
    (s.aiApproval === 'approved' && s.brokerApproval === 'approved')
  );
  
  const cancelled = allShipments.filter(s => s.status === 'cancelled');
  
  // Filter shipments based on selected status
  const getFilteredShipments = () => {
    if (filterStatus === 'all') return allShipments;
    if (filterStatus === 'pending') return pendingReview;
    if (filterStatus === 'review') return inReview;
    if (filterStatus === 'cleared') return cleared;
    if (filterStatus === 'cancelled') return cancelled;
    return allShipments;
  };
  
  const filteredShipments = getFilteredShipments();

  const getStatusBadge = (shipment) => {
    if (shipment.status === 'cancelled') {
      return (
        <span className="px-3 py-1 bg-slate-100 text-slate-700 rounded-full text-sm flex items-center gap-1">
          <XCircle className="w-3 h-3" />
          Cancelled
        </span>
      );
    }
    if (shipment.status === 'document-requested') {
      return (
        <span className="px-3 py-1 bg-red-100 text-red-700 rounded-full text-sm flex items-center gap-1">
          <AlertTriangle className="w-3 h-3" />
          Documents Requested
        </span>
      );
    }
    if (shipment.token && shipment.status === 'token-generated') {
      return (
        <span className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm flex items-center gap-1">
          <CheckCircle className="w-3 h-3" />
          Cleared
        </span>
      );
    }
    if (shipment.aiApproval === 'approved' && shipment.brokerApproval === 'approved') {
      return (
        <span className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm flex items-center gap-1">
          <CheckCircle className="w-3 h-3" />
          Cleared
        </span>
      );
    }
    if (shipment.status === 'awaiting-broker' || shipment.brokerApproval === 'pending') {
      return (
        <span className="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm flex items-center gap-1">
          <Clock className="w-3 h-3" />
          Broker Review
        </span>
      );
    }
    if (shipment.status === 'awaiting-ai' || shipment.aiApproval === 'pending') {
      return (
        <span className="px-3 py-1 bg-purple-100 text-purple-700 rounded-full text-sm flex items-center gap-1">
          <Clock className="w-3 h-3" />
          AI Processing
        </span>
      );
    }
    if (shipment.aiApproval === 'rejected' || shipment.brokerApproval === 'rejected') {
      return (
        <span className="px-3 py-1 bg-red-100 text-red-700 rounded-full text-sm flex items-center gap-1">
          <XCircle className="w-3 h-3" />
          Rejected
        </span>
      );
    }
    return (
      <span className="px-3 py-1 bg-amber-100 text-amber-700 rounded-full text-sm flex items-center gap-1">
        <Clock className="w-3 h-3" />
        Pending Review
      </span>
    );
  };

  const getActionButton = (shipment) => {
    if (shipment.status === 'cancelled') {
      return null;
    }
    if (shipment.status === 'draft' || shipment.status === 'document-requested') {
      return (
        <div className="flex gap-2">
          <button
            onClick={() => onNavigate('shipment-form', shipment)}
            className="px-3 py-1.5 bg-amber-600 text-white rounded-lg hover:bg-amber-700 transition-colors text-sm flex items-center gap-1"
            title="Edit shipment details"
          >
            <Edit className="w-3.5 h-3.5" />
            Edit
          </button>
          {shipment.status === 'document-requested' && (
            <button
              onClick={() => onNavigate('upload-documents', shipment)}
              className="px-3 py-1.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors text-sm flex items-center gap-1"
            >
              <Upload className="w-3.5 h-3.5" />
              Upload
            </button>
          )}
        </div>
      );
    }
    if (shipment.token && !shipment.paymentStatus) {
      return (
        <button
          onClick={() => onNavigate('payment', shipment)}
          className="px-3 py-1.5 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors text-sm flex items-center gap-1"
        >
          <DollarSign className="w-3.5 h-3.5" />
          Pay Now
        </button>
      );
    }
    return (
      <button
        onClick={() => onNavigate('shipment-details', shipment)}
        className="px-3 py-1.5 bg-yellow-500 text-yellow-900 rounded-lg hover:bg-yellow-600 hover:shadow-md transition-all text-sm flex items-center gap-1"
        title="View shipment details"
      >
        <Eye className="w-3.5 h-3.5" />
        View
      </button>
    );
  };

  const UnifiedShipmentTable = ({ shipments }) => {
    if (shipments.length === 0) {
      return (
        <div className="bg-slate-50 border border-slate-200 rounded-lg p-8 text-center">
            <Package className="w-12 h-12 mx-auto mb-4 text-slate-400" />
            <p className="text-slate-500 text-base">No shipments found</p>
            <p className="text-slate-500 text-sm mt-1">Try adjusting your filters or create a new shipment</p>
          </div>
      );
    }

    return (
      <div className="bg-white border border-slate-200 rounded-lg overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="bg-slate-50">
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">Shipment ID</th>
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">Product</th>
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">Origin → Destination</th>
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">Value</th>
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">AI Score</th>
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">Status</th>
                <th className="text-slate-900 p-4 text-left font-semibold text-sm">Date Created</th>
                <th className="text-slate-900 p-4 text-right font-semibold text-sm">Actions</th>
              </tr>
            </thead>
            <tbody>
              {shipments.map((shipment, index) => {
                const currency = getCurrencyByCountry(shipment.originCountry || 'US');
                const aiScore = shipment.aiScore || 0;
                const isEvenRow = index % 2 === 0;

                return (
                  <tr key={shipment.id} className={`${isEvenRow ? 'bg-white' : 'bg-slate-50'} hover:bg-slate-100` }>
                    <td className="p-4 text-slate-900 font-semibold text-sm">#{shipment.id}</td>
                    <td className="p-4 text-slate-900 text-sm">
                      <div className="max-w-[150px] truncate">{shipment.productName}</div>
                    </td>
                    <td className="p-4 text-slate-900 text-sm">
                      <div className="text-sm"><strong>{shipment.originCountry || 'N/A'}</strong> → <strong>{shipment.destCountry || 'N/A'}</strong></div>
                      <div className="text-xs text-slate-500 mt-1">{shipment.destCity || 'Unknown'}</div>
                    </td>
                    <td className="p-4 text-slate-900 font-medium text-sm">{currency.symbol}{shipment.value} {currency.code}</td>
                    <td className="p-4">
                      {aiScore > 0 ? (
                        <div className="flex items-center gap-2">
                          <div className="w-16 h-2 bg-slate-200 rounded-full overflow-hidden">
                            <div
                              className={`${aiScore >= 85 ? 'bg-green-500' : aiScore >= 70 ? 'bg-amber-500' : 'bg-red-500'} h-full`}
                              style={{ width: `${aiScore}%` }}
                            />
                          </div>
                          <span className={`${aiScore >= 85 ? 'text-green-700' : aiScore >= 70 ? 'text-amber-700' : 'text-red-700'} text-sm font-semibold`}>{aiScore}%</span>
                        </div>
                      ) : (
                        <span className="text-slate-400 text-sm">-</span>
                      )}
                    </td>
                    <td className="p-4">{getStatusBadge(shipment)}</td>
                    <td className="p-4 text-slate-600 text-sm">{new Date(shipment.createdAt).toLocaleDateString()}</td>
                    <td className="p-4 text-right">
                      <div className="flex items-center justify-end gap-2">
                        <button
                          onClick={() => onNavigate('chat', shipment)}
                          title="Open chat for this shipment"
                          className="p-2 rounded-md hover:bg-slate-100"
                        >
                          <MessageSquare className="w-5 h-5 text-slate-600" />
                        </button>
                        {getActionButton(shipment)}
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    );
  };

  return (
    <div className="bg-white">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-yellow-900 text-3xl font-bold mb-2">Shipper Dashboard</h1>
        <p className="text-slate-600 text-base opacity-80">Manage your shipments and track pre-clearance approvals</p>
      </div>

      {/* Real-time Notifications */}
      <div className="mb-6">
        <NotificationPanel role="shipper" onNavigate={onNavigate} />
      </div>

      {/* Global Summary from backend */}
      <DashboardSummaryWidget />

      {/* Quick Actions - Create Shipment Button */}
      <div className="mb-8">
        <button
          onClick={() => {
            const newShipment = createDefaultShipment();
            onNavigate('shipment-form', newShipment);
          }}
          className="w-full p-6 text-white rounded-xl hover:shadow-2xl transition-all group bg-gradient-to-r from-blue-600 to-blue-700"
        >
          <div className="flex items-center gap-4">
            <div className="w-14 h-14 rounded-xl flex items-center justify-center group-hover:scale-110 transition-transform" style={{ background: 'rgba(255,255,255,0.2)' }}>
              <PackagePlus className="w-7 h-7" />
            </div>
            <div className="text-left">
              <h3 style={{ fontSize: '20px', fontWeight: '600', marginBottom: '4px' }}>Create New Shipment</h3>
              <p style={{ color: 'rgba(255,255,255,0.9)', fontSize: '14px' }}>Start pre-clearance process with AI-powered compliance check</p>
            </div>
          </div>
        </button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div className="bg-white rounded-xl p-5 border border-slate-200 transition-all hover:shadow-lg cursor-pointer">
          <div className="flex items-center justify-between mb-3">
            <p className="text-slate-600 text-sm font-medium opacity-80">Pending Review</p>
            <Clock className="text-yellow-500 w-5 h-5" />
          </div>
          <p className="text-yellow-900 text-3xl font-bold">{pendingReview.length}</p>
        </div>
        
        <div className="bg-white rounded-xl p-5 border border-slate-200 transition-all hover:shadow-lg cursor-pointer">
          <div className="flex items-center justify-between mb-3">
            <p className="text-slate-600 text-sm font-medium opacity-80">In Review</p>
            <TrendingUp className="text-blue-500 w-5 h-5" />
          </div>
          <p className="text-yellow-900 text-3xl font-bold">{inReview.length}</p>
        </div>
        
        <div className="bg-white rounded-xl p-5 border border-slate-200 transition-all hover:shadow-lg cursor-pointer">
          <div className="flex items-center justify-between mb-3">
            <p className="text-slate-600 text-sm font-medium opacity-80">Cleared</p>
            <CheckCircle className="text-green-500 w-5 h-5" />
          </div>
          <p className="text-yellow-900 text-3xl font-bold">{cleared.length}</p>
        </div>
        
        <div className="bg-white rounded-xl p-5 border border-slate-200 transition-all hover:shadow-lg cursor-pointer">
          <div className="flex items-center justify-between mb-3">
            <p className="text-slate-600 text-sm font-medium opacity-80">Cancelled</p>
            <XCircle className="text-gray-500 w-5 h-5" />
          </div>
          <p className="text-yellow-900 text-3xl font-bold">{cancelled.length}</p>
        </div>
      </div>

      {/* Filter Controls */}
      <div className="mb-6 flex items-center gap-3 flex-wrap">
        <div className="flex items-center gap-2 text-slate-600">
          <Filter className="w-5 h-5" />
          <span className="text-sm font-medium">Filter:</span>
        </div>
        {[
          { label: 'All Shipments', value: 'all' },
          { label: 'Pending Review', value: 'pending' },
          { label: 'In Review', value: 'review' },
          { label: 'Cleared', value: 'cleared' },
          { label: 'Cancelled', value: 'cancelled' }
        ].map(filter => (
          <button
            key={filter.value}
            onClick={() => setFilterStatus(filter.value)}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-all ${
              filterStatus === filter.value 
                ? 'bg-yellow-500 text-yellow-900' 
                : 'bg-slate-100 text-slate-600 hover:bg-slate-200'
            }`}
          >
            {filter.label}
          </button>
        ))}
      </div>

      {/* Unified Shipments Table */}
      <div>
        <h2 className="text-yellow-900 text-lg font-bold mb-4">
          {filterStatus === 'all' ? 'All Shipments' : 
           filterStatus === 'pending' ? 'Pending Review' : 
           filterStatus === 'review' ? 'In Review' : 
           filterStatus === 'cleared' ? 'Cleared' : 
           'Cancelled'} ({filteredShipments.length})
        </h2>
        <UnifiedShipmentTable shipments={filteredShipments} />
      </div>
    </div>
  );
}