"use client";

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { ArrowLeftOutlined, CalendarOutlined, CheckCircleFilled, ClockCircleOutlined, ShopOutlined, DeleteOutlined, RocketOutlined, HomeOutlined, CarOutlined, PlusOutlined, DownOutlined, UpOutlined } from '@ant-design/icons';
import { Checkbox } from 'antd'; // Assuming AntD is available or using simple HTML checkbox if not

// Simple money formatter
const money = (x: number) => new Intl.NumberFormat("vi-VN").format(x);

export default function ItineraryPage() {
    const [orders, setOrders] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedItems, setSelectedItems] = useState<number[]>([]);
    const [checkingOut, setCheckingOut] = useState(false);
    const [showPaymentModal, setShowPaymentModal] = useState(false);
    const [paymentQR, setPaymentQR] = useState("");
    const [paymentDetails, setPaymentDetails] = useState<any>(null);
    const [checkoutOrder, setCheckoutOrder] = useState<any>(null);

    // Discovery states
    const [discoveryHotels, setDiscoveryHotels] = useState<any[]>([]);
    const [discoveryCars, setDiscoveryCars] = useState<any[]>([]);
    const [discoveryFlights, setDiscoveryFlights] = useState<any[]>([]);
    const [addingService, setAddingService] = useState(false);
    const [expandedItems, setExpandedItems] = useState<number[]>([]);

    const fetchOrders = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            setLoading(false);
            return;
        }

        try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/GetMyOrders`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                const data = await res.json();
                setOrders(data);
            }
        } catch (error) {
            console.error("Failed to fetch orders", error);
        } finally {
            setLoading(false);
        }
    };

    const fetchDiscoveryData = async (locations: string[], provinceIds: number[]) => {
        if (locations.length === 0 && provinceIds.length === 0) {
            setDiscoveryHotels([]);
            return;
        }
        try {
            const locQuery = locations.join(',');
            const idQuery = provinceIds.join(',');
            const [hRes, cRes, fRes] = await Promise.all([
                fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Discovery/Hotels?location=${encodeURIComponent(locQuery)}&provinceIds=${idQuery}`),
                fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Discovery/Cars`),
                fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Discovery/Flights`)
            ]);
            if (hRes.ok) setDiscoveryHotels(await hRes.json());
            if (cRes.ok) setDiscoveryCars(await cRes.json());
            if (fRes.ok) setDiscoveryFlights(await fRes.json());
        } catch (e) {
            console.error("Discovery error", e);
        }
    }

    const pendingOrder = orders.find(o => o.status === 'Pending');

    useEffect(() => {
        fetchOrders();
    }, []);

    useEffect(() => {
        if (pendingOrder?.orderItems) {
            const locations: string[] = [];
            const provinceIds: number[] = [];

            pendingOrder.orderItems.forEach((item: any) => {
                if (item.scenicSpot?.provinceId) {
                    provinceIds.push(item.scenicSpot.provinceId);
                }

                if (item.scenicSpot?.province?.provinceName) {
                    locations.push(item.scenicSpot.province.provinceName);
                } else if (item.itemName && (item.itemName.includes("Đà Nẵng") || item.itemName.includes("Hội An") || item.itemName.includes("Phú Quốc") || item.itemName.includes("Hà Nội") || item.itemName.includes("Sapa"))) {
                    const names = ["Đà Nẵng", "Hội An", "Phú Quốc", "Hà Nội", "Sapa"];
                    names.forEach(n => { if (item.itemName.includes(n)) locations.push(n); });
                }
            });
            const uniqueLocs = Array.from(new Set(locations));
            const uniqueIds = Array.from(new Set(provinceIds));
            fetchDiscoveryData(uniqueLocs, uniqueIds);
        }
    }, [pendingOrder?.orderItems?.length]);

    const historyOrders = orders.filter(o => o.status !== 'Pending');

    const addServiceToCart = async (item: any, type: string) => {
        const token = localStorage.getItem('token');
        if (!token) return alert("Vui lòng đăng nhập");

        setAddingService(true);
        try {
            const body: any = {
                itemName: item.name || item.model || (`Vé máy bay ${item.airline}`),
                price: item.pricePerNight || item.pricePerDay || item.price,
                quantity: 1,
                serviceType: type,
                serviceId: item.hotelId || item.carRentalId || item.flightId,
                parentItemId: item.parentItemId || null
            };

            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/AddToCart`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(body)
            });

            if (res.ok) {
                fetchOrders();
                alert(`Đã thêm ${body.itemName} vào chuyến đi của bạn!`);
            }
        } catch (e) {
            alert("Lỗi khi thêm dịch vụ");
        } finally {
            setAddingService(false);
        }
    }

    const toggleExpand = (id: number) => {
        setExpandedItems(prev => prev.includes(id) ? prev.filter(i => i !== id) : [...prev, id]);
    };

    const handleToggleItem = (id: number) => {
        setSelectedItems(prev => prev.includes(id) ? prev.filter(i => i !== id) : [...prev, id]);
    };

    const handleSelectAll = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.checked && pendingOrder?.orderItems) {
            setSelectedItems(pendingOrder.orderItems.map((i: any) => i.orderItemId));
        } else {
            setSelectedItems([]);
        }
    };

    const handleRemoveItem = async (id: number) => {
        if (!confirm("Bạn có chắc muốn xóa mục này khỏi hành trình?")) return;

        const token = localStorage.getItem('token');
        try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/RemoveFromCart/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                fetchOrders(); // Refresh
                setSelectedItems(prev => prev.filter(i => i !== id));
            } else {
                alert("Không thể xóa mục này");
            }
        } catch (e) {
            alert("Lỗi kết nối");
        }
    }

    const handleCheckout = async () => {
        if (selectedItems.length === 0) return alert("Vui lòng chọn ít nhất 1 mục để thanh toán");

        setCheckingOut(true);
        const token = localStorage.getItem('token');
        try {
            // 1. First get Payment Info (Bank details from appsettings)
            const payInfoRes = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/GetPaymentInfo`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            const payInfo = await payInfoRes.json();
            setPaymentDetails(payInfo);

            // 2. Perform Checkout
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/Order/Checkout`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(selectedItems)
            });

            if (res.ok) {
                const d = await res.json();
                const orderId = d.newOrderId;
                const amount = selectedTotal;

                // 3. Generate VietQR URL
                // Template: https://img.vietqr.io/image/<BANK_BIN>-<ACC_NO>-<TEMPLATE>.png?amount=<AMOUNT>&addInfo=<INFO>&accountName=<NAME>
                const qrUrl = `https://img.vietqr.io/image/${payInfo.bankBin}-${payInfo.accountNo}-${payInfo.template}.png?amount=${amount}&addInfo=${encodeURIComponent("GOWHERE ORDER " + orderId)}&accountName=${encodeURIComponent(payInfo.accountName)}`;

                setPaymentQR(qrUrl);
                setCheckoutOrder({ id: orderId, total: amount });
                setShowPaymentModal(true);

                fetchOrders();
                setSelectedItems([]);
            } else {
                const d = await res.json();
                alert(d.message || "Lỗi thanh toán");
            }
        } catch (e) {
            alert("Lỗi kết nối server");
        } finally {
            setCheckingOut(false);
        }
    }

    const selectedTotal = pendingOrder?.orderItems
        ?.filter((i: any) => selectedItems.includes(i.orderItemId))
        .reduce((sum: number, i: any) => sum + (i.price * i.quantity), 0) || 0;

    return (
        <div className="page-container">
            <div className="container">
                <div className="header-simple">
                    <Link href="/" className="back-link"><ArrowLeftOutlined /> Quay lại</Link>
                    <h1>Hành trình của tôi</h1>
                </div>

                {loading ? (
                    <div className="loading">Đang tải dữ liệu...</div>
                ) : (
                    <>
                        <div className="section">
                            <h2 className="section-title"><ClockCircleOutlined /> Lịch trình hiện tại (Giỏ hàng)</h2>
                            {pendingOrder && pendingOrder.orderItems && pendingOrder.orderItems.length > 0 ? (
                                <div className="cart-card">
                                    <div className="cart-header-row">
                                        <label className="checkbox-wrap">
                                            <input
                                                type="checkbox"
                                                onChange={handleSelectAll}
                                                checked={selectedItems.length === pendingOrder.orderItems.length && pendingOrder.orderItems.length > 0}
                                            /> Chọn toàn bộ ({pendingOrder.orderItems.length} mục)
                                        </label>
                                    </div>
                                    <div className="cart-items">
                                        {pendingOrder.orderItems.filter((i: any) => !i.parentItemId).map((item: any) => {
                                            const subItems = pendingOrder.orderItems.filter((si: any) => si.parentItemId === item.orderItemId);
                                            const isExpanded = expandedItems.includes(item.orderItemId);
                                            const itemSubTotal = item.price * item.quantity + subItems.reduce((s: number, si: any) => s + (si.price * si.quantity), 0);

                                            return (
                                                <div key={item.orderItemId} className="cart-item-group">
                                                    <div className={`cart-item ${selectedItems.includes(item.orderItemId) ? 'selected' : ''}`}>
                                                        <div className="checkbox-col">
                                                            <input
                                                                type="checkbox"
                                                                checked={selectedItems.includes(item.orderItemId)}
                                                                onChange={() => {
                                                                    handleToggleItem(item.orderItemId);
                                                                    // Also toggle sub items
                                                                    subItems.forEach((si: any) => {
                                                                        const isSelected = selectedItems.includes(item.orderItemId);
                                                                        if (isSelected) {
                                                                            setSelectedItems(prev => prev.filter(x => x !== si.orderItemId));
                                                                        } else {
                                                                            setSelectedItems(prev => Array.from(new Set([...prev, si.orderItemId])));
                                                                        }
                                                                    });
                                                                }}
                                                            />
                                                        </div>
                                                        <div className="item-thumb">
                                                            {item.scenicSpot?.imageUrl ? (
                                                                <img src={item.scenicSpot.imageUrl} alt={item.itemName} />
                                                            ) : (
                                                                <div className="placeholder-icon">
                                                                    {item.serviceType === 'Flight' && <RocketOutlined />}
                                                                    {item.serviceType === 'Hotel' && <HomeOutlined />}
                                                                    {item.serviceType === 'Car' && <CarOutlined />}
                                                                    {(item.serviceType === 'Tour' || !item.serviceType) && <ShopOutlined />}
                                                                </div>
                                                            )}
                                                        </div>
                                                        <div className="item-info">
                                                            <div className="item-type-badge">{item.serviceType || 'Tour'}</div>
                                                            <h3>{item.itemName}</h3>
                                                            <p className="desc">
                                                                {item.scenicSpot?.description ? (item.scenicSpot.description.substring(0, 100) + '...') : 'Điểm đến trong lịch trình'}
                                                            </p>
                                                            <p className="price-line">{money(item.price)} VND x {item.quantity}</p>
                                                        </div>
                                                        <div className="item-total">
                                                            {money(itemSubTotal)} VND
                                                            <div className="sub-label">{subItems.length > 0 ? `(Gồm ${subItems.length} dịch vụ thêm)` : ''}</div>
                                                        </div>
                                                        <div className="item-actions">
                                                            <button className={`btn-expand ${isExpanded ? 'active' : ''}`} onClick={() => toggleExpand(item.orderItemId)}>
                                                                {isExpanded ? <UpOutlined /> : <DownOutlined />} Mở rộng
                                                            </button>
                                                            <button className="btn-icon" onClick={() => handleRemoveItem(item.orderItemId)}><DeleteOutlined /></button>
                                                        </div>
                                                    </div>

                                                    {isExpanded && (
                                                        <div className="extension-area">
                                                            {subItems.length > 0 && (
                                                                <div className="sub-list">
                                                                    <h5>Dịch vụ đã chọn:</h5>
                                                                    {subItems.map((si: any) => (
                                                                        <div key={si.orderItemId} className="sub-item-row-added">
                                                                            <span className="s-type">{si.serviceType === 'Hotel' ? <HomeOutlined /> : <CarOutlined />}</span>
                                                                            <span className="s-name">{si.itemName}</span>
                                                                            <span className="s-price">{money(si.price)} VND</span>
                                                                            <button className="btn-remove-sub" onClick={() => handleRemoveItem(si.orderItemId)}>Xóa</button>
                                                                        </div>
                                                                    ))}
                                                                </div>
                                                            )}

                                                            <div className="extension-discovery">
                                                                <div className="ext-tabs">
                                                                    <div className="ext-tab-header">
                                                                        <PlusOutlined /> Thêm dịch vụ cho {item.itemName}
                                                                    </div>
                                                                    <div className="ext-grid">
                                                                        <div className="ext-col">
                                                                            <h6><HomeOutlined /> Lựa chọn khách sạn</h6>
                                                                            <div className="ext-list">
                                                                                {discoveryHotels.filter(h => h.provinceId === item.scenicSpot?.provinceId).length > 0 ? (
                                                                                    discoveryHotels.filter(h => h.provinceId === item.scenicSpot?.provinceId).map(h => (
                                                                                        <div key={h.hotelId} className="ext-item-card">
                                                                                            <span className="n">{h.name}</span>
                                                                                            <span className="p">{money(h.pricePerNight)}</span>
                                                                                            <button className="btn-add-ext" onClick={() => addServiceToCart({ ...h, parentItemId: item.orderItemId }, 'Hotel')}>+ Thêm</button>
                                                                                        </div>
                                                                                    ))
                                                                                ) : (
                                                                                    <div className="small-empty">Không có khách sạn phù hợp.</div>
                                                                                )}
                                                                            </div>
                                                                        </div>
                                                                        <div className="ext-col">
                                                                            <h6><CarOutlined /> Lựa chọn thuê xe</h6>
                                                                            <div className="ext-list">
                                                                                {discoveryCars.slice(0, 3).map(c => (
                                                                                    <div key={c.carRentalId} className="ext-item-card">
                                                                                        <span className="n">{c.model}</span>
                                                                                        <span className="p">{money(c.pricePerDay)}</span>
                                                                                        <button className="btn-add-ext" onClick={() => addServiceToCart({ ...c, parentItemId: item.orderItemId }, 'Car')}>+ Thêm</button>
                                                                                    </div>
                                                                                ))}
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    )}
                                                </div>
                                            );
                                        })}
                                    </div>

                                    <div className="cart-footer">
                                        <div className="total-info">
                                            <div className="total-label">Tổng thanh toán:</div>
                                            <div className="total-val">{money(selectedTotal)} VND</div>
                                        </div>
                                        <button
                                            className="btn primary checkout-btn"
                                            onClick={handleCheckout}
                                            disabled={selectedItems.length === 0 || checkingOut}
                                        >
                                            {checkingOut ? 'Đang xử lý...' : 'Thanh toán ngay'}
                                        </button>
                                    </div>
                                </div>
                            ) : (
                                <div className="empty-state">Chưa có địa điểm nào trong hành trình hiện tại. <Link href="/">Khám phá ngay!</Link></div>
                            )}
                        </div>

                        {historyOrders.length > 0 && (
                            <div className="section">
                                <h2 className="section-title"><CheckCircleFilled /> Lịch sử chuyến đi</h2>
                                <div className="history-list">
                                    {historyOrders.map(order => (
                                        <div key={order.orderId} className="history-card">
                                            <div className="h-header">
                                                <span className="date">{new Date(order.orderDate).toLocaleDateString('vi-VN')}</span>
                                                <span className={`status ${order.status.toLowerCase()}`}>{order.status}</span>
                                            </div>
                                            <div className="h-items">
                                                {order.orderItems?.map((item: any) => (
                                                    <div key={item.orderItemId} className="h-item">
                                                        <div className="h-thumb">
                                                            {item.scenicSpot?.imageUrl && <img src={item.scenicSpot.imageUrl} alt="" />}
                                                        </div>
                                                        <div className="h-det">
                                                            <strong>{item.itemName}</strong>
                                                            <span>x{item.quantity}</span>
                                                        </div>
                                                        <div className="h-price">{money(item.price * item.quantity)}</div>
                                                    </div>
                                                ))}
                                            </div>
                                            <div className="h-total">
                                                Tổng cộng: <span>{money(order.totalPrice)} VND</span>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </>
                )}
            </div>

            {showPaymentModal && checkoutOrder && (
                <div className="payment-modal-overlay">
                    <div className="payment-modal">
                        <div className="pay-header">
                            <h3>Xác nhận thanh toán</h3>
                            <button className="btn-close" onClick={() => setShowPaymentModal(false)}><DeleteOutlined rotate={45} /></button>
                        </div>
                        <div className="pay-body">
                            <div className="qr-box">
                                <img src={paymentQR} alt="VietQR Payment" />
                                <div className="qr-hint">Sử dụng ứng dụng Ngân hàng hoặc Ví điện tử để quét mã QR</div>
                            </div>
                            <div className="pay-info-summary">
                                <div className="p-row"><span>Mã đơn hàng:</span> <strong>#GW-{checkoutOrder.id}</strong></div>
                                <div className="p-row"><span>Số tiền:</span> <strong className="p-amount">{money(checkoutOrder.total)} VND</strong></div>
                                <div className="p-row"><span>Nội dung:</span> <strong>GOWHERE ORDER {checkoutOrder.id}</strong></div>
                                <div className="p-row"><span>Chủ tài khoản:</span> <strong>{paymentDetails?.accountName}</strong></div>
                            </div>
                            <div className="pay-notice">
                                <CheckCircleFilled /> Hệ thống sẽ ghi nhận sau khi bạn thực hiện chuyển khoản thành công.
                            </div>
                        </div>
                        <div className="pay-footer">
                            <button className="btn primary full" onClick={() => setShowPaymentModal(false)}>Tôi đã thanh toán xong</button>
                        </div>
                    </div>
                </div>
            )}

            <style jsx>{`
        .page-container { background: #f8fafc; min-height: 100vh; padding-bottom: 80px; }
        .container { max-width: 900px; margin: 0 auto; padding: 40px 20px; }
        .header-simple { display: flex; align-items: center; gap: 20px; margin-bottom: 40px; }
        .back-link { color: #64748b; font-weight: 600; display: flex; align-items: center; gap: 8px; transition: 0.2s; text-decoration: none; }
        .back-link:hover { color: #0ea5e9; }
        h1 { margin: 0; font-size: 28px; color: #1e293b; }

        .section { margin-bottom: 40px; }
        .section-title { font-size: 18px; margin-bottom: 16px; display: flex; align-items: center; gap: 8px; color: #1e293b; }
        
        .cart-card { background: white; border-radius: 20px; padding: 24px; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1); border: 1px solid #f1f5f9; }
        .cart-header-row { padding-bottom: 16px; border-bottom: 1px solid #f1f5f9; margin-bottom: 8px; }
        .checkbox-wrap { font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 8px; user-select: none; color: #475569; }
        .checkbox-wrap input { width: 18px; height: 18px; cursor: pointer; }

        .cart-item { display: flex; align-items: center; gap: 16px; padding: 16px 0; border-bottom: 1px dashed #f1f5f9; transition: 0.2s; }
        .cart-item:last-child { border-bottom: none; }
        .cart-item.selected { background: #f0f9ff; margin: 0 -24px; padding: 16px 24px; }
        
        .checkbox-col input { width: 18px; height: 18px; cursor: pointer; }
        
        .item-thumb { width: 80px; height: 60px; border-radius: 8px; overflow: hidden; background: #f1f5f9; flex-shrink: 0; }
        .item-thumb img { width: 100%; height: 100%; object-fit: cover; }
        .placeholder-icon { width: 100%; height: 100%; display: grid; place-items: center; color: #94a3b8; font-size: 24px; }

        .item-info { flex: 1; }
        .item-type-badge { display: inline-block; padding: 2px 8px; border-radius: 4px; background: #e0f2fe; color: #0ea5e9; font-size: 10px; font-weight: 700; text-transform: uppercase; margin-bottom: 4px; }
        .item-info h3 { font-size: 16px; margin: 0 0 4px 0; color: #1e293b; }
        .item-info .desc { font-size: 13px; color: #64748b; margin-bottom: 4px; line-height: 1.4; }
        .item-info .price-line { font-size: 13px; font-weight: 600; color: #475569; }

        .item-total { font-weight: 700; font-size: 16px; min-width: 100px; text-align: right; color: #1e293b; }
        .item-actions { margin-left: 12px; }
        .btn-icon { background: none; border: none; color: #ef4444; cursor: pointer; padding: 8px; border-radius: 6px; transition: 0.2s; }
        .btn-icon:hover { background: #fee2e2; }

        .cart-footer { margin-top: 24px; padding-top: 24px; border-top: 2px solid #f1f5f9; display: flex; align-items: center; justify-content: flex-end; gap: 30px; }
        .total-info { text-align: right; }
        .total-label { font-size: 14px; font-weight: 600; color: #64748b; }
        .total-val { font-size: 28px; font-weight: 800; color: #0ea5e9; }
        .checkout-btn { min-width: 180px; }
        .checkout-btn:disabled { opacity: 0.6; cursor: not-allowed; }

        .btn { padding: 12px 24px; border-radius: 12px; font-weight: 700; cursor: pointer; border: none; transition: 0.3s; font-size: 15px; }
        .btn.primary { background: #0ea5e9; color: white; box-shadow: 0 4px 14px 0 rgba(14, 165, 233, 0.39); }
        .btn.primary:hover { background: #0284c7; transform: translateY(-1px); box-shadow: 0 6px 20px rgba(14, 165, 233, 0.23); }

        .empty-state { padding: 60px; text-align: center; background: white; border-radius: 20px; color: #64748b; border: 2px dashed #e2e8f0; }
        .empty-state a { color: #0ea5e9; font-weight: 700; text-decoration: none; margin-left: 5px; }

        .history-list { display: grid; gap: 16px; }
        .history-card { background: white; padding: 20px; border-radius: 16px; border: 1px solid #e2e8f0; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
        .h-header { display: flex; justify-content: space-between; margin-bottom: 12px; font-size: 14px; color: #64748b; }
        .status { font-weight: 700; text-transform: uppercase; font-size: 11px; padding: 4px 10px; border-radius: 6px; letter-spacing: 0.5px; }
        .status.paid { background: #dcfce7; color: #16a34a; }
        .status.payed { background: #dcfce7; color: #16a34a; }
        .status.waitpayment { background: #fee2e2; color: #ef4444; }
        .status.pending { background: #fef9c3; color: #ca8a04; }
        
        .h-items { border-top: 1px solid #f1f5f9; padding-top: 12px; }
        .h-item { display: flex; align-items: center; gap: 12px; margin-bottom: 10px; font-size: 14px; }
        .h-thumb { width: 45px; height: 35px; background: #f1f5f9; border-radius: 6px; overflow: hidden; }
        .h-thumb img { width: 100%; height: 100%; object-fit: cover; }
        .h-det { flex: 1; display: flex; gap: 8px; color: #334155; }
        .h-price { font-weight: 600; color: #1e293b; }

        .h-total { margin-top: 12px; padding-top: 12px; border-top: 1px dashed #e2e8f0; text-align: right; font-size: 15px; color: #475569; }
        .h-total span { font-weight: 800; color: #0ea5e9; font-size: 18px; margin-left: 8px; }

        /* Integrated Suggestions */
        .cart-additional-services { background: #f8fafc; border-radius: 16px; padding: 20px; margin: 24px 0; border: 1px solid #e2e8f0; }
        .sug-header h4 { margin: 0; font-size: 15px; font-weight: 700; display: flex; align-items: center; gap: 8px; color: #1e293b; }
        .sug-header .sub { font-size: 12px; color: #64748b; margin: 4px 0 16px 0; }
        .sug-list-simple { display: flex; flex-direction: column; gap: 12px; }
        .sug-item-row { display: flex; align-items: center; gap: 16px; background: white; padding: 12px; border-radius: 12px; border: 1px solid transparent; transition: 0.3s; }
        .sug-item-row:hover { border-color: #0ea5e9; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }
        .s-thumb { width: 50px; height: 50px; border-radius: 8px; overflow: hidden; flex-shrink: 0; }
        .s-thumb img { width: 100%; height: 100%; object-fit: cover; }
        .s-info { flex: 1; }
        .s-name { font-size: 14px; font-weight: 700; color: #1e293b; margin-bottom: 2px; }
        .s-price { font-size: 13px; color: #0ea5e9; font-weight: 700; }
        .btn-add-sug { background: white; border: 1.5px solid #0ea5e9; color: #0ea5e9; padding: 6px 14px; border-radius: 8px; font-size: 12px; font-weight: 700; cursor: pointer; transition: 0.2s; }
        .btn-add-sug:hover { background: #0ea5e9; color: white; }
        
        .no-sug-alert { padding: 20px; text-align: center; color: #94a3b8; font-size: 13px; display: flex; align-items: center; justify-content: center; gap: 10px; background: #fff; border-radius: 12px; border: 1px dashed #cbd5e1; }
        
        .loading { padding: 100px; text-align: center; color: #64748b; font-size: 18px; }

        /* Extension Styles */
        .cart-item-group { border-bottom: 1px solid #f1f5f9; }
        .cart-item-group:last-child { border-bottom: none; }
        .btn-expand { display: flex; align-items: center; gap: 6px; background: #f8fafc; border: 1px solid #e2e8f0; padding: 6px 12px; border-radius: 8px; font-size: 12px; font-weight: 600; color: #64748b; cursor: pointer; transition: 0.2s; }
        .btn-expand:hover { background: #e0f2fe; color: #0ea5e9; border-color: #0ea5e9; }
        .btn-expand.active { background: #0ea5e9; color: white; border-color: #0ea5e9; }

        .extension-area { background: #f8fafc; margin: 0 -24px; padding: 20px 24px; border-top: 1px solid #f1f5f9; border-bottom: 1px solid #f1f5f9; }
        .sub-list { margin-bottom: 20px; }
        .sub-list h5 { font-size: 13px; color: #64748b; margin-bottom: 8px; }
        .sub-item-row-added { display: flex; align-items: center; gap: 12px; background: white; padding: 10px 16px; border-radius: 10px; margin-bottom: 8px; border: 1px solid #e2e8f0; }
        .sub-item-row-added .s-type { color: #0ea5e9; }
        .sub-item-row-added .s-name { flex: 1; font-size: 13px; font-weight: 600; }
        .sub-item-row-added .s-price { font-weight: 700; color: #1e293b; font-size: 13px; }
        .btn-remove-sub { background: none; border: none; color: #94a3b8; font-size: 11px; cursor: pointer; text-decoration: underline; }
        .btn-remove-sub:hover { color: #ef4444; }

        .ext-tab-header { font-size: 14px; font-weight: 700; color: #1e293b; margin-bottom: 16px; display: flex; align-items: center; gap: 8px; }
        .ext-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
        .ext-col h6 { font-size: 12px; color: #64748b; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 0.5px; }
        .ext-list { display: flex; flex-direction: column; gap: 8px; }
        .ext-item-card { background: white; padding: 10px; border-radius: 8px; border: 1px solid #e2e8f0; display: flex; align-items: center; justify-content: space-between; gap: 10px; }
        .ext-item-card .n { font-size: 12px; font-weight: 700; color: #334155; flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .ext-item-card .p { font-size: 12px; color: #0ea5e9; font-weight: 700; }
        .btn-add-ext { background: #f0f9ff; border: 1px solid #e0f2fe; color: #0ea5e9; font-size: 10px; font-weight: 700; padding: 4px 8px; border-radius: 4px; cursor: pointer; }
        .btn-add-ext:hover { background: #0ea5e9; color: white; }
        .small-empty { font-size: 11px; color: #94a3b8; font-style: italic; }
        .sub-label { font-size: 11px; color: #0ea5e9; font-weight: 600; margin-top: 4px; }

        /* Payment Modal */
        .payment-modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); backdrop-filter: blur(4px); z-index: 2000; display: grid; place-items: center; padding: 20px; }
        .payment-modal { background: white; width: 100%; max-width: 440px; border-radius: 24px; overflow: hidden; box-shadow: 0 30px 60px -12px rgba(0,0,0,0.25); animation: zoomIn 0.3s cubic-bezier(0.16, 1, 0.3, 1); }
        .pay-header { padding: 20px 24px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
        .pay-header h3 { margin: 0; font-size: 18px; color: #1e293b; }
        .btn-close { background: none; border: none; font-size: 20px; color: #94a3b8; cursor: pointer; }
        .pay-body { padding: 24px; }
        .qr-box { background: #f8fafc; border-radius: 16px; padding: 20px; text-align: center; border: 1px solid #e2e8f0; margin-bottom: 24px; }
        .qr-box img { width: 240px; height: 240px; background: white; padding: 10px; border-radius: 12px; margin-bottom: 12px; border: 1px solid #f1f5f9; }
        .qr-hint { font-size: 12px; color: #64748b; font-weight: 500; }
        
        .pay-info-summary { display: flex; flex-direction: column; gap: 10px; margin-bottom: 24px; }
        .p-row { display: flex; justify-content: space-between; font-size: 14px; }
        .p-row span { color: #64748b; }
        .p-row strong { color: #1e293b; }
        .p-amount { color: #0ea5e9 !important; font-size: 18px; }
        
        .pay-notice { background: #f0fdf4; color: #16a34a; padding: 12px; border-radius: 12px; font-size: 13px; font-weight: 600; display: flex; gap: 8px; align-items: center; }
        .pay-footer { padding: 0 24px 24px 24px; }
        .btn.full { width: 100%; }

        @keyframes zoomIn { from { opacity: 0; transform: scale(0.95); } to { opacity: 1; transform: scale(1); } }
      `}</style>
        </div>
    );
}
