"use client";

import React, { useState, useEffect } from 'react';
import { HistoryOutlined, CalendarOutlined, DollarOutlined, DeleteOutlined, EyeOutlined, LoadingOutlined } from '@ant-design/icons';

const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

export default function HistoryPage() {
    const [history, setHistory] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedSchedule, setSelectedSchedule] = useState<any>(null);

    useEffect(() => {
        fetchHistory();
    }, []);

    const fetchHistory = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/';
            return;
        }

        try {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AIChat/History`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setHistory(data);
            }
        } catch (error) {
            console.error('Failed to fetch history:', error);
        } finally {
            setLoading(false);
        }
    };

    const deleteHistory = async (id: number) => {
        if (!confirm('Bạn có chắc muốn xóa lịch trình này?')) return;

        const token = localStorage.getItem('token');
        try {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/AIChat/History/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                setHistory(history.filter(h => h.id !== id));
                alert('Đã xóa lịch trình');
            }
        } catch (error) {
            console.error('Failed to delete:', error);
            alert('Lỗi khi xóa lịch trình');
        }
    };

    const viewSchedule = (item: any) => {
        try {
            const schedule = JSON.parse(item.scheduleJson);
            // Support both PascalCase (old data) and camelCase (new data)
            const normalizedSchedule = {
                ...schedule,
                dailyItinerary: schedule.dailyItinerary || schedule.DailyItinerary || [],
                summary: schedule.summary || schedule.Summary || item.summary,
                estimatedTotalCost: schedule.estimatedTotalCost || schedule.EstimatedTotalCost || item.estimatedTotalCost
            };
            setSelectedSchedule({ ...item, parsedSchedule: normalizedSchedule });
        } catch (error) {
            console.error('Failed to parse schedule:', error);
        }
    };

    return (
        <main className="history-page">
            <div className="hero">
                <div className="container">
                    <h1><HistoryOutlined /> Lịch sử lịch trình AI</h1>
                    <p>Xem lại các lịch trình bạn đã tạo với AI</p>
                </div>
            </div>

            <div className="container">
                {loading ? (
                    <div className="loading-state">
                        <LoadingOutlined style={{ fontSize: '48px', color: '#667eea' }} />
                        <p>Đang tải lịch sử...</p>
                    </div>
                ) : history.length === 0 ? (
                    <div className="empty-state">
                        <CalendarOutlined style={{ fontSize: '64px', color: '#cbd5e1' }} />
                        <h3>Chưa có lịch trình nào</h3>
                        <p>Hãy tạo lịch trình đầu tiên của bạn!</p>
                        <a href="/ai" className="btn-primary">Tạo lịch trình ngay</a>
                    </div>
                ) : (
                    <div className="history-grid">
                        {history.map((item) => (
                            <div key={item.id} className="history-card">
                                <div className="card-header">
                                    <div className="destination">
                                        <h3>{item.destination}</h3>
                                        <span className="province">{item.province?.provinceName}</span>
                                    </div>
                                    <div className="date">
                                        {new Date(item.createdAt).toLocaleDateString('vi-VN')}
                                    </div>
                                </div>

                                <div className="card-body">
                                    <div className="summary">{item.summary}</div>

                                    <div className="details">
                                        <div className="detail-item">
                                            <CalendarOutlined />
                                            <span>{item.numberOfDays} ngày</span>
                                        </div>
                                        <div className="detail-item">
                                            <DollarOutlined />
                                            <span>{money(item.budget)} VND</span>
                                        </div>
                                        <div className="detail-item">
                                            <span className="badge">{item.travelStyle}</span>
                                            <span className="badge">{item.companionType}</span>
                                        </div>
                                    </div>

                                    <div className="cost-info">
                                        Chi phí dự kiến: <strong>{money(item.estimatedTotalCost)} VND</strong>
                                    </div>
                                </div>

                                <div className="card-footer">
                                    <button className="btn-view" onClick={() => viewSchedule(item)}>
                                        <EyeOutlined /> Xem chi tiết
                                    </button>
                                    <button className="btn-delete" onClick={() => deleteHistory(item.id)}>
                                        <DeleteOutlined /> Xóa
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Schedule Detail Modal */}
            {selectedSchedule && (
                <div className="modal-overlay" onClick={() => setSelectedSchedule(null)}>
                    <div className="modal-container" onClick={(e) => e.stopPropagation()}>
                        <button className="modal-close" onClick={() => setSelectedSchedule(null)}>
                            ✕
                        </button>

                        <div className="modal-header">
                            <CalendarOutlined style={{ fontSize: '28px' }} />
                            <div>
                                <h2>{selectedSchedule.destination}</h2>
                                <p>{new Date(selectedSchedule.createdAt).toLocaleString('vi-VN')}</p>
                            </div>
                        </div>

                        <div className="modal-body">
                            <div className="result-summary">
                                {selectedSchedule.parsedSchedule?.summary || selectedSchedule.summary || 'Không có mô tả'}
                            </div>

                            <div className="result-cost">
                                <DollarOutlined /> Tổng chi phí: <strong>
                                    {money(selectedSchedule.parsedSchedule?.estimatedTotalCost || selectedSchedule.estimatedTotalCost || 0)} VND
                                </strong>
                            </div>

                            {selectedSchedule.parsedSchedule?.dailyItinerary && selectedSchedule.parsedSchedule.dailyItinerary.length > 0 ? (
                                <div className="days-list">
                                    {selectedSchedule.parsedSchedule.dailyItinerary.map((day: any, idx: number) => (
                                        <div key={idx} className="day-item">
                                            <div className="day-header">
                                                <strong>Ngày {day.day}</strong>
                                                <span>{money(day.dayBudget || 0)} VND</span>
                                            </div>
                                            <div className="activities">
                                                {day.activities?.map((act: any, actIdx: number) => (
                                                    <div key={actIdx} className="activity">
                                                        <div className="act-time">{act.time || 'N/A'}</div>
                                                        <div className="act-details">
                                                            <strong>{act.scenicSpot?.name || act.activity || 'Hoạt động'}</strong>
                                                            <p>{act.description || ''}</p>
                                                            {act.scenicSpot?.tips && (
                                                                <div className="act-tips">💡 {act.scenicSpot.tips}</div>
                                                            )}
                                                        </div>
                                                        <div className="act-cost">{money(act.estimatedCost || 0)} VND</div>
                                                    </div>
                                                ))}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            ) : (
                                <div className="empty-schedule">
                                    <p>Không có chi tiết lịch trình</p>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            )}

            <style jsx>{`
        .history-page { min-height: 100vh; background: linear-gradient(to bottom, #f8f9ff 0%, #ffffff 100%); padding-bottom: 80px; }
        
        .hero { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 80px 0; text-align: center; margin-bottom: 60px; }
        .hero h1 { font-size: 48px; font-weight: 800; margin-bottom: 16px; }
        .hero p { font-size: 20px; opacity: 0.9; }

        .loading-state, .empty-state { text-align: center; padding: 80px 20px; }
        .loading-state p, .empty-state p { color: #64748b; margin-top: 16px; font-size: 16px; }
        .empty-state h3 { color: #0f172a; font-size: 24px; margin: 16px 0; }
        .btn-primary { display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 14px 32px; border-radius: 12px; font-weight: 700; text-decoration: none; margin-top: 20px; transition: 0.2s; }
        .btn-primary:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4); }

        .history-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 24px; }
        
        .history-card { background: white; border-radius: 20px; padding: 24px; box-shadow: 0 4px 20px rgba(0,0,0,0.08); border: 1px solid #e5e7eb; transition: 0.3s; }
        .history-card:hover { transform: translateY(-4px); box-shadow: 0 8px 30px rgba(102, 126, 234, 0.15); }

        .card-header { display: flex; justify-content: space-between; align-items: start; margin-bottom: 16px; padding-bottom: 16px; border-bottom: 2px solid #f1f5f9; }
        .destination h3 { font-size: 22px; font-weight: 800; color: #667eea; margin: 0 0 4px 0; }
        .province { font-size: 13px; color: #64748b; }
        .date { font-size: 13px; color: #94a3b8; background: #f1f5f9; padding: 6px 12px; border-radius: 8px; }

        .card-body { margin-bottom: 20px; }
        .summary { color: #475569; font-size: 14px; line-height: 1.6; margin-bottom: 16px; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; }

        .details { display: flex; flex-direction: column; gap: 8px; margin-bottom: 16px; }
        .detail-item { display: flex; align-items: center; gap: 8px; color: #64748b; font-size: 14px; }
        .badge { background: #f0f9ff; color: #0369a1; padding: 4px 12px; border-radius: 6px; font-size: 12px; font-weight: 600; }

        .cost-info { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 12px 16px; border-radius: 12px; font-size: 14px; }
        .cost-info strong { font-size: 16px; }

        .card-footer { display: flex; gap: 12px; }
        .btn-view, .btn-delete { flex: 1; padding: 10px; border-radius: 10px; font-weight: 700; font-size: 14px; cursor: pointer; transition: 0.2s; display: flex; align-items: center; justify-content: center; gap: 6px; border: none; }
        .btn-view { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; }
        .btn-view:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
        .btn-delete { background: #fef2f2; color: #ef4444; border: 1px solid #fee2e2; }
        .btn-delete:hover { background: #ef4444; color: white; }

        /* Modal Styles */
        .modal-overlay { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.7); backdrop-filter: blur(4px); z-index: 9999; display: flex; align-items: center; justify-content: center; padding: 20px; animation: fadeIn 0.2s; }
        .modal-container { background: white; border-radius: 24px; max-width: 900px; width: 100%; max-height: 90vh; overflow: hidden; box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3); position: relative; display: flex; flex-direction: column; animation: slideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1); }
        .modal-close { position: absolute; top: 20px; right: 20px; z-index: 10; background: rgba(0, 0, 0, 0.1); border: none; width: 40px; height: 40px; border-radius: 50%; cursor: pointer; font-size: 20px; font-weight: 700; color: #64748b; transition: 0.2s; display: grid; place-items: center; }
        .modal-close:hover { background: rgba(0, 0, 0, 0.2); transform: rotate(90deg); color: #0f172a; }
        
        .modal-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 32px; display: flex; align-items: center; gap: 16px; }
        .modal-header h2 { font-size: 28px; font-weight: 800; margin: 0; }
        .modal-header p { font-size: 14px; opacity: 0.9; margin: 4px 0 0 0; }
        
        .modal-body { flex: 1; overflow-y: auto; padding: 32px; }
        .result-summary { background: #f0f9ff; border: 1px dashed #667eea; border-radius: 12px; padding: 16px; margin-bottom: 24px; color: #0369a1; font-style: italic; line-height: 1.6; }
        .result-cost { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 16px 20px; border-radius: 12px; margin-bottom: 24px; font-size: 16px; font-weight: 700; display: flex; align-items: center; gap: 8px; }
        .result-cost strong { font-size: 20px; }

        .days-list { display: flex; flex-direction: column; gap: 20px; }
        .day-item { background: #f8fafc; border-radius: 16px; padding: 20px; border: 1px solid #e5e7eb; }
        .day-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 2px solid #e5e7eb; }
        .day-header strong { font-size: 18px; color: #667eea; }
        .day-header span { color: #10b981; font-weight: 700; }

        .activities { display: flex; flex-direction: column; gap: 12px; }
        .activity { display: grid; grid-template-columns: 80px 1fr 120px; gap: 16px; align-items: start; background: white; padding: 16px; border-radius: 12px; border: 1px solid #e5e7eb; }
        .act-time { background: #f0f9ff; color: #667eea; font-weight: 800; padding: 8px; border-radius: 8px; text-align: center; font-size: 13px; }
        .act-details strong { display: block; color: #0f172a; margin-bottom: 6px; font-size: 15px; }
        .act-details p { color: #64748b; font-size: 13px; line-height: 1.5; margin: 0; }
        .act-tips { background: #fef3c7; color: #92400e; padding: 8px 12px; border-radius: 8px; font-size: 12px; margin-top: 8px; border: 1px dashed #fbbf24; }
        .act-cost { color: #10b981; font-weight: 700; text-align: right; font-size: 14px; }

        .empty-schedule { text-align: center; padding: 40px 20px; color: #94a3b8; font-style: italic; }

        @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
        @keyframes slideUp { from { opacity: 0; transform: translateY(40px); } to { opacity: 1; transform: translateY(0); } }
      `}</style>
        </main>
    );
}
