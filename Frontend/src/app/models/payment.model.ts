export interface CreatePaymentDto {
  bookingId: string;
  amount: number;
  paymentMethod: string;
  transactionCode?: string;
}

export interface PaymentResultDto {
  id: string;
  bookingId: string;
  amount: number;
  paymentMethod: string;
  transactionCode: string;
  paymentStatus: string;   // 'Pending', 'Success', 'Failed'
  bookingStatus: string;   // 'Pending', 'Confirmed', 'CancelRequested', 'Cancelled'
  paidAt: string | null;
}
