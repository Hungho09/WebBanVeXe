export interface Invoice {
  id: string;
  invoiceNumber: string;
  bookingId: string;
  customerName: string;
  customerEmail: string;
  totalAmount: number;
  createdAt: string;
  status: string;
}
