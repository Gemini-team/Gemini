import time
import socket

client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
client_socket.settimeout(5.0)
message = b'TEST!'
addr = ("127.0.0.1", 50090)

start = time.time()
for i in range(10):
    print("sending message ", i)
    client_socket.sendto(message, addr)
    try:
        data, server = client_socket.recvfrom(1024)
        end = time.time()
        elapsed = end - start
        print(f'{data} {elapsed}')
    except socket.timeout:
        print('REQUEST TIMED OUT')