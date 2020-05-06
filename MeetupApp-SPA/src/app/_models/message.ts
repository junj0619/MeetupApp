export interface Message {
  id: number;
  senderId: number;
  senderKnownAs: string;
  senderPhotoUrl: string;
  recipientId: number;
  recipientKnownAs: string;
  recipientPhotoUrl: string;
  messageContainer: string;
  content: string;
  messageSent: string;
  isRead: boolean;
  dateRead: string;
}
