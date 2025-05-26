import socket
import json

# Cấu hình cổng
RECEIVE_PORT = 5052  # Nhận từ Unity
SEND_PORT = 5051     # Gửi lại sang Unity
HOST = '127.0.0.1'

def receive_move():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.bind((HOST, RECEIVE_PORT))
        server.listen(1)
        print(f"[Server] Listening on port {RECEIVE_PORT}...")
        conn, addr = server.accept()
        with conn:
            print(f"[Server] Connected by {addr}")
            data = conn.recv(1024)
            if data:
                move_data = json.loads(data.decode('utf-8'))
                print(f"[Unity Move] {move_data}")
                return move_data
    return None

def send_move(move_dict):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client:
        client.connect((HOST, SEND_PORT))
        json_data = json.dumps(move_dict)
        client.sendall(json_data.encode('utf-8'))
        print(f"[Sent] {json_data}")

def main():
    while True:
        print("\nWaiting for Unity's move...")
        move = receive_move()
        if move is None:
            continue

        # Gợi ý định dạng: e2 → e4
        start = input("Enter AI move start (e.g., e7): ").strip()
        end = input("Enter AI move end (e.g., e5): ").strip()

        # Nếu phong cấp, ví dụ: Q, R, B, N
        promotion = input("Promotion piece (Q/R/B/N or leave blank): ").strip().upper()
        move_response = {
            "start": start,
            "end": end,
            "promotionPiece": promotion
        }

        send_move(move_response)

if __name__ == "__main__":
    main()

