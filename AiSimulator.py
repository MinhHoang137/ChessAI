import socket
import json

HOST = '127.0.0.1'
RECEIVE_PORT = 5052  # Nhận từ Unity (nước đi người chơi)
SEND_PORT = 5051     # Gửi nước đi AI sang Unity
GAMEINFO_PORT = 5050 # Nhận thông tin ván đấu từ Unity


def receive_data(port):
    """Nhận dữ liệu JSON từ Unity qua cổng chỉ định."""
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.bind((HOST, port))
        server.listen(1)
        print(f"[Listening] Port {port}...")
        conn, addr = server.accept()
        with conn:
            print(f"[Connected] From {addr}")
            data = conn.recv(1024)
            if data:
                return json.loads(data.decode('utf-8'))
    return None


def send_move(move_dict):
    """Gửi nước đi sang Unity."""
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client:
        client.connect((HOST, SEND_PORT))
        json_data = json.dumps(move_dict)
        client.sendall(json_data.encode('utf-8'))
        print(f"[Sent] {json_data}")


def main():
    print("📥 Waiting for GameInfo...")
    game_info = receive_data(GAMEINFO_PORT)
    if game_info:
        side = game_info.get("playerSide", "White")  # "White" hoặc "Black"
        depth = game_info.get("depth", 1)
        print(f"[GameInfo] Player is {side}, Depth: {depth}")
    else:
        print("❌ Failed to receive GameInfo.")
        return

    # Nếu người chơi là đen → AI đi trước
    if side.lower() == "black":
        print("\n🤖 AI plays first.")
        start = input("Enter AI move start (e.g., e2): ").strip()
        end = input("Enter AI move end (e.g., e4): ").strip()
        promotion = input("Promotion piece (Q/R/B/N or leave blank): ").strip().upper()

        move_response = {
            "start": start,
            "end": end,
            "promotionPiece": promotion
        }
        send_move(move_response)

    while True:
        print("\n📥 Waiting for Unity's move...")
        move = receive_data(RECEIVE_PORT)
        if move is None:
            continue

        print(f"[Unity Move] {move}")

        # Người dùng nhập nước đi AI tiếp theo
        start = input("Enter AI move start (e.g., e7): ").strip()
        end = input("Enter AI move end (e.g., e5): ").strip()
        promotion = input("Promotion piece (Q/R/B/N or leave blank): ").strip().upper()

        move_response = {
            "start": start,
            "end": end,
            "promotionPiece": promotion
        }

        send_move(move_response)


if __name__ == "__main__":
    main()

